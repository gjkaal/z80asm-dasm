namespace Z80Asm
{
    /// <summary>
    /// In assembler or compiler applications, Ip (Instruction Pointer) represents the current position in
    /// the instruction set being processed, while Op (Output Pointer) represents the position in the
    /// generated machine code or intermediate representation. The methods allow setting initial positions
    /// and reserving space as instructions are converted.
    /// </summary>
    public class LayoutContext
    {
        public int Ip { get; private set; }
        public int Op { get; private set; }

        /// <summary>
        /// Sets the starting position of the instruction pointer.
        /// </summary>
        /// <param name="value">address for Input Pointer</param>
        public void SetOrg(int value)
        {
            Ip = value;
        }

        /// <summary>
        /// Sets the position of the output pointer.
        /// </summary>
        /// <param name="value">address for Output Pointer</param>
        public void Seek(int value)
        {
            Op = value;
        }

        /// <summary>
        ///  Increments both pointers, simulating the allocation of space for instructions or data.
        /// </summary>
        /// <param name="amount">number of bytes to reserve.</param>
        public void ReserveBytes(int amount)
        {
            Ip += amount;
            Op += amount;
        }
    }
}