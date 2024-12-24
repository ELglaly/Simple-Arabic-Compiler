using SAC9.Lexer;

namespace SAC9.Parser;

// Interface representing a generic parser structure
public interface IParser
{
    // Parses the input and returns the overall result
    public Result Parse();

    // Handles a list of declarations in the source code
    public Result DeclarationList(int start, int end);

    // Parses a single declaration (e.g., variable or function)
    public Result Declaration(int start, int end);

    // Parses a variable declaration
    public Result VarDeclaration(int start, int end);

    // Parses a function declaration
    public Result FunDeclaration(int start, int end);

    // Parses parameters of a function
    public Result Params(int start, int end);

    // Parses a compound statement (block of code enclosed in braces)
    public Result CompoundStmt(int start, int end);

    // Parses local declarations within a block
    public Result LocalDeclarations(int start, int end);

    // Parses a list of statements
    public Result StatementList(int start, int end);

    // Parses a single statement
    public Result Statement(int start, int end);

    // Parses an expression statement (e.g., `x = 5;`)
    public Result ExpressionStmt(int start, int end);

    // Parses a selection statement (e.g., `if` statement)
    public Result SelectionStmt(int start, int end);

    // Parses an iteration statement (e.g., `while` loop)
    public Result IterationStmt(int start, int end);

    // Parses a return statement (e.g., `return x;`)
    public Result ReturnStmt(int start, int end);

    // Parses an expression
    public Result Expression(int start, int end);

    // Parses a variable usage or assignment
    public Result Var(int start, int end);

    // Parses a simple expression (e.g., relational operations like `x > y`)
    public Result SimpleExpression(int start, int end);

    // Parses additive expressions (e.g., `x + y - z`)
    public Result AdditiveExpression(int start, int end);

    // Parses terms in expressions (e.g., `x * y / z`)
    public Result Term(int start, int end);

    // Parses individual factors in expressions (e.g., a single variable or constant)
    public Result Factor(int start, int end);

    // Parses a function call
    public Result Call(int start, int end);

    // Parses arguments of a function call
    public Result Args(int start, int end);

    // Parses a list of arguments
    public Result ArgList(int start, int end);
}

// Interface providing utility services for the parser
public interface IParserServices
{
    // Checks if a lexeme represents a type specifier (e.g., `int`, `float`)
    public static abstract bool TypeSpecifier(Lexeme lexeme);

    // Checks if a lexeme represents an additive operator (e.g., `+`, `-`)
    public static abstract bool addOp(Lexeme lexeme);

    // Checks if a lexeme represents a multiplicative operator (e.g., `*`, `/`)
    public static abstract bool mulOp(Lexeme lexeme);

    // Checks if a lexeme represents a relational operator (e.g., `<`, `>=`)
    public static abstract bool relOp(Lexeme lexeme);

    // Creates a new node in the parse tree
    public static abstract Node CreateNode(string type, int left, int right, params Node[] childrens);

    // Creates a result object encapsulating parsing information
    public static abstract Result CreateResult(int last, string error, Node? node = null);
}
