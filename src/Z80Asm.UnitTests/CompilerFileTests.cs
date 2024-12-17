using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class CompilerFileTests
{
    // Read the contents of a file and compile it
    // The file resides in the TestResources directory
    [DataTestMethod]
    [DataRow("allInstructions.asm", 1)]
    [DataRow("zc-bootstrap.asm", 4)]
    [DataRow("macro.asm", 0)]
    [DataRow("stack-underflow.asm", 1)]
    [DataRow("strings.asm", 1)]
    [DataRow("rewriting-code.asm", 1)]
    public void CanCompileFile(string fileName, int expectedCodeBlocks)
    {
        Log.Reset();
        var compiler = new Z80Assembler();
        var path = Path.Combine("TestResources", fileName);
        var source = new FileInfo(path);
        Assert.IsTrue(source.Exists);
        compiler.CreateSyntaxTreeFile = true;
        compiler.CreateListFile = true;
        compiler.CreateBinaryFile = true;
        compiler.CreateIntelHexFile = true;

        var (errorCount, warningCount) = compiler.Compile(source);

        var items = compiler.ContentBlocks;
        Assert.AreEqual(expectedCodeBlocks, items.Count());

        Assert.AreEqual(0, errorCount);
        Assert.AreEqual(0, warningCount);
        Log.DumpSummary();
    }

    [DataTestMethod]
    [DataRow("overwrite-error.asm")]
    public void ThrowsExceptionWhenCreatingBinary(string fileName)
    {
        Log.Reset();
        var compiler = new Z80Assembler();
        var path = Path.Combine("TestResources", fileName);
        var source = new FileInfo(path);
        Assert.IsTrue(source.Exists);
        compiler.CreateSyntaxTreeFile = true;
        compiler.CreateListFile = true;
        compiler.CreateBinaryFile = true;
        compiler.CreateIntelHexFile = true;

        Assert.ThrowsException<InvalidDataException>(() => compiler.Compile(source));

        Log.DumpSummary();
    }
}
