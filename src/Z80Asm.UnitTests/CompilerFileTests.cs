using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class CompilerFileTests
{
    // Read the contents of a file and compile it
    // The file resides in the TestResources directory
    [DataTestMethod]
    [DataRow("allInstructions.asm")]
    public void CanCompileFile(string fileName)
    {
        Log.Reset();
        var compiler = new Compiler();
        var path = Path.Combine("TestResources", fileName);
        var source = new FileInfo(path);
        Assert.IsTrue(source.Exists);
        compiler.CreateSyntaxTreeFile = true;
        compiler.CreateListFile = true;
        compiler.CreateBinaryFile = true;
        compiler.CreateIntelHexFile = true;

        var result = compiler.Compile(source);

        Assert.AreEqual(0, result);
        Log.DumpSummary();
    }
}
