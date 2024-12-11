using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class CompilerTests {

    readonly Compiler Compiler = new();

    [DataTestMethod]
    [DataRow("nop")]
    [DataRow("ld a, 0")]
    [DataRow("ld a, 0x12")]
    [DataRow("ld a, 0x12\nld a, 0\n")]
    [DataRow("imm_offset: equ 0x45\nADC A,(IX+imm_offset)\n")]
    public void CanGenerateLayout(string code)
    {
        Log.Reset();
        SourcePosition position = Compiler.ParseLiteral(code);
        Assert.IsNotNull(position);

        var result = Compiler.CalculateLayout();

        Assert.AreEqual(0, result);
        Log.DumpSummary();
    }

    [DataTestMethod]
    [DataRow("nop")]
    [DataRow("ld a, 0")]
    [DataRow("ld a, 0x12")]
    [DataRow("ld a, 0x12\nld a, 0\n")]
    [DataRow(@"
imm_word: equ 0xDADA
JP imm_word")]
    [DataRow("DJNZ $")] // Jump to defined location
    [DataRow(@"
ORG 0x8000
LOOP:	LD	A, (HL)		;Get next byte from input buffer
	    LD	(DE), A		;Store in output buffer
	    CP	$0D		    ;Is it a CR?
	    JR	Z, DONE		;Yes finished
	    INC	HL		    ;Increment pointers
	    INC	DE
	    DJNZ	LOOP	;Loop back if 80 bytes have not been moved.
        JP LOOP+1
DONE:")]
    [DataRow(@"
LOOP:	LD	A, (HL)		;Get next byte from input buffer
	    LD	(DE), A		;Store in output buffer
	    CP	$0D		    ;Is it a CR?
	    JR	Z, DONE		;Yes finished
	    INC	HL		    ;Increment pointers
	    INC	DE
	    DJNZ	LOOP	;Loop back if 80 bytes have not been moved.
DONE:")]
    [DataRow("imm_offset: equ 0x45\nADC A,(IX+imm_offset)\n")]
    public void CanGenerateCode(string code)
    {
        Log.Reset();
        SourcePosition position = Compiler.ParseLiteral(code);

        byte[]? codeBytes = null;
        Assert.IsNotNull(position);

        var result = Compiler.GenerateCode(
            position, 
            (s, newLine) =>
            {
                if (newLine)
                {
                    Console.WriteLine(s);
                }
                else
                {
                    Console.Write(s);
                }
            },
            (bytes) =>
            {
                Assert.IsNotNull(bytes);
                Assert.IsTrue(bytes.Length > 0);
                codeBytes = bytes;
            }
        );

        Assert.AreEqual(0, Log.ErrorCount);
        Assert.IsNotNull(codeBytes);

        // Dump the code bytes as hex, each line 16 bytes
        Utils.HexDump(codeBytes, 8, (o) => { o.ShowCrc = true; });

        Console.WriteLine();
        Console.WriteLine("# Log Summary");
        Log.DumpSummary();
    }
}
