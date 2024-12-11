using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Z80Asm.UnitTests;

[TestClass]
public sealed class ScopeTests
{
    readonly SourcePosition testSource = new SourcePosition(
        new StringSource(string.Empty,
            "ScopeTests", "init"), 0);

    [TestMethod]
    public void Scope_Can_Initialize()
    {
        var scope = new AstScope("global", testSource);
        Assert.IsNotNull(scope);
    }

    [TestMethod]
    public void Scope_Can_Add_Symbols()
    {
        var scope = new AstScope("global", testSource);
        var exprNodeIP = new ExprNodeIP(true);
        scope.Define("$", exprNodeIP);

        Assert.IsTrue(scope.IsSymbolDefined("$"));
        Assert.AreEqual(exprNodeIP, scope.FindSymbol("$"));
    }

    [TestMethod]
    public void Scope_Can_Add_Types()
    {
        var scope = new AstScope("global", testSource);
        var byteType = new AstTypeByte(testSource);
        scope.AddElement(byteType);

        Assert.IsTrue(scope.ElementExist(byteType.Id));
    }

    [TestMethod]
    public void Scope_Can_DefineSymbols()
    {
        var scope = new AstScope("global", testSource);
        var exprNodeIP = new ExprNodeIP(true);
        scope.Define("$", exprNodeIP);
        var byteType = new AstTypeByte(testSource);
        scope.AddElement(byteType);
        var symbols = scope.DefineSymbols();
        Assert.IsTrue(symbols > 0);
    }

    [TestMethod]
    public void Scope_Can_AddUserDefines()
    {
        var scope = new AstScope("global", testSource);

        var userDefines = new Dictionary<string, string?> {
            { "TEST", null },
            { "DEAD", "&Hdead" },
            { "ZERO", "0" }
        };
        scope.AddUserDefines(userDefines);

        Assert.IsTrue(scope.IsSymbolDefined("TEST"));
        Assert.IsTrue(scope.IsSymbolDefined("DEAD"));
        Assert.IsTrue(scope.IsSymbolDefined("ZERO"));

        var lookup = scope.FindSymbol("TEST");
        Assert.IsNotNull(lookup);
        var literal = lookup as ExprNodeNumberLiteral;
        Assert.IsNotNull(literal);
        Assert.AreEqual(1, literal.GetImmediateValue(scope));

        var zero = (scope.FindSymbol("ZERO") as ExprNodeNumberLiteral)!
            .GetImmediateValue(scope);
        Assert.AreEqual(0, zero);

        var dead = (scope.FindSymbol("DEAD") as ExprNodeNumberLiteral)!
            .GetImmediateValue(scope);
        Assert.AreEqual(0xDEAD, dead);
    }
}
