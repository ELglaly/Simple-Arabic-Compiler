using SAC.Lexer;
using SAC.Parser;


namespace test;

public class parserTest {
  // [Test]
  // public void Test1()
  // {
  //   Parser.lexemes= Lexer.scan("1 + 2;");
  //   var result = Parser.Parse();
  //   Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
  // }
  [Test]
  public void Test2() {
    string input = String.Join("\n", new string[] {
      "صحيح رقم1;",
      // "رقم1 = 5;",


    });

        Parser parser = new Parser(Lexer.scan(input));
        var result = parser.Parse();
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

    }

  [Test]
  public void Test3() {
    string input = String.Join("\n", new string[] {
      "صحيح رقم1 = 5;",
      "بينما (رقم1 <= 5 )",
      "{رقم1 = رقم1 + 1+2;}",
    });
        Parser parser = new Parser(Lexer.scan(input));
        var result = parser.Parse();
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
    }
}
