using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class ParserTests
{
    [TestMethod]
    public void Parser_Can_Initialize()
    {
        var parser = new Parser();
        Assert.IsNotNull(parser);
    }

    [TestMethod]
    public void Parser_Can_Parse_Empty()
    {
        var parser = new Parser();
        var result = parser.Parse("Test", "");
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Name);

        // Top level is not in a container
        Assert.IsNull(result.Container);

        var resultText = new StringBuilder();
        result.Dump((s) => resultText.AppendLine(s), 0);
        Console.WriteLine(resultText.ToString());
    }

    [DataTestMethod]
    [DataRow("nop")]
    [DataRow("ld a, 0")]
    [DataRow("ld a, 0x12")]
    [DataRow("imm_offset: equ 0x45")]
    public void Parser_Can_Parse(string input)
    {
        var parser = new Parser();
        var result = parser.Parse("Test", input);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Name);

        var resultText = new StringBuilder();
        result.Dump((s) => resultText.AppendLine(s), 0);
        Console.WriteLine(resultText.ToString());
    }

    [DataTestMethod]
    [DataRow("imm_offset: equ 0x45")]
    public void Parser_Can_Parse_Definitions(string input)
    {
        var parser = new Parser();
        var result = parser.Parse("Test", input);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Name);

        var resultText = new StringBuilder();
        result.Dump((s) => resultText.AppendLine(s), 0);
        Console.WriteLine(resultText.ToString());
    }
}
