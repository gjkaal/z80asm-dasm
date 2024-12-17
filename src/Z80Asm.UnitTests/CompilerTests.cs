using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class CompilerTests
{
    private readonly Z80Assembler Compiler = new();

    [DataTestMethod]
    [DataRow("nop")]
    [DataRow("ld a, 0")]
    [DataRow("ld a, 0x12")]
    [DataRow("ld a, 0x12\nld a, 0\n")]
    [DataRow("imm_offset: equ 0x45\nADC A,(IX+imm_offset)\n")]
    public void CanGenerateLayout(string code)
    {
        Log.Reset();
        var position = Compiler.ParseLiteral(code);
        Assert.IsNotNull(position);

        var result = Compiler.CalculateLayout();

        Assert.AreEqual(0, result);
        Log.DumpSummary();
    }

    [DataTestMethod]
    [DataRow("pop A")]
    public void ShouldShowWarning(string code)
    {
        Log.Reset();
        var position = Compiler.ParseLiteral(code);

        byte[]? codeBytes = null;
        Contentblock[]? contentBlocks = null;
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
            (bytes, content) =>
            {
                codeBytes = bytes;
                contentBlocks = content.ToArray();
            }
        );

        Assert.AreNotEqual(0, Log.WarningCount);
        Assert.IsNotNull(codeBytes);
        Assert.IsNotNull(contentBlocks);
        Assert.AreEqual(0, codeBytes.Length);
        Assert.AreEqual(0, contentBlocks.Length);

        Console.WriteLine();
        Console.WriteLine("# Log Summary");
        Log.DumpSummary();
    }

    [DataTestMethod]
    [DataRow("M_SLA MACRO\r\n    rla\r\n    rla\r\n    rla\r\n    rla\r\n    and 0xF0\r\nENDM")]
    [DataRow("imm_offset: equ 0x45")]
    public void ShouldNotGenerateCode(string code)
    {
        Log.Reset();
        var position = Compiler.ParseLiteral(code);

        byte[]? codeBytes = null;
        Contentblock[]? contentBlocks = null;
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
            (bytes, content) =>
            {
                codeBytes = bytes;
                contentBlocks = content.ToArray();
            }
        );

        Assert.AreEqual(0, Log.ErrorCount);
        Assert.IsNotNull(codeBytes);
        Assert.IsNotNull(contentBlocks);
        Assert.IsTrue(codeBytes.Length == 0);
        Assert.IsTrue(contentBlocks.Length == 0);

        Console.WriteLine();
        Console.WriteLine("# Log Summary");
        Log.DumpSummary();
    }

    [DataTestMethod]
    [DataRow("nop")]
    [DataRow("pop AF")]
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
        var position = Compiler.ParseLiteral(code);

        byte[]? codeBytes = null;
        Contentblock[]? contentBlocks = null;
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
            (bytes, content) =>
            {
                codeBytes = bytes;
                contentBlocks = content.ToArray();
            }
        );

        Assert.AreEqual(0, Log.ErrorCount);
        Assert.IsNotNull(codeBytes);
        Assert.IsNotNull(contentBlocks);
        Assert.IsTrue(codeBytes.Length > 0);
        Assert.IsTrue(contentBlocks.Length > 0);

        // Dump the code bytes as hex, each line 16 bytes
        Utils.HexDump(codeBytes, 8, (o) => { o.ShowCrc = true; });

        Console.WriteLine();
        Console.WriteLine("# Log Summary");
        Log.DumpSummary();
    }
}
