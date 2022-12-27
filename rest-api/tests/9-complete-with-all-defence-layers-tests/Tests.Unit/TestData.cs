namespace Test.Unit.Token.Domain;

public static class TestData
{
    public static IEnumerable<object[]> InjectionStrings => 
        File
            .ReadAllLines("Tests.Unit/blns-injection.txt") // A selection from https://github.com/minimaxir/big-list-of-naughty-strings
            .Select(item => new object[] { item });
    
    public static IEnumerable<object[]> StrangeNames => 
        File
            .ReadAllLines("Tests.Unit/blns-names.txt") // A selection from https://github.com/minimaxir/big-list-of-naughty-strings
            .Select(item => new object[] { item });
}