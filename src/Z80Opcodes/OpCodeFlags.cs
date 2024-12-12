namespace Z80Mnemonics;

[Flags]
public enum OpCodeFlags
{
    None = 0x0000,      // No flags
    Continues = 0x0001, // Instruction may continue at the next address
    Jumps = 0x0002,     // Instruction jumps to an absolute or relative address
    Returns = 0x0004,   // Instruction returns
    Restarts = 0x0008,  // Instruction jumps to restart address
    RefAddr = 0x0010,   // References a literal address
    PortRef = 0x0020,   // IN or OUT instruction
    Call = 0x0040,
    ImplicitA = 0x0080, // Implicit accumulator register eg: ADD B
    NoAsm = 0x0100,     // Ignore this instruction in the assembler
}
