using SAC9.Lexer;

namespace SAC9.Parser;

public class ParserServices : IParserServices
{
    // Checks if the lexeme is a type specifier (real, num, or void)
    public static bool TypeSpecifier(Lexeme lexeme)
    {
        return lexeme.type == TokenType.real_ || lexeme.type == TokenType.num_ || lexeme.type == TokenType.Void_;
    }

    // Checks if the lexeme is an addition operator
    public static bool addOp(Lexeme lexeme)
    {
        return lexeme.type == TokenType.AddOp;
    }

    // Checks if the lexeme is a multiplication operator
    public static bool mulOp(Lexeme lexeme)
    {
        return lexeme.type == TokenType.MulOp;
    }

    // Checks if the lexeme is a logical operator
    public static bool relOp(Lexeme lexeme)
    {
        return lexeme.type == TokenType.LogOp;
    }

    // Creates a node in the abstract syntax tree (AST) with given type, position, and children nodes
    public static Node CreateNode(string type, int left, int right, params Node?[] children)
    {
        Node node = new Node { Type = type, left = left, right = right }; // Create the node
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] is not null)
                node.Children.Add(children[i]); // Add each non-null child to the node
        }
        return node;
    }

    // Creates a result object with the last index, error message, and optional node
    public static Result CreateResult(int last, string error, Node? node = null)
    {
        return new Result { last = last, error = error, node = node };
    }

    // Creates a result indicating an error, with a message and a last index of -1
    public static Result CreateError(string error)
    {
        return new Result { last = -1, error = error };
    }
}

public static class Enders
{
    // Finds the closing brace for a compound statement (block of code wrapped in {})
    public static int CompoundStatementClose(int i, List<Lexeme> lex, int end)
    {
        int cnt = 1; // Start with a count of 1 for the opening brace
        for (; i < end; i++)
        {
            if (lex[i].type == TokenType.OpenBrace) // Increment for each opening brace
                cnt++;
            else if (lex[i].type == TokenType.CloseBrace) // Decrement for each closing brace
                cnt--;
            if (cnt == 0) // When count reaches 0, the closing brace is found
                return i;
        }
        return -1; // Return -1 if no matching closing brace is found
    }

    // Finds the closing bracket for an array or index operation ([])
    public static int BracetClose(int i, List<Lexeme> lex, int end)
    {
        int cnt = 1; // Start with a count of 1 for the opening bracket
        for (; i < end; i++)
        {
            if (lex[i].type == TokenType.OpenBracket) // Increment for each opening bracket
                cnt++;
            else if (lex[i].type == TokenType.CloseBracket) // Decrement for each closing bracket
                cnt--;
            if (cnt == 0) // When count reaches 0, the closing bracket is found
                return i;
        }
        return -1; // Return -1 if no matching closing bracket is found
    }
}
