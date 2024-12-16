namespace Konamiman.Z80dotNet.Z80EventArgs
{
    /// <summary>
    /// Event triggered by the <see cref="IZ80Processor"/> class before an instruction is executed.
    /// </summary>
    public class BeforeInstructionExecutionEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="opcode">The full opcode bytes of the instruction that is about to be executed.</param>
        public BeforeInstructionExecutionEventArgs(byte[] opcode, object? localUserState)
        {
            Opcode = opcode;
            LocalUserState = localUserState;
        }

        /// <summary>
        /// Contains the full opcode bytes of the instruction that is about to be executed.
        /// </summary>
        public byte[] Opcode { get; set; }
    }
}