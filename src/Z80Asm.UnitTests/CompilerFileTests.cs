using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class CompilerFileTests
{
    // Read the contents of a file and compile it
    // The file resides in the TestResources directory
    [DataTestMethod]
    [DataRow("allInstructions.asm")]
    [DataRow("zc-bootstrap.asm")]
    [DataRow("macro.asm")]
    [DataRow("stack-underflow.asm")]
    [DataRow("strings.asm")]
    [DataRow("rewriting-code.asm")]
    public void CanCompileFile(string fileName)
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
