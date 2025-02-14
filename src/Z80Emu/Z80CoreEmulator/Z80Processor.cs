﻿using Konamiman.Z80dotNet.Enums;
using Konamiman.Z80dotNet.Z80EventArgs;
using System.Collections.ObjectModel;
using System.Net;

namespace Konamiman.Z80dotNet
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.

    /// <summary>
    /// The implementation of the <see cref="IZ80Processor"/> interface.
    /// </summary>
    public class Z80Processor : IZ80Processor, IZ80ProcessorAgent
    {
        private const int MemorySpaceSize = 65536;
        private const int PortSpaceSize = 256;

        private const decimal MaxEffectiveClockSpeed = 100M;
        private const decimal MinEffectiveClockSpeed = 0.001M;

        private const ushort NmiServiceRoutine = 0x66;
        private const byte NOP_opcode = 0x00;
        private const byte RST38h_opcode = 0xFF;

        public Z80Processor()
        {
            ClockSynchronizer = new ClockSynchronizer();
            Registers = new Z80Registers();

            ClockFrequencyInMHz = 4;
            ClockSpeedFactor = 1;

            AutoStopOnDiPlusHalt = true;
            AutoStopOnRetWithStackEmpty = false;
            unchecked { Registers.InitializeSP((short)0xFFFF); }

            SetMemoryWaitStatesForM1(0, MemorySpaceSize, 0);
            SetMemoryWaitStatesForNonM1(0, MemorySpaceSize, 0);
            SetPortWaitStates(0, PortSpaceSize, 0);

            Memory = new PlainMemory(MemorySpaceSize);
            PortsSpace = new PlainMemory(PortSpaceSize);

            SetMemoryAccessMode(0, MemorySpaceSize, MemoryAccessMode.ReadAndWrite);
            SetPortsSpaceAccessMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            InterruptSources = [];

            StopReason = StopReason.NeverRan;
            State = ProcessorState.Stopped;
        }

        #region Processor control

        public void Start(object? userState = null)
        {
            if (userState != null)
                UserState = userState;

            Reset();
            TStatesElapsedSinceStart = 0;

            InstructionExecutionLoop();
        }

        public void Continue()
        {
            InstructionExecutionLoop();
        }

        private int InstructionExecutionLoop(bool isSingleInstruction = false)
        {
            try
            {
                return InstructionExecutionLoopCore(isSingleInstruction);
            }
            catch
            {
                State = ProcessorState.Stopped;
                StopReason = StopReason.ExceptionThrown;

                throw;
            }
        }

        private readonly HashSet<ushort> activeBreakpoints = [];
        private readonly List<Breakpoint> breakpoints = new();
        public ReadOnlyCollection<Breakpoint> Breakpoints => breakpoints.AsReadOnly();

        public void AddBreakpoint(string name, int address)
        {
            breakpoints.Add(new Breakpoint(name, address));
            var uAddress = (ushort)address;
            activeBreakpoints.Add(uAddress);
        }

        public void RemoveBreakpoint(string name)
        {
            breakpoints.RemoveAll(b => b.Name == name);
            activeBreakpoints.Clear();
            foreach (var breakpoint in breakpoints)
            {
                var uAddress = (ushort)breakpoint.Address;
                activeBreakpoints.Add(uAddress);
            }
        }

        public void ClearBreakpoints()
        {
            breakpoints.Clear();
            activeBreakpoints.Clear();
        }

        /// <summary>
        /// Executes the next instruction and returns the number of T-states elapsed.
        /// </summary>
        /// <param name="singleStep">If this value is true, only a single step (one complete opcode) will be executed</param>
        /// <returns></returns>
        private int InstructionExecutionLoopCore(bool singleStep)
        {
            if (clockSynchronizer != null) clockSynchronizer.Start();
            executionContext = new InstructionExecutionContext();
            StopReason = StopReason.NotApplicable;
            State = ProcessorState.Running;
            var totalTStates = 0;

            while (!executionContext.MustStop)
            {
                executionContext.StartNewInstruction();

                FireBeforeInstructionFetchEvent();
                if (executionContext.MustStop)
                    break;

                var executionTStates = ExecuteNextOpcode();

                totalTStates = executionTStates + executionContext.AccummulatedMemoryWaitStates;
                TStatesElapsedSinceStart += (ulong)totalTStates;
                TStatesElapsedSinceReset += (ulong)totalTStates;

                ThrowIfNoFetchFinishedEventFired();

                // singleStep is true if the 
                if (!singleStep)
                {
                    CheckAutoStopForHaltOnDi();
                    CheckForAutoStopForRetWithStackEmpty();
                    CheckForLdSpInstruction();
                    CheckForAutoStopOnRetInstruction();
                    CheckForAutoStopOnStackLimits();
                    CheckBreakpoints();
                    CheckAutostopOnCycleCount();
                }

                FireAfterInstructionExecutionEvent(totalTStates);

                if (!IsHalted)
                    IsHalted = executionContext.IsHaltInstruction;

                var interruptTStates = AcceptPendingInterrupt();
                totalTStates += interruptTStates;
                TStatesElapsedSinceStart += (ulong)interruptTStates;
                TStatesElapsedSinceReset += (ulong)interruptTStates;

                if (singleStep)
                {
                    executionContext.StopReason = StopReason.ExecuteNextInstructionInvoked;
                }
                else if (clockSynchronizer != null)
                {
                    clockSynchronizer.TryWait(totalTStates);
                }
            }

            if (clockSynchronizer != null)
                clockSynchronizer.Stop();
            StopReason = executionContext.StopReason;
            State =
                StopReason == StopReason.PauseInvoked
                    ? ProcessorState.Paused
                    : ProcessorState.Stopped;

            executionContext = null;

            return totalTStates;
        }

        private int ExecuteNextOpcode()
        {
            ArgumentNullException.ThrowIfNull(executionContext);
            if (IsHalted)
            {
                executionContext.OpcodeBytes.Add(NOP_opcode);
                return InstructionExecutor.Execute(NOP_opcode);
            }

            return InstructionExecutor.Execute(FetchNextOpcode());
        }

        private int AcceptPendingInterrupt()
        {
            ArgumentNullException.ThrowIfNull(executionContext);
            if (executionContext.IsEiOrDiInstruction)
                return 0;

            if (NmiInterruptPending)
            {
                IsHalted = false;
                Registers.IFF1 = 0;
                ExecuteCall(NmiServiceRoutine);
                return 11;
            }

            if (!InterruptsEnabled)
                return 0;

            var activeIntSource = InterruptSources.FirstOrDefault(s => s.IntLineIsActive);
            if (activeIntSource == null)
                return 0;

            Registers.IFF1 = 0;
            Registers.IFF2 = 0;
            IsHalted = false;

            switch (InterruptMode)
            {
                case 0:
                    var opcode = activeIntSource.ValueOnDataBus.GetValueOrDefault(0xFF);
                    InstructionExecutor.Execute(opcode);
                    return 13;

                case 1:
                    InstructionExecutor.Execute(RST38h_opcode);
                    return 13;

                case 2:
                    var pointerAddress = NumberUtils.CreateUshort(
                        lowByte: activeIntSource.ValueOnDataBus.GetValueOrDefault(0xFF),
                        highByte: Registers.I);
                    var callAddress = NumberUtils.CreateUshort(
                        lowByte: ReadFromMemoryInternal(pointerAddress),
                        highByte: ReadFromMemoryInternal((ushort)(pointerAddress + 1)));
                    ExecuteCall(callAddress);
                    return 19;
            }

            return 0;
        }

        public void ExecuteCall(ushort address)
        {
            var oldAddress = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            WriteToMemoryInternal(sp, oldAddress.GetHighByte());
            sp = (ushort)(sp - 1);
            WriteToMemoryInternal(sp, oldAddress.GetLowByte());

            Registers.DecSp();
            Registers.PC = address;
        }

        public void ExecuteRet()
        {
            var sp = (ushort)Registers.SP;
            var newPC = NumberUtils.CreateShort(ReadFromMemoryInternal(sp), ReadFromMemoryInternal((ushort)(sp + 1)));

            Registers.PC = (ushort)newPC;
            Registers.IncSp();
        }

        private void ThrowIfNoFetchFinishedEventFired()
        {
            if (executionContext.FetchComplete)
                return;

            throw new InstructionFetchFinishedEventNotFiredException(
                instructionAddress: (ushort)(Registers.PC - executionContext.OpcodeBytes.Count),
                fetchedBytes: [.. executionContext.OpcodeBytes]);
        }

        private void CheckAutoStopForHaltOnDi()
        {
            if (AutoStopOnDiPlusHalt && executionContext.IsHaltInstruction && !InterruptsEnabled)
                executionContext.StopReason = StopReason.DiPlusHalt;
        }

        private void CheckForAutoStopOnRetInstruction()
        {
            if (AutoStopOnRetInstruction && executionContext.IsRetInstruction)
                executionContext.StopReason = StopReason.RetInstruction;
        }

        private void CheckBreakpoints()
        {
            if (activeBreakpoints.Contains(Registers.PC))
                executionContext.StopReason = StopReason.Breakpoint;
        }

        private void CheckAutostopOnCycleCount()
        {
            if (AutoStopOnCycleCount > 0)
            {
                if (TStatesElapsedSinceStart >= (ulong)AutoStopOnCycleCount)
                    executionContext.StopReason = StopReason.CycleCountReached;
            }
        }

        private void CheckForAutoStopOnStackLimits()
        {
            if (AutoStopOnStackLimits)
            {
                if (registers.StackUnderflow)
                {
                    executionContext.StopReason = StopReason.StackUnderflow;
                }
                if (registers.StackOverflow)
                {
                    executionContext.StopReason = StopReason.StackOverflow;
                }
            }
        }

        private void CheckForAutoStopForRetWithStackEmpty()
        {
            if (AutoStopOnRetWithStackEmpty && executionContext.IsRetInstruction && StackIsEmpty())
                executionContext.StopReason = StopReason.RetWithStackEmpty;
        }

        private void CheckForLdSpInstruction()
        {
            if (executionContext.IsLdSpInstruction)
            {
                var sp = Registers.SP;
                Registers.InitializeSP(sp);
                // TODO : Raise SP initialization event ?
            }
        }

        public bool StackIsEmpty()
        {
            return executionContext.SpAfterInstructionFetch == Registers.StartOfStack;
        }

        private bool InterruptsEnabled => Registers.IFF1 == 1;

        private void FireAfterInstructionExecutionEvent(int tStates)
        {
            AfterInstructionExecution?.Invoke(this, new AfterInstructionExecutionEventArgs(
                [.. executionContext.OpcodeBytes],
                stopper: this,
                localUserState: executionContext.LocalUserStateFromPreviousEvent,
                tStates: tStates));
        }

        private void InstructionExecutor_InstructionFetchFinished(object? sender, InstructionFetchFinishedEventArgs e)
        {
            if (executionContext.FetchComplete)
                return;

            executionContext.FetchComplete = true;

            executionContext.IsRetInstruction = e.IsRetInstruction;
            executionContext.IsLdSpInstruction = e.IsLdSpInstruction;
            executionContext.IsHaltInstruction = e.IsHaltInstruction;
            executionContext.IsEiOrDiInstruction = e.IsEiOrDiInstruction;

            executionContext.SpAfterInstructionFetch = Registers.SP;

            var eventArgs = FireBeforeInstructionExecutionEvent();
            executionContext.LocalUserStateFromPreviousEvent = eventArgs.LocalUserState;
        }

        private void FireBeforeInstructionFetchEvent()
        {
            var eventArgs = new BeforeInstructionFetchEventArgs(stopper: this);

            if (BeforeInstructionFetch != null)
            {
                executionContext.ExecutingBeforeInstructionEvent = true;
                try
                {
                    BeforeInstructionFetch(this, eventArgs);
                }
                finally
                {
                    executionContext.ExecutingBeforeInstructionEvent = false;
                }
            }

            executionContext.LocalUserStateFromPreviousEvent = eventArgs.LocalUserState;
        }

        private BeforeInstructionExecutionEventArgs FireBeforeInstructionExecutionEvent()
        {
            var eventArgs = new BeforeInstructionExecutionEventArgs(
                [.. executionContext.OpcodeBytes],
                executionContext.LocalUserStateFromPreviousEvent);

            if (BeforeInstructionExecution != null)
                BeforeInstructionExecution(this, eventArgs);

            return eventArgs;
        }

        public void Reset()
        {
            Registers.IFF1 = 0;
            Registers.IFF2 = 0;
            Registers.PC = 0;
            unchecked { Registers.AF = (short)0xFFFF; }
            unchecked { Registers.InitializeSP((short)0xFFFF); }
            InterruptMode = 0;

            NmiInterruptPending = false;
            IsHalted = false;

            TStatesElapsedSinceReset = 0;
        }

        public int ExecuteNextInstruction()
        {
            return InstructionExecutionLoop(isSingleInstruction: true);
        }

        #endregion Processor control

        #region Information and state

        public ulong TStatesElapsedSinceStart { get; private set; }

        public ulong TStatesElapsedSinceReset { get; private set; }

        public StopReason StopReason { get; private set; }

        public ProcessorState State { get; private set; }

        public object? UserState { get; set; }

        public bool IsHalted { get; protected set; }

        private byte _InterruptMode;

        public byte InterruptMode
        {
            get => _InterruptMode;
            set
            {
                if (value > 2)
                    throw new ArgumentException("Interrupt mode can be set to 0, 1 or 2 only");

                _InterruptMode = value;
            }
        }

        #endregion Information and state

        #region Inside and outside world

        public IZ80Registers Registers
        {
            get => registers;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                registers = value;
            }
        }

        public IMemory Memory
        {
            get => memory; set
            {
                ArgumentNullException.ThrowIfNull(value);
                memory = value;
            }
        }

        private readonly MemoryAccessMode[] memoryAccessModes = new MemoryAccessMode[MemorySpaceSize];

        public void SetMemoryAccessMode(ushort startAddress, int length, MemoryAccessMode mode)
        {
            SetArrayContents(memoryAccessModes, startAddress, length, mode);
        }

        private static void SetArrayContents<T>(T[] array, ushort startIndex, int length, T value)
        {
            if (length < 0)
                throw new ArgumentException("length can't be negative");
            if (startIndex + length > array.Length)
                throw new ArgumentException("start + length go beyond " + (array.Length - 1));

            var data = Enumerable.Repeat(value, length).ToArray();
            Array.Copy(data, 0, array, startIndex, length);
        }

        public MemoryAccessMode GetMemoryAccessMode(ushort address)
        {
            return memoryAccessModes[address];
        }

        public IMemory PortsSpace
        {
            get => portsSpace;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                portsSpace = value;
            }
        }

        private readonly MemoryAccessMode[] portsAccessModes = new MemoryAccessMode[PortSpaceSize];

        public void SetPortsSpaceAccessMode(byte startPort, int length, MemoryAccessMode mode)
        {
            SetArrayContents(portsAccessModes, startPort, length, mode);
        }

        public MemoryAccessMode GetPortAccessMode(byte portNumber)
        {
            return portsAccessModes[portNumber];
        }

        private IList<IZ80InterruptSource> InterruptSources { get; set; }

        public void RegisterInterruptSource(IZ80InterruptSource source)
        {
            if (InterruptSources.Contains(source))
                return;

            InterruptSources.Add(source);
            source.NmiInterruptPulse += (sender, args) => NmiInterruptPending = true;
        }

        private readonly object nmiInterruptPendingSync = new();
        private bool _nmiInterruptPending;

        private bool NmiInterruptPending
        {
            get
            {
                lock (nmiInterruptPendingSync)
                {
                    var value = _nmiInterruptPending;
                    _nmiInterruptPending = false;
                    return value;
                }
            }
            set
            {
                lock (nmiInterruptPendingSync)
                {
                    _nmiInterruptPending = value;
                }
            }
        }

        public IEnumerable<IZ80InterruptSource> GetRegisteredInterruptSources()
        {
            return [.. InterruptSources];
        }

        public void UnregisterAllInterruptSources()
        {
            foreach (var source in InterruptSources)
            {
                source.NmiInterruptPulse -= (sender, args) => NmiInterruptPending = true;
            }

            InterruptSources.Clear();
        }

        #endregion Inside and outside world

        #region Configuration

        private decimal effectiveClockFrequency;

        private decimal _ClockFrequencyInMHz = 1;

        public decimal ClockFrequencyInMHz
        {
            get => _ClockFrequencyInMHz;
            set
            {
                SetEffectiveClockFrequency(value, ClockSpeedFactor);
                _ClockFrequencyInMHz = value;
            }
        }

        private void SetEffectiveClockFrequency(decimal clockFrequency, decimal clockSpeedFactor)
        {
            var effectiveClockFrequency = clockFrequency * clockSpeedFactor;
            if ((effectiveClockFrequency > MaxEffectiveClockSpeed) ||
                (effectiveClockFrequency < MinEffectiveClockSpeed))
            {
                throw new ArgumentException(string.Format("Clock frequency multiplied by clock speed factor must be a number between {0} and {1}", MinEffectiveClockSpeed, MaxEffectiveClockSpeed));
            }

            this.effectiveClockFrequency = effectiveClockFrequency;
            if (clockSynchronizer != null)
                clockSynchronizer.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
        }

        private decimal _ClockSpeedFactor = 1;

        public decimal ClockSpeedFactor
        {
            get => _ClockSpeedFactor;
            set
            {
                SetEffectiveClockFrequency(ClockFrequencyInMHz, value);
                _ClockSpeedFactor = value;
            }
        }

        public bool AutoStopOnDiPlusHalt { get; set; }

        public bool AutoStopOnRetInstruction { get; set; }

        public bool AutoStopOnStackLimits { get; set; }

        public bool AutoStopOnRetWithStackEmpty { get; set; }

        public int AutoStopOnCycleCount { get; set; }

        private readonly byte[] memoryWaitStatesForM1 = new byte[MemorySpaceSize];

        public void SetMemoryWaitStatesForM1(ushort startAddress, int length, byte waitStates)
        {
            SetArrayContents(memoryWaitStatesForM1, startAddress, length, waitStates);
        }

        public byte GetMemoryWaitStatesForM1(ushort address)
        {
            return memoryWaitStatesForM1[address];
        }

        private readonly byte[] memoryWaitStatesForNonM1 = new byte[MemorySpaceSize];

        public void SetMemoryWaitStatesForNonM1(ushort startAddress, int length, byte waitStates)
        {
            SetArrayContents(memoryWaitStatesForNonM1, startAddress, length, waitStates);
        }

        public byte GetMemoryWaitStatesForNonM1(ushort address)
        {
            return memoryWaitStatesForNonM1[address];
        }

        private readonly byte[] portWaitStates = new byte[PortSpaceSize];

        public void SetPortWaitStates(ushort startPort, int length, byte waitStates)
        {
            SetArrayContents(portWaitStates, startPort, length, waitStates);
        }

        public byte GetPortWaitStates(byte portNumber)
        {
            return portWaitStates[portNumber];
        }

        private IZ80InstructionExecutor? _instructionExecutor;

        public IZ80InstructionExecutor InstructionExecutor
        {
            get => _instructionExecutor ?? throw new ArgumentNullException("InstructionExecutor not configured.");
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _instructionExecutor = value;
                _instructionExecutor.InitProcessorAgent(this);
                _instructionExecutor.InstructionFetchFinished += InstructionExecutor_InstructionFetchFinished;
            }
        }

        private IClockSynchronizer? clockSynchronizer;

        public IClockSynchronizer? ClockSynchronizer
        {
            get => clockSynchronizer;
            set
            {
                clockSynchronizer = value;
                if (clockSynchronizer == null)
                {
                    return;
                }
                else
                {
                    clockSynchronizer.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
                }
            }
        }

        #endregion Configuration

        #region Events

        public event EventHandler<MemoryAccessEventArgs>? MemoryAccess;

        public event EventHandler<BeforeInstructionFetchEventArgs>? BeforeInstructionFetch;

        public event EventHandler<BeforeInstructionExecutionEventArgs>? BeforeInstructionExecution;

        public event EventHandler<AfterInstructionExecutionEventArgs>? AfterInstructionExecution;

        #endregion Events

        #region Members of IZ80ProcessorAgent

        public byte FetchNextOpcode()
        {
            FailIfNoExecutionContext();

            if (executionContext.FetchComplete)
                throw new InvalidOperationException("FetchNextOpcode can be invoked only before the InstructionFetchFinished event has been raised.");

            byte opcode;
            if (executionContext.PeekedOpcode == null)
            {
                var address = Registers.PC;
                opcode = ReadFromMemoryOrPort(
                    address,
                    Memory,
                    GetMemoryAccessMode(address),
                    MemoryAccessEventType.BeforeMemoryRead,
                    MemoryAccessEventType.AfterMemoryRead,
                    GetMemoryWaitStatesForM1(address));
            }
            else
            {
                executionContext.AccummulatedMemoryWaitStates +=
                    GetMemoryWaitStatesForM1(executionContext.AddressOfPeekedOpcode);
                opcode = executionContext.PeekedOpcode.Value;
                executionContext.PeekedOpcode = null;
            }

            executionContext.OpcodeBytes.Add(opcode);
            Registers.PC++;
            return opcode;
        }

        public byte PeekNextOpcode()
        {
            FailIfNoExecutionContext();

            if (executionContext.FetchComplete)
                throw new InvalidOperationException("PeekNextOpcode can be invoked only before the InstructionFetchFinished event has been raised.");

            if (executionContext.PeekedOpcode == null)
            {
                var address = Registers.PC;
                var opcode = ReadFromMemoryOrPort(
                    address,
                    Memory,
                    GetMemoryAccessMode(address),
                    MemoryAccessEventType.BeforeMemoryRead,
                    MemoryAccessEventType.AfterMemoryRead,
                    waitStates: 0);

                executionContext.PeekedOpcode = opcode;
                executionContext.AddressOfPeekedOpcode = Registers.PC;
                return opcode;
            }
            else
            {
                return executionContext.PeekedOpcode.Value;
            }
        }

        private void FailIfNoExecutionContext()
        {
            if (executionContext == null)
                throw new InvalidOperationException("This method can be invoked only when an instruction is being executed.");
        }

        public byte ReadFromMemory(ushort address)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            return ReadFromMemoryInternal(address);
        }

        private byte ReadFromMemoryInternal(ushort address)
        {
            return ReadFromMemoryOrPort(
                address,
                Memory,
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryRead,
                MemoryAccessEventType.AfterMemoryRead,
                GetMemoryWaitStatesForNonM1(address));
        }

        protected virtual void FailIfNoInstructionFetchComplete()
        {
            if (executionContext != null && !executionContext.FetchComplete)
                throw new InvalidOperationException("IZ80ProcessorAgent members other than FetchNextOpcode can be invoked only after the InstructionFetchFinished event has been raised.");
        }

        private byte ReadFromMemoryOrPort(
            ushort address,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType,
            byte waitStates)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, 0xFF);

            byte value;
            if (!beforeEventArgs.CancelMemoryAccess &&
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.ReadOnly))
            {
                value = memory[address];
            }
            else
            {
                value = beforeEventArgs.Value;
            }

            if (executionContext != null)
                executionContext.AccummulatedMemoryWaitStates += waitStates;

            var afterEventArgs = FireMemoryAccessEvent(
                afterEventType,
                address,
                value,
                beforeEventArgs.LocalUserState,
                beforeEventArgs.CancelMemoryAccess);
            return afterEventArgs.Value;
        }

        private MemoryAccessEventArgs FireMemoryAccessEvent(
            MemoryAccessEventType eventType,
            ushort address,
            byte value,
            object? localUserState = null,
            bool cancelMemoryAccess = false)
        {
            var eventArgs = new MemoryAccessEventArgs(eventType, address, value, localUserState, cancelMemoryAccess);
            MemoryAccess?.Invoke(this, eventArgs);
            return eventArgs;
        }

        public void WriteToMemory(ushort address, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            WriteToMemoryInternal(address, value);
        }

        private void WriteToMemoryInternal(ushort address, byte value)
        {
            WritetoMemoryOrPort(
                address,
                value,
                Memory,
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryWrite,
                MemoryAccessEventType.AfterMemoryWrite,
                GetMemoryWaitStatesForNonM1(address));
        }

        private void WritetoMemoryOrPort(
            ushort address,
            byte value,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType,
            byte waitStates)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, value);

            if (!beforeEventArgs.CancelMemoryAccess &&
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.WriteOnly))
            {
                memory[address] = beforeEventArgs.Value;
            }

            if (executionContext != null)
                executionContext.AccummulatedMemoryWaitStates += waitStates;

            FireMemoryAccessEvent(
                afterEventType,
                address,
                beforeEventArgs.Value,
                beforeEventArgs.LocalUserState,
                beforeEventArgs.CancelMemoryAccess);
        }

        public byte ReadFromPort(byte portNumber)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            return ReadFromMemoryOrPort(
                portNumber,
                PortsSpace,
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortRead,
                MemoryAccessEventType.AfterPortRead,
                GetPortWaitStates(portNumber));
        }

        public void WriteToPort(byte portNumber, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            WritetoMemoryOrPort(
                portNumber,
                value,
                PortsSpace,
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortWrite,
                MemoryAccessEventType.AfterPortWrite,
                GetPortWaitStates(portNumber));
        }

        public void SetInterruptMode(byte interruptMode)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            InterruptMode = interruptMode;
        }

        public void Stop(bool isPause = false)
        {
            FailIfNoExecutionContext();

            if (!executionContext.ExecutingBeforeInstructionEvent)
                FailIfNoInstructionFetchComplete();

            executionContext.StopReason =
                isPause ?
                StopReason.PauseInvoked :
                StopReason.StopInvoked;
        }

        IZ80Registers IZ80ProcessorAgent.Registers => Registers;

        #endregion Members of IZ80ProcessorAgent

        #region Instruction execution context

        protected InstructionExecutionContext? executionContext;
        private IMemory memory;
        private IMemory portsSpace;
        private IZ80Registers registers;

        #endregion Instruction execution context
    }
}