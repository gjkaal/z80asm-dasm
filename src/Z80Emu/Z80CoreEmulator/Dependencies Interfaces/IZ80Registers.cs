namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a full set of Z80 registers.
    /// </summary>
    public interface IZ80Registers : IMainZ80Registers
    {
        /// <summary>
        /// The alternate register set (AF', BC', DE', HL')
        /// </summary>
        IMainZ80Registers Alternate { get; set; }

        /// <summary>
        /// The IX register pair
        /// </summary>
        short IX { get; set; }

        /// <summary>
        /// The IY register pair
        /// </summary>
        short IY { get; set; }

        /// <summary>
        /// The program counter
        /// </summary>
        ushort PC { get; set; }

        /// <summary>
        /// The stack pointer. The Z80 stack pointer is a 16-bit register, but the stack is stored in the memory
        /// The SP is decresed at a PUSH and increased at a POP.
        /// </summary>
        short SP { get; }

        /// <summary>
        /// The initial value of the stack pointer for a setup or test. If <see cref="AutoStopOnDiPlusHalt"/> is <b>true</b>,
        /// execution will stop automatically when a <c>RET</c> instruction is executed with the SP register
        /// having this value.
        /// </summary>
        /// <remarks>
        /// This property is set to 0xFFFF when the class is first instantiated and when the <see cref="Start"/>
        /// and <see cref="Reset"/> methods are executed. Also, when a <c>LD SP,x</c> instruction is executed,
        /// this property is set to the new value of SP.
        /// </remarks>
        void InitializeSP(short value);

        /// <summary>
        /// The initial value of the stack pointer for a setup or test.
        /// </summary>
        short StartOfStack { get; }

        /// <summary>
        /// Sets the stack pointer to a specific value. This is used by the CALL instruction.
        /// It does not change the initial value of the stack pointer and does not affect the stack limit.
        /// </summary>
        void SetSpFromInstruction(short value);

        /// <summary>
        /// Increases the stack pointer by 2. (This is used by the POP instruction.)
        /// If <see cref="ThrowOnStackUnderflow"/> is set, the Z80 will throw an exception if the SP is greater than the stack initialization.
        /// </summary>
        void IncSp();

        /// <summary>
        /// Decreases the stack pointer by 2. (This is used by the PUSH instruction.)
        /// If <see cref="SPLowerLimit"/> is set, the Z80 will throw an exception if the SP is less than this limit.
        /// </summary>
        void DecSp();

        /// <summary>
        /// If the SP is empty (i.e. pop is called when the stack is empty), the Z80 will throw an exception.)
        /// </summary>
        bool ThrowOnStackUnderflow { get; set; }
        bool StackUnderflow { get; }

        /// <summary>
        /// If the SP is full (i.e. push is called when the stack is full), the Z80 will throw an exception.
        /// The full stack is only detected if the <see cref="SPLowerLimit"/> is set to a value other than zero.
        /// </summary>
        bool ThrowOnStackOverflow { get; set; }
        bool StackOverflow { get; }

        /// <summary>
        /// If this value is set to another value as zero, the Z80 will throw an exception when the stack pointer reaches this address.
        /// (i.e. push when the stack is full)
        /// </summary>
        short SPLowerLimit { get; set; }

        /// <summary>
        /// The interrupt and refresh register
        /// </summary>
        short IR { get; set; }

        /// <summary>
        /// The IFF1 flag. It has always the value 0 or 1.
        /// </summary>
        Bit IFF1 { get; set; }

        /// <summary>
        /// The IFF2 flag. It has always the value 0 or 1.
        /// </summary>
        Bit IFF2 { get; set; }

        /// <summary>
        /// The IXH register.
        /// </summary>
        byte IXH { get; set; }

        /// <summary>
        /// The IXL register.
        /// </summary>
        byte IXL { get; set; }

        /// <summary>
        /// The IYH register.
        /// </summary>
        byte IYH { get; set; }

        /// <summary>
        /// The IYL register.
        /// </summary>
        byte IYL { get; set; }

        /// <summary>
        /// The I register.
        /// </summary>
        byte I { get; set; }

        /// <summary>
        /// The R register.
        /// </summary>
        byte R { get; set; }
    }
}