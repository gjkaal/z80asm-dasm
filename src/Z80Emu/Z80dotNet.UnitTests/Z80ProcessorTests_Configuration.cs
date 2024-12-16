﻿using AutoFixture;
using Konamiman.Z80dotNet.Enums;
using Moq;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Configuration
    {
        private const int MemorySpaceSize = 65536;
        private const int PortSpaceSize = 256;

        private Z80ProcessorForTests Sut { get; set; }
        private Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.That(Sut, Is.Not.Null);
        }

        [Test]
        public void Has_proper_defaults()
        {
            Assert.That(Sut.ClockFrequencyInMHz, Is.EqualTo(4));
            Assert.That(Sut.ClockSpeedFactor, Is.EqualTo(1));

            Assert.That(Sut.AutoStopOnDiPlusHalt);
            Assert.That(Sut.AutoStopOnRetWithStackEmpty, Is.Not.True);
            Assert.That(Sut.Registers.StartOfStack, Is.EqualTo(0xFFFF.ToShort()));

            Assert.That(Sut.Memory, Is.InstanceOf<PlainMemory>());
            Assert.That(Sut.Memory.Size, Is.EqualTo(65536));
            Assert.That(Sut.PortsSpace, Is.InstanceOf<PlainMemory>());
            Assert.That(Sut.PortsSpace.Size, Is.EqualTo(256));

            for (var i = 0; i < 65536; i++)
            {
                Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetMemoryAccessMode((ushort)i)));
                Assert.That(Sut.GetMemoryWaitStatesForM1((ushort)i), Is.EqualTo(0));
                Assert.That(Sut.GetMemoryWaitStatesForNonM1((ushort)i), Is.EqualTo(0));
            }
            for (var i = 0; i < 256; i++)
            {
                Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetPortAccessMode((byte)i)));
                Assert.That(Sut.GetPortWaitStates((byte)i), Is.EqualTo(0));
            }

            Assert.That(Sut.Registers, Is.InstanceOf<Z80Registers>());

            Assert.That(Sut.InstructionExecutor, Is.InstanceOf<IZ80InstructionExecutor>());
            Assert.That(Sut, Is.SameAs(Sut.InstructionExecutor.ProcessorAgent));
            Assert.That(Sut.ClockSynchronizer, Is.InstanceOf<ClockSynchronizer>());

            Assert.That(StopReason.NeverRan, Is.EqualTo(Sut.StopReason));
            Assert.That(ProcessorState.Stopped, Is.EqualTo(Sut.State));
            Assert.That(Sut.IsHalted, Is.False);
            Assert.That(Sut.UserState, Is.Null);
        }

        [Test]
        public void Reset_sets_registers_properly()
        {
            Sut.Registers.IFF1 = 1;
            Sut.Registers.IFF1 = 1;
            Sut.Registers.PC = 1;
            Sut.Registers.AF = 0;
            Sut.Registers.InitializeSP(0);
            Sut.InterruptMode = 1;
            Sut.SetIsHalted();

            Sut.Reset();

            Assert.That(Sut.Registers.AF == 0xFFFF.ToShort());
            Assert.That(Sut.Registers.SP, Is.EqualTo(0xFFFF.ToShort()));
            Assert.That(Sut.Registers.PC, Is.EqualTo(0));
            Assert.That(Sut.Registers.IFF1 == 0);
            Assert.That(Sut.Registers.IFF2 == 0);
            Assert.That(Sut.InterruptMode, Is.EqualTo(0));

            Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(0));

            Assert.That(Sut.IsHalted, Is.False);
        }

        [Test]
        public void Interrupt_mode_can_be_set_to_0_1_or_2()
        {
            Sut.InterruptMode = 0;
            Assert.That(0, Is.EqualTo(Sut.InterruptMode));

            Sut.InterruptMode = 1;
            Assert.That(1, Is.EqualTo(Sut.InterruptMode));

            Sut.InterruptMode = 2;
            Assert.That(2, Is.EqualTo(Sut.InterruptMode));
        }

        [Test]
        public void Interrupt_mode_cannot_be_set_to_higher_than_2()
        {
            Assert.Throws<ArgumentException>(() => Sut.InterruptMode = 3);
        }

        [Test]
        public void SetMemoryAccessMode_and_GetMemoryAccessMode_are_consistent()
        {
            Sut.SetMemoryAccessMode(0, 0x4000, MemoryAccessMode.NotConnected);
            Sut.SetMemoryAccessMode(0x4000, 0x4000, MemoryAccessMode.ReadAndWrite);
            Sut.SetMemoryAccessMode(0x8000, 0x4000, MemoryAccessMode.ReadOnly);
            Sut.SetMemoryAccessMode(0xC000, 0x4000, MemoryAccessMode.WriteOnly);

            Assert.That(MemoryAccessMode.NotConnected, Is.EqualTo(Sut.GetMemoryAccessMode(0)));
            Assert.That(MemoryAccessMode.NotConnected, Is.EqualTo(Sut.GetMemoryAccessMode(0x3FFF)));
            Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetMemoryAccessMode(0x4000)));
            Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetMemoryAccessMode(0x7FFF)));
            Assert.That(MemoryAccessMode.ReadOnly, Is.EqualTo(Sut.GetMemoryAccessMode(0x8000)));
            Assert.That(MemoryAccessMode.ReadOnly, Is.EqualTo(Sut.GetMemoryAccessMode(0xBFFF)));
            Assert.That(MemoryAccessMode.WriteOnly, Is.EqualTo(Sut.GetMemoryAccessMode(0xC000)));
            Assert.That(MemoryAccessMode.WriteOnly, Is.EqualTo(Sut.GetMemoryAccessMode(0xFFFF)));
        }

        [Test]
        public void SetMemoryAccessMode_works_when_address_plus_length_are_on_memory_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Sut.SetMemoryAccessMode((ushort)(MemorySpaceSize - length), length, MemoryAccessMode.NotConnected);
        }

        [Test]
        public void SetMemoryAccessMode_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(() => Sut.SetMemoryAccessMode((ushort)(MemorySpaceSize - length), length + 1, MemoryAccessMode.NotConnected));
        }

        [Test]
        public void SetMemoryAccessMode_fails_when_length_is_negative()
        {
            var value = Fixture.Create<MemoryAccessMode>();

            Assert.Throws<ArgumentException>(() => Sut.SetMemoryAccessMode(0, -1, value));
        }

        [Test]
        public void SetPortsSpaceAccessMode_and_GetPortsSpaceAccessMode_are_consistent()
        {
            Sut.SetPortsSpaceAccessMode(0, 64, MemoryAccessMode.NotConnected);
            Sut.SetPortsSpaceAccessMode(64, 64, MemoryAccessMode.ReadAndWrite);
            Sut.SetPortsSpaceAccessMode(128, 64, MemoryAccessMode.ReadOnly);
            Sut.SetPortsSpaceAccessMode(192, 64, MemoryAccessMode.WriteOnly);

            Assert.That(MemoryAccessMode.NotConnected, Is.EqualTo(Sut.GetPortAccessMode(0)));
            Assert.That(MemoryAccessMode.NotConnected, Is.EqualTo(Sut.GetPortAccessMode(63)));
            Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetPortAccessMode(64)));
            Assert.That(MemoryAccessMode.ReadAndWrite, Is.EqualTo(Sut.GetPortAccessMode(127)));
            Assert.That(MemoryAccessMode.ReadOnly, Is.EqualTo(Sut.GetPortAccessMode(128)));
            Assert.That(MemoryAccessMode.ReadOnly, Is.EqualTo(Sut.GetPortAccessMode(191)));
            Assert.That(MemoryAccessMode.WriteOnly, Is.EqualTo(Sut.GetPortAccessMode(192)));
            Assert.That(MemoryAccessMode.WriteOnly, Is.EqualTo(Sut.GetPortAccessMode(255)));
        }

        [Test]
        public void SetPortsAccessMode_works_when_address_plus_length_are_on_ports_space_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Sut.SetPortsSpaceAccessMode((byte)(PortSpaceSize - length), length, MemoryAccessMode.NotConnected);
        }

        [Test]
        public void SetPortsAccessMode_fails_when_address_plus_length_are_beyond_ports_space_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(() => Sut.SetPortsSpaceAccessMode((byte)(PortSpaceSize - length), length + 1, MemoryAccessMode.NotConnected));
        }

        [Test]
        public void SetPortsSpaceAccessMode_fails_when_length_is_negative()
        {
            var value = Fixture.Create<MemoryAccessMode>();

            Assert.Throws<ArgumentException>(() => Sut.SetPortsSpaceAccessMode(0, -1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForM1_and_GetMemoryWaitStatesForM1_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForM1(0, 0x8000, value1);
            Sut.SetMemoryWaitStatesForM1(0x8000, 0x8000, value2);

            Assert.That(value1, Is.EqualTo(Sut.GetMemoryWaitStatesForM1(0)));
            Assert.That(value1, Is.EqualTo(Sut.GetMemoryWaitStatesForM1(0x7FFF)));
            Assert.That(value2, Is.EqualTo(Sut.GetMemoryWaitStatesForM1(0x8000)));
            Assert.That(value2, Is.EqualTo(Sut.GetMemoryWaitStatesForM1(0xFFFF)));
        }

        [Test]
        public void SetMemoryWaitStatesForM1_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForM1((ushort)(MemorySpaceSize - length), length, value);
        }

        [Test]
        public void SetMemoryWaitStatesForM1_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForM1((ushort)(MemorySpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForM1_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForM1(0, -1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_and_GetMemoryWaitStatesForNonM1_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForNonM1(0, 0x8000, value1);
            Sut.SetMemoryWaitStatesForNonM1(0x8000, 0x8000, value2);

            Assert.That(value1, Is.EqualTo(Sut.GetMemoryWaitStatesForNonM1(0)));
            Assert.That(value1, Is.EqualTo(Sut.GetMemoryWaitStatesForNonM1(0x7FFF)));
            Assert.That(value2, Is.EqualTo(Sut.GetMemoryWaitStatesForNonM1(0x8000)));
            Assert.That(value2, Is.EqualTo(Sut.GetMemoryWaitStatesForNonM1(0xFFFF)));
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForNonM1((ushort)(MemorySpaceSize - length), length, value);
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForNonM1((ushort)(MemorySpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForNonM1(0, -1, value));
        }

        [Test]
        public void SetPortWaitStates_and_GetPortWaitStates_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetPortWaitStates(0, 128, value1);
            Sut.SetPortWaitStates(128, 128, value2);

            Assert.That(value1, Is.EqualTo(Sut.GetPortWaitStates(0)));
            Assert.That(value1, Is.EqualTo(Sut.GetPortWaitStates(127)));
            Assert.That(value2, Is.EqualTo(Sut.GetPortWaitStates(128)));
            Assert.That(value2, Is.EqualTo(Sut.GetPortWaitStates(255)));
        }

        [Test]
        public void SetPortWaitStates_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetPortWaitStates((ushort)(PortSpaceSize - length), length, value);
        }

        [Test]
        public void SetPortWaitStates_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetPortWaitStates((ushort)(PortSpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetPortWaitStates_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetPortWaitStates(0, -1, value));
        }

        [Test]
        public void Can_set_Memory_to_non_null_value()
        {
            var value = new Mock<IMemory>().Object;
            Sut.Memory = value;
            Assert.That(value, Is.EqualTo(Sut.Memory));
        }

        [Test]
        public void Cannot_set_Memory_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.Memory = null);
        }

        [Test]
        public void Can_set_Registers_to_non_null_value()
        {
            var value = new Mock<IZ80Registers>().Object;
            Sut.Registers = value;
            Assert.That(value, Is.EqualTo(Sut.Registers));
        }

        [Test]
        public void Cannot_set_Registers_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.Registers = null);
        }

        [Test]
        public void Can_set_PortsSpace_to_non_null_value()
        {
            var value = new Mock<IMemory>().Object;
            Sut.PortsSpace = value;
            Assert.That(value, Is.EqualTo(Sut.PortsSpace));
        }

        [Test]
        public void Cannot_set_PortsSpace_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.PortsSpace = null);
        }

        [Test]
        public void Can_set_InstructionExecutor_to_non_null_value()
        {
            var value = new Mock<IZ80InstructionExecutor>().Object;
            Sut.InstructionExecutor = value;
            Assert.That(value, Is.EqualTo(Sut.InstructionExecutor));
        }

        [Test]
        public void Cannot_set_InstructionExecutor_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.InstructionExecutor = null);
        }

        [Test]
        public void Sets_InstructionExecutor_agent_to_self()
        {
            IZ80ProcessorAgent? agent = null;
            var mock = new Mock<IZ80InstructionExecutor>();
            mock.Setup(m => m.InitProcessorAgent(It.IsAny<IZ80ProcessorAgent>()))
                .Callback<IZ80ProcessorAgent>(s => { agent = s; });
            Sut.InstructionExecutor = mock.Object;

            Assert.That(agent, Is.Not.Null);
        }

        [Test]
        public void Can_set_ClockSynchronizationHelper_to_non_null_value()
        {
            var value = new Mock<IClockSynchronizer>().Object;
            Sut.ClockSynchronizer = value;
            Assert.That(value, Is.EqualTo(Sut.ClockSynchronizer));
        }

        [Test]
        public void Can_set_ClockSynchronizationHelper_to_null()
        {
            Sut.ClockSynchronizer = null;
        }

        [Test]
        public void Sets_ClockSynchronizationHelper_clockSpeed_to_processor_speed_by_speed_factor()
        {
            var mock = new Mock<IClockSynchronizer>();
            Sut.ClockFrequencyInMHz = 2;
            Sut.ClockSpeedFactor = 3;
            Sut.ClockSynchronizer = mock.Object;

            mock.VerifySet(m => m.EffectiveClockFrequencyInMHz = 2 * 3);
        }

        [Test]
        public void Can_set_clock_speed_and_clock_factor_combination_up_to_100_MHz()
        {
            Sut.ClockFrequencyInMHz = 20;
            Sut.ClockSpeedFactor = 5;

            Assert.That(20, Is.EqualTo(Sut.ClockFrequencyInMHz));
            Assert.That(5, Is.EqualTo(Sut.ClockSpeedFactor));
        }

        [Test]
        public void Cannot_set_clock_speed_and_clock_factor_combination_over_100_MHz()
        {
            Sut.ClockFrequencyInMHz = 1;
            Sut.ClockSpeedFactor = 1;

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockFrequencyInMHz = 1;
                Sut.ClockSpeedFactor = 101;
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockSpeedFactor = 1;
                Sut.ClockFrequencyInMHz = 101;
            });
        }

        [Test]
        public void Can_set_clock_speed_and_clock_factor_combination_down_to_1_KHz()
        {
            Sut.ClockFrequencyInMHz = 0.1M;
            Sut.ClockSpeedFactor = 0.01M;

            Assert.That(0.1M, Is.EqualTo(Sut.ClockFrequencyInMHz));
            Assert.That(0.01M, Is.EqualTo(Sut.ClockSpeedFactor));
        }

        [Test]
        public void Cannot_set_clock_speed_and_clock_factor_combination_under_1_KHz()
        {
            Sut.ClockFrequencyInMHz = 1;
            Sut.ClockSpeedFactor = 1;

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockFrequencyInMHz = 1;
                Sut.ClockSpeedFactor = 0.0009M;
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockSpeedFactor = 0.0009M;
                Sut.ClockFrequencyInMHz = 1;
            });
        }
    }
}
