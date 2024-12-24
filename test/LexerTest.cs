using SAC9.Lexer;

namespace test;
// Create a test class for the Lexer
public class LexerTest
{
    // Helper function to compare expected and found lexemes
    void check(List<Lexeme> expect, List<Lexeme> found)
    {
        // Check if the number of expected and found tokens match
        Assert.True(
            expect.Count == found.Count,
            $"expected size: {expect.Count} || found size{found.Count}:: {found[0]}");

        // Compare each lexeme in the expected and found lists
        for (int i = 0; i < found.Count; i++)
        {
            Assert.True(
                expect[i] == found[i],
                $"error in {i}'th token: expected {expect[i]} but found {found[i]} \n '{found[i].value}'  ");
        }
    }

    // Test for identifying valid identifiers (e.g., variable names)
    [Test]
    public void Ident()
    {
        // Test for valid identifiers
        Assert.True(Lexer.isIdent("ا"));
        Assert.True(Lexer.isIdent("ضصثقفغعهخحجدشسيبلاتنمكطئءؤرلاىةوزظذ"));
        Assert.True(Lexer.isIdent("ا1"));

        // Test for invalid identifiers
        Assert.False(Lexer.isIdent("1ا"));
        Assert.False(Lexer.isIdent(" "));
        Assert.False(Lexer.isIdent("\n"));
        Assert.False(Lexer.isIdent("احا\n"));
    }

    // Test for detecting valid numbers
    [Test]
    public void Number()
    {
        // Valid numbers
        Assert.True(Lexer.isNumber("1.23"));
        Assert.True(Lexer.isNumber("23"));

        // Invalid number formats
        Assert.False(Lexer.isNumber("1..2"));
        Assert.False(Lexer.isNumber("a2.3"));
        Assert.False(Lexer.isNumber("2.3a"));
        Assert.False(Lexer.isNumber(""));
        Assert.False(Lexer.isNumber("4\n"));
        Console.WriteLine("great number");
    }

    // Test reserved words like 'if', 'else', etc.
    [Test]
    public void ReservedWords()
    {
        Assert.True(Lexer.ResWord("خالى") == TokenType.Void_, "error in void");
        Assert.True(Lexer.ResWord("خالي") == TokenType.Void_, "error in void");
        Assert.True(Lexer.ResWord("اذا") == TokenType.if_, "error in if");
        Assert.True(Lexer.ResWord("اخر") == TokenType.else_, "error in else");
        Assert.True(Lexer.ResWord("بينما") == TokenType.while_, "error in while");
        Assert.True(Lexer.ResWord("ارجع") == TokenType.return_, "error in return ");
        Assert.True(Lexer.ResWord("حقيقي") == TokenType.real_, "error in real");
        Assert.True(Lexer.ResWord("صحيح") == TokenType.num_, "error in int");
        Assert.True(Lexer.ResWord("احا") == TokenType.Ident, "error in a7a");
    }

    // Test the scanning of a single line of code into lexemes
    [Test]
    public void scan1()
    {
        string input = "صحيح رقم1 = 5;";

        // Expected lexemes for this input
        List<Lexeme> expect = new List<Lexeme>() {
            new Lexeme() { type = TokenType.num_, value = "صحيح", line = 0, column = 0 },
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 0, column = 5 },
            new Lexeme() { type = TokenType.equal_, value = "=", line = 0, column = 10 },
            new Lexeme() { type = TokenType.Number, value = "5", line = 0, column = 12 },
            new Lexeme() { type = TokenType.Simecolon, value = ";", line = 0, column = 13 }
        };

        // Perform the scanning
        List<Lexeme> found = Lexer.scan(input);

        // Compare expected lexemes with found ones
        check(expect, found);
    }

    // Test the scanning of multiple lines of code into lexemes
    [Test]
    public void scan2()
    {
        string input = String.Join("\n", new string[] {
            "صحيح رقم1 = 5;",
            "صحيح احمد[5] = {0,1,2,3,4};",
        });

        // Expected lexemes for this input
        List<Lexeme> expect = new List<Lexeme>() {
            new Lexeme() { type = TokenType.num_, value = "صحيح", line = 0, column = 0 },
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 0, column = 5 },
            new Lexeme() { type = TokenType.equal_, value = "=", line = 0, column = 10 },
            new Lexeme() { type = TokenType.Number, value = "5", line = 0, column = 12 },
            new Lexeme() { type = TokenType.Simecolon, value = ";", line = 0, column = 13 },
            new Lexeme() { type = TokenType.num_, value = "صحيح", line = 1, column = 0 },
            new Lexeme() { type = TokenType.Ident, value = "احمد", line = 1, column = 5 },
            new Lexeme() { type = TokenType.OpenBracket, value = "[", line = 1, column = 9 },
            new Lexeme() { type = TokenType.Number, value = "5", line = 1, column = 10 },
            new Lexeme() { type = TokenType.CloseBracket, value = "]", line = 1, column = 11 },
            new Lexeme() { type = TokenType.equal_, value = "=", line = 1, column = 13 },
            new Lexeme() { type = TokenType.OpenBrace, value = "{", line = 1, column = 15 },
            new Lexeme() { type = TokenType.Number, value = "0", line = 1, column = 16 },
            new Lexeme() { type = TokenType.Comma, value = ",", line = 1, column = 17 },
            new Lexeme() { type = TokenType.Number, value = "1", line = 1, column = 18 },
            new Lexeme() { type = TokenType.Comma, value = ",", line = 1, column = 19 },
            new Lexeme() { type = TokenType.Number, value = "2", line = 1, column = 20 },
            new Lexeme() { type = TokenType.Comma, value = ",", line = 1, column = 21 },
            new Lexeme() { type = TokenType.Number, value = "3", line = 1, column = 22 },
            new Lexeme() { type = TokenType.Comma, value = ",", line = 1, column = 23 },
            new Lexeme() { type = TokenType.Number, value = "4", line = 1, column = 24 },
            new Lexeme() { type = TokenType.CloseBrace, value = "}", line = 1, column = 25 },
            new Lexeme() { type = TokenType.Simecolon, value = ";", line = 1, column = 26 }
        };

        // Perform the scanning
        List<Lexeme> found = Lexer.scan(input);

        // Compare expected lexemes with found ones
        check(expect, found);
    }

    // Test scanning of multiple lines including a while loop and an expression
    [Test]
    public void scan3()
    {
        string input = String.Join("\n", new string[] {
            "صحيح رقم1 = 5;",
            "بينما (رقم1 <= 5 )",
            "رقم1 = رقم1 + 1+2;",
        });

        // Expected lexemes for this input
        List<Lexeme> expect = new List<Lexeme>() {
            // Lexemes for line 1
            new Lexeme() { type = TokenType.num_, value = "صحيح", line = 0, column = 0 },
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 0, column = 5 },
            new Lexeme() { type = TokenType.equal_, value = "=", line = 0, column = 10 },
            new Lexeme() { type = TokenType.Number, value = "5", line = 0, column = 12 },
            new Lexeme() { type = TokenType.Simecolon, value = ";", line = 0, column = 13 },

            // Lexemes for line 2 (while loop)
            new Lexeme() { type = TokenType.while_, value = "بينما", line = 1, column = 0 },
            new Lexeme() { type = TokenType.OpenPar, value = "(", line = 1, column = 6 },
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 1, column = 7 },
            new Lexeme() { type = TokenType.le_, value = "<=", line = 1, column = 12 },
            new Lexeme() { type = TokenType.Number, value = "5", line = 1, column = 15 },
            new Lexeme() { type = TokenType.ClosePar, value = ")", line = 1, column = 16 },

            // Lexemes for line 3 (assignment and expression)
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 2, column = 0 },
            new Lexeme() { type = TokenType.equal_, value = "=", line = 2, column = 5 },
            new Lexeme() { type = TokenType.Ident, value = "رقم1", line = 2, column = 7 },
            new Lexeme() { type = TokenType.plus_, value = "+", line = 2, column = 13 },
            new Lexeme() { type = TokenType.Number, value = "1", line = 2, column = 14 },
            new Lexeme() { type = TokenType.plus_, value = "+", line = 2, column = 16 },
            new Lexeme() { type = TokenType.Number, value = "2", line = 2, column = 17 },
            new Lexeme() { type = TokenType.Simecolon, value = ";", line = 2, column = 18 }
        };

        // Perform the scanning
        List<Lexeme> found = Lexer.scan(input);

        // Compare expected lexemes with found ones
        check(expect, found);
    }
}
