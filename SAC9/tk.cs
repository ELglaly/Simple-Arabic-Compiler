using SAC9.Parser;
using SAC9.Lexer;

namespace SAC9
{
    // Interface for a class that can parse a source code string and return a syntax tree
    public interface Itk
    {
        // Method to generate a tree (abstract syntax tree) from the source string
        public Node tree(string source);
    }



    public class Tk
    {
        // Method to parse the source string and return a result object (as a JSON string)
        public string tree(string source)
        {
            // Create a parser and scan the source using the Lexer to tokenize it
            Parser.Parser parser = new Parser.Parser(Lexer.Lexer.scan(source));

            // Call the Parse method to process the tokenized input and generate a result
            var res = parser.Parse();

            // Serialize the result to JSON and print it to the console
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(res));

            // Return the error message from the result, if any
            return res.error;
        }

        // Method to lex (tokenize) the source string and return a list of Lexemes
        public List<Lexeme> lex(string source)
        {
            // Scan the source code and obtain a list of Lexemes
            Parser.Parser parser = new Parser.Parser(Lexer.Lexer.scan(source));
            var res = Lexer.Lexer.scan(source);

            // Return the list of Lexemes as a collection
            return res.ToList();
        }

        // Main method for entry point (currently empty)
        public static void main(string[] args)
        {
            // Placeholder for future implementation
        }
    }
}
