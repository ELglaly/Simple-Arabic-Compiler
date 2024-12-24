namespace SAC9.Lexer;

// Enum to represent different token types for the lexer
public enum TokenType
{
    Ident,          // Identifier (e.g., variable names)
    Number,         // Numeric values
    AddOp,          // Addition or subtraction operator (+ or -)
    MulOp,          // Multiplication or division operator (* or /)
    LogOp,          // Logical operators (e.g., ==, <, >, !=)
    OpenPar,        // Open parenthesis '('
    ClosePar,       // Close parenthesis ')'
    OpenBrace,      // Open curly brace '{'
    CloseBrace,     // Close curly brace '}'
    OpenBracket,    // Open square bracket '['
    CloseBracket,   // Close square bracket ']'
    Simecolon,      // Semicolon ';'
    Comma,          // Comma ','
    Void_,          // Keyword 'void' or equivalent in a specific language
    if_,            // Keyword 'if' or equivalent in a specific language
    else_,          // Keyword 'else' or equivalent in a specific language
    while_,         // Keyword 'while' or equivalent in a specific language
    return_,        // Keyword 'return' or equivalent in a specific language
    real_,          // Keyword for real (floating-point) numbers
    num_,           // Keyword for integer numbers
    equal_,         // Assignment operator '='
    invalid         // Invalid or unrecognized token
}


// Class to represent a lexeme (token with additional details)
public record Lexeme
{
    public string value { set; get; } = "";   // The textual value of the lexeme
    public TokenType type { set; get; } = 0; // The type of token
    public int line { set; get; }            // The line number where the lexeme occurs
    public int column { set; get; }          // The column number where the lexeme starts


}
