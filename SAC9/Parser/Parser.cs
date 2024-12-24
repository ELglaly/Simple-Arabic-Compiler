using SAC9.Lexer;

namespace SAC9.Parser;

// The Parser class handles parsing of lexemes according to the defined grammar.
public class Parser : IParser
{
    // List of lexemes to be parsed
    public List<Lexeme> lexemes;

    // Constructor that initializes the parser with a list of lexemes
    public Parser(List<Lexeme> lexemes) => this.lexemes = lexemes;

    // Main parse method that starts the parsing process
    public Result Parse()
    {
        // If no lexemes are provided, return an error
        if (lexemes.Count == 0)
            return ParserServices.CreateResult(-1, "جدع", new Node { });

        // Begin parsing with the DeclarationList method
        return DeclarationList(0, lexemes.Count);
    }

    // Parses a list of declarations by recursively parsing individual declarations
    public Result DeclarationList(int start, int end)
    {
        // If start index exceeds end, return an error
        if (start > end)
            return new Result { last = -1 };

        // Parse the first declaration
        var path1 = Declaration(start, end);

        // If the first path is invalid, return the error immediately
        if (path1.last == -1)
        {
            return path1;
        }

        // If the end of the lexemes is reached, return the current valid path
        if (path1.last + 1 == end)
            return path1;

        // Recursively parse the rest of the declaration list
        var path2 = DeclarationList(path1.last + 1, end);

        // If the second path is invalid, return the valid path
        // Create a new node combining the two paths (first and second)
        Node node = ParserServices.CreateNode(
            "DeclarationList", start, path2.last + 1, path1.node, path2.node);

        // Return a result containing the combined node and error messages
        Result result = ParserServices.CreateResult(path2.last, path1.error + " " + path2.error, node);
        return result;
    }

    // Parses a single declaration, which can be a variable or function declaration
    public Result Declaration(int start, int end)
    {
        // Attempt to parse both variable and function declarations
        var path1 = VarDeclaration(start, end);
        var path2 = FunDeclaration(start, end);

        // If both paths are invalid, return an error
        if (path1.last == -1 && path2.last == -1)
        {
            return new Result { last = -1, error = "" + path1.error + " or " + path2.error };
        }

        // Create a result for the declaration node
        var result = new Result { node = new Node { Type = "Declaration", left = start } };

        // If the variable declaration was valid, add its node to the result
        if (path1.last != -1)
        {
            if (path1.node is not null)
                result.node.Children.Add(path1.node);
            result.last = path1.last;
            result.node.right = path1.last + 1;
            result.error = path1.error;
        }

        // If the function declaration was valid, add its node to the result
        if (path2.last != -1)
        {
            if (path2.node is not null)
                result.node.Children.Add(path2.node);
            result.last = path2.last;
            result.node.right = path2.last + 1;
            result.error = path2.error;
        }
        return result;
    }

    // Parses a variable declaration
    public Result VarDeclaration(int start, int end)
    {
        int i = start;

        // If there are not enough lexemes to form a valid declaration, return an error
        if (start > end - 3)
            return ParserServices.CreateResult(-1, $"incomplete VarDeclaration at line:{lexemes[i].line}");

        // Check for array declaration
        if (start < end - 5 && lexemes[start + 2].type == TokenType.OpenBracket)
        {
            // If the lexemes match the expected pattern for an array variable declaration, return a success
            if (ParserServices.TypeSpecifier(lexemes[i++]) &&
                lexemes[i++].type == TokenType.Ident &&
                lexemes[i++].type == TokenType.OpenBracket &&
                lexemes[i++].type == TokenType.Number &&
                lexemes[i++].type == TokenType.CloseBracket &&
                lexemes[i].type == TokenType.Simecolon)
            {
                return ParserServices.CreateResult(
                    i, "", ParserServices.CreateNode("VarDeclaration", start, i));
            }

            // If the pattern doesn't match, return an error
            return ParserServices.CreateResult(
                -1,
                $"incorrect VarDeclaration at line: {lexemes[i - 1].line} col: {lexemes[i - 1].column}");
        }
        else
        {
            // Check for simple variable declaration
            if (ParserServices.TypeSpecifier(lexemes[i++]) &&
                lexemes[i++].type == TokenType.Ident &&
                lexemes[i].type == TokenType.Simecolon)
            {
                return ParserServices.CreateResult(
                    i, "", ParserServices.CreateNode("VarDeclaration", start, i));
            }

            // If the pattern doesn't match, return an error
            return ParserServices.CreateResult(
                -1,
                $"incorrect VarDeclaration at line: {lexemes[i].line} col: {lexemes[i].column}");
        }
    }

    // Parses a function declaration
    public Result FunDeclaration(int start, int end)
    {
        int i = start;

        // If there are not enough lexemes to form a valid function declaration, return an error
        if (start > end - 5)
            return ParserServices.CreateResult(
                -1, $"incomplete FunDeclaration {lexemes[start].line}");

        Result path1, path2;

        // Helper function to find the closing parenthesis for function parameters
        var getNextPar = (int j) =>
        {
            while (j < end)
            {
                if (lexemes[j].type == TokenType.ClosePar)
                    return j;
                j++;
            }
            return -1;
        };

        int j, k;

        // Check for a valid function declaration syntax (type, identifier, parameters)
        if (ParserServices.TypeSpecifier(lexemes[i++]) &&
            lexemes[i++].type == TokenType.Ident &&
            lexemes[i++].type == TokenType.OpenPar && (j = getNextPar(i)) != -1)
        {
            path1 = Params(i, j++);

            // Ensure the function has a closing '}' for the body
            if ((k = Enders.CompoundStatementClose(j + 1, lexemes, end)) == -1)
                return ParserServices.CreateResult(
                    -1, $"expected {'}'} at line : {lexemes[end - 1].line} ");

            path2 = CompoundStmt(j, k);

            // Return the function declaration result with the corresponding node
            return ParserServices.CreateResult(
                k, path2.error,
                ParserServices.CreateNode("FunDeclaration", start, k + 1, path1.node, path2.node));
        }

        // If the syntax is incomplete, return an error
        return ParserServices.CreateResult(
            -1,
            $"incomplete function Declaration at line: {lexemes[i].line} column: {lexemes[i].column}");
    }

    public Result Params(int start, int end)
    {
        // Create a node to represent the list of parameters
        var node = ParserServices.CreateNode("paramList", start, end - 1);

        // Iterate through the lexemes from start to end
        for (; start < end; start++)
        {
            // Check if the lexeme is a valid type and identifier
            if (ParserServices.TypeSpecifier(lexemes[start++]) &&
                lexemes[start++].type == TokenType.Ident)
            {
                // Check if it's followed by a comma or the end of the list
                if (lexemes[start].type == TokenType.Comma || start == end)
                    node.Children.Add(ParserServices.CreateNode("param", start - 2, start + 1));
                // Check if it's a parameter with brackets (array or similar)
                else if (lexemes[start].type == TokenType.OpenBracket &&
                         lexemes[++start].type == TokenType.CloseBracket &&
                         (lexemes[start].type == TokenType.Comma || start == end))
                {
                    node.Children.Add(ParserServices.CreateNode("param", start - 4, start + 1));
                }
                else
                {
                    // Return error if the parameter structure is incorrect
                    return ParserServices.CreateError($"function paramers Incorrect at line: {lexemes[end - 1].line}");
                }
            }
            else
            {
                // Return error if the parameter is not valid
                return ParserServices.CreateError($"function paramers Incorrect at line: {lexemes[end - 1].line}");
            }
        }
        // Return the parsed result
        return ParserServices.CreateResult(end, "", node);
    }

    public Result CompoundStmt(int start, int end)
    {
        // Validate the minimum length for a compound statement
        if (start > end - 2)
            return ParserServices.CreateError("expected compound statement");

        // Ensure the statement begins with an opening brace
        if (lexemes[start++].type != TokenType.OpenBrace)
            return ParserServices.CreateError("expected {");

        // Parse local declarations (variables)
        var path1 = LocalDeclarations(start, end);

        // Parse the statement list within the compound statement
        var path2 = StatementList(path1.last + 1, end);
        if (path2.last == -1)
            return ParserServices.CreateError(path2.error);

        // Ensure the compound statement ends with a closing brace
        if (lexemes[path2.last + 1].type != TokenType.CloseBrace)
            return ParserServices.CreateError($"expected }} at line: {lexemes[path2.last].line}");

        // Return the result of parsing the compound statement
        return ParserServices.CreateResult(path2.last, "", ParserServices.CreateNode("CompoundStmt", start, path2.last + 1, path1.node, path2.node));
    }

    public Result LocalDeclarations(int start, int end)
    {
        // If no declarations are present, return an empty result
        if (start > end)
            return new Result { last = -1 };

        // Parse a single declaration
        var path1 = Declaration(start, end);

        // If the first path is invalid, return an empty result
        if (path1.last == -1)
        {
            return ParserServices.CreateResult(start - 1, "", ParserServices.CreateNode("", start, start));
        }

        // Parse additional declarations recursively
        var path2 = DeclarationList(path1.last + 1, end);

        // If the second path is invalid, return the first path
        if (path2.last == -1)
        {
            return path1;
        }

        // Create a new node for the declaration list and return the result
        Node node = ParserServices.CreateNode("DeclarationList", start, path2.last, path1.node, path2.node);
        Result result = new Result
        {
            node = node,
            last = path2.last,
            error = path1.error + path2.error
        };
        return result;
    }

    public Result StatementList(int start, int end)
    {
        // If no statements are present, return an empty result
        if (start > end - 1)
            return new Result { last = -1 };

        // Parse a single statement
        var path1 = Statement(start, end);

        // If the first path is invalid, return it
        if (path1.last == -1)
        {
            return path1;
        }

        // If the statement is the last one, return the result
        if (path1.last + 1 == end)
            return path1;

        // Parse additional statements recursively
        var path2 = StatementList(path1.last + 1, end);

        // If the second path is invalid, return the first path
        if (path2.last == -1)
        {
            return path1;
        }

        // Create a new node for the statement list and return the result
        Node node = ParserServices.CreateNode("StatementList", start, path2.last + 1, path1.node, path2.node);
        Result result = new Result { node = node, last = path2.last };
        return result;
    }

    public Result Statement(int start, int end)
    {
        // If there's only one element, it's an empty statement (warning)
        if (start == end - 1)
            return ParserServices.CreateResult(start, "warning: empty statement");

        // If there's no statement, return an error
        if (start > end - 1)
            return ParserServices.CreateError("expected statement");

        // Parse compound statement (block)
        if (lexemes[start].type == TokenType.OpenBrace)
            return CompoundStmt(start, end);

        // Parse selection statement (if)
        if (lexemes[start].type == TokenType.if_)
            return SelectionStmt(start, end);

        // Parse iteration statement (while)
        if (lexemes[start].type == TokenType.while_)
            return IterationStmt(start, end);

        // Parse return statement
        if (lexemes[start].type == TokenType.return_)
            return ReturnStmt(start, end);

        // Otherwise, treat it as an expression statement
        return ExpressionStmt(start, end);
    }

    public Result ExpressionStmt(int start, int end)
    {
        // Ensure there is at least one lexeme for the expression statement
        if (start > end - 1)
            return ParserServices.CreateError("expected expression statement");

        // Parse the expression
        var path1 = Expression(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError($"expected expression statement at line: {lexemes[start].line}");

        // Ensure the expression ends with a semicolon
        if (lexemes[path1.last].type != TokenType.Simecolon)
            return ParserServices.CreateError($"expected ; at line: {lexemes[path1.last].line}");

        // Return the parsed result
        return ParserServices.CreateResult(path1.last, "", ParserServices.CreateNode("ExpressionStmt", start, path1.last + 1, path1.node));
    }

    public Result SelectionStmt(int start, int end)
    {
        // Ensure the statement is long enough for an 'if' statement
        if (start > end - 6)
            return ParserServices.CreateError("expected selection statement");

        // Ensure the statement starts with 'if'
        if (lexemes[start++].type != TokenType.if_)
            return ParserServices.CreateError("expected if statement");

        // Ensure the statement has an opening parenthesis
        if (lexemes[start++].type != TokenType.OpenPar)
            return ParserServices.CreateError("expected (");

        // Parse the expression inside the 'if' condition
        var path1 = Expression(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError($"expected expression statement at line: {lexemes[start].line}");

        // Ensure the condition ends with a closing parenthesis
        if (lexemes[++path1.last].type != TokenType.ClosePar)
            return ParserServices.CreateError($"expected ) at line: {lexemes[path1.last].line}");

        // Parse the statement inside the 'if'
        var path2 = Statement(path1.last + 1, end);
        if (path2.last == -1)
            return ParserServices.CreateError($"expected statement at line: {lexemes[path1.last + 1].line}");

        // If there's an 'else' clause, parse it
        if (path2.last + 1 < end && lexemes[path2.last + 1].type == TokenType.else_)
        {
            var path3 = Statement(path2.last + 2, end);
            if (path3.last == -1)
                return ParserServices.CreateError($"expected statement at line: {lexemes[path2.last + 2].line}");
            return ParserServices.CreateResult(path3.last, "", ParserServices.CreateNode("SelectionStmt", start, path3.last + 1, path1.node, path2.node, path3.node));
        }

        // Return the result of the 'if' statement
        return ParserServices.CreateResult(path2.last, "", ParserServices.CreateNode("SelectionStmt", start, path2.last + 1, path1.node, path2.node));
    }

    public Result IterationStmt(int start, int end)
    {
        // Ensure the statement is long enough for a 'while' loop
        if (start > end - 3)
            return ParserServices.CreateError("expected iteration statement");

        // Ensure the statement starts with 'while'
        if (lexemes[start++].type != TokenType.while_)
            return ParserServices.CreateError("expected while statement");

        // Ensure the statement has an opening parenthesis
        if (lexemes[start++].type != TokenType.OpenPar)
            return ParserServices.CreateError("expected (");

        // Parse the expression inside the 'while' condition
        var path1 = Expression(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError($"expected expression statement at line: {lexemes[start].line}");

        // Ensure the condition ends with a closing parenthesis
        if (lexemes[++path1.last].type != TokenType.ClosePar)
            return ParserServices.CreateError($"expected ) at line: {lexemes[path1.last].line}");

        // Parse the statement inside the 'while'
        var path2 = Statement(path1.last + 1, end);
        if (path2.last == -1)
            return ParserServices.CreateError($"expected statement at line: {lexemes[path1.last + 1].line}");

        // Return the result of the 'while' loop
        return ParserServices.CreateResult(path2.last, "", ParserServices.CreateNode("IterationStmt", start, path2.last + 1, path1.node, path2.node));
    }

    public Result ReturnStmt(int start, int end)
    {
        // Ensure the return statement has at least one lexeme after 'return'
        if (start + 1 > end)
            return ParserServices.CreateError("expected return statement");

        // Ensure the statement starts with 'return'
        if (lexemes[start++].type != TokenType.return_)
            return ParserServices.CreateError("expected return statement");

        // If the return statement only has a semicolon, it's valid
        if (lexemes[start].type == TokenType.Simecolon)
            return ParserServices.CreateResult(start, "", ParserServices.CreateNode("ReturnStmt", start, start + 1));

        // Otherwise, parse the expression for the return value
        var path1 = Expression(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError($"expected expression statement at line: {lexemes[start].line}");

        // Ensure the return statement ends with a semicolon
        if (lexemes[path1.last].type != TokenType.Simecolon)
            return ParserServices.CreateError($"expected ; at line: {lexemes[path1.last].line}");

        // Return the result of the return statement
        return ParserServices.CreateResult(path1.last, "", ParserServices.CreateNode("ReturnStmt", start, path1.last + 1, path1.node));
    }


    public Result Expression(int start, int end)
    {
        // expression -> var = expression | simpleExpression
        if (start > end - 1)
            return ParserServices.CreateError("expected expression");

        // Try parsing a variable assignment (var = expression)
        var path1 = Var(start, end);
        if (path1.last != -1)
        {
            // If a variable assignment is found and there is an '=' symbol after it
            if (path1.last + 1 < end &&
                               lexemes[path1.last + 1].type == TokenType.equal_)
            {
                var path2 = Expression(path1.last + 2, end);
                if (path2.last == -1)
                    return ParserServices.CreateError(
                                               $"expected expression at line: {lexemes[path1.last + 2].line}");
                return ParserServices.CreateResult(
                                       path2.last, "",
                                                              ParserServices.CreateNode("Expression", start, path2.last + 1,
                                                                                                           path1.node, path2.node));
            }

            // If no assignment, process as a simple expression
            return SimpleExpression(start, end);
        }

        // If no variable assignment, process as a simple expression
        return SimpleExpression(start, end);
    }

    public Result Var(int start, int end)
    {
        // Parsing a variable
        int i = start;
        if (start < end && lexemes[i++].type != TokenType.Ident)
        {
            return ParserServices.CreateError("expected variable");
        }

        // Check if there's an open bracket for array-like structures (e.g., var[i])
        Result path1;
        if (start < end && lexemes[i++].type == TokenType.OpenBracket)
        {
            int j = Enders.BracetClose(i, lexemes, end);
            if (j == -1)
                return ParserServices.CreateError(
                    $"expected ] at line: {lexemes[start].line}");

            // Parse the expression inside the brackets
            path1 = Expression(i, j);
            if (path1.last == -1)
                return ParserServices.CreateError(
                    $"expected a valid expression in line: {lexemes[i].line}");

            // Return result with parsed variable node
            return ParserServices.CreateResult(
                j, "", ParserServices.CreateNode("Var", i, j + 1, path1.node));
        }

        // Return the result when no brackets are found (simple variable)
        return ParserServices.CreateResult(
            start, "", ParserServices.CreateNode("Var", start, start + 1));
    }

    public Result SimpleExpression(int start, int end)
    {
        // simpleExpression -> additiveExpression | additiveExpression logOp additiveExpression
        if (start > end - 1)
            return ParserServices.CreateError("expected simple expression");

        var path1 = AdditiveExpression(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError(
                $"expected additive expression at line: {lexemes[start].line}");

        // Check for logical operators (e.g., AND, OR) between additive expressions
        if (path1.last + 1 < end &&
            lexemes[path1.last + 1].type == TokenType.LogOp)
        {
            var path2 = AdditiveExpression(path1.last + 2, end);
            if (path2.last == -1)
                return ParserServices.CreateError(
                    $"expected additive expression at line: {lexemes[path1.last + 2].line}");
            return ParserServices.CreateResult(
                path2.last, "",
                ParserServices.CreateNode("SimpleExpression", start, path2.last + 1,
                                          path1.node, path2.node));
        }

        // If no logical operator is found, return the result of the additive expression
        return ParserServices.CreateResult(
            path1.last, "",
            ParserServices.CreateNode("SimpleExpression", start, path1.last + 1,
                                      path1.node));
    }

    public Result AdditiveExpression(int start, int end)
    {
        // additiveExpression -> term | term addOp term
        if (start > end - 1)
            return ParserServices.CreateError("expected additive expression");

        var path1 = Term(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError(
                $"expected term at line: {lexemes[start].line}");

        // Check for additive operators (e.g., +, -) between terms
        if (path1.last + 1 < end &&
            lexemes[path1.last + 1].type == TokenType.AddOp)
        {
            var path2 = AdditiveExpression(path1.last + 2, end);
            if (path2.last == -1)
                return ParserServices.CreateError(
                    $"expected additive expression at line: {lexemes[path1.last + 2].line}");
            return ParserServices.CreateResult(
                path2.last, "",
                ParserServices.CreateNode("AdditiveExpression", start, path2.last + 1,
                                          path1.node, path2.node));
        }

        // If no additive operator is found, return the result of the term
        return ParserServices.CreateResult(
            path1.last, "",
            ParserServices.CreateNode("AdditiveExpression", start, path1.last + 1,
                                      path1.node));
    }

    public Result Term(int start, int end)
    {
        // term -> factor | factor mulOp factor
        if (start > end - 1)
            return ParserServices.CreateError("expected term");

        var path1 = Factor(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError(
                $"expected factor at line: {lexemes[start].line}");

        // Check for multiplication or division operators between factors
        if (path1.last + 1 < end &&
            lexemes[path1.last + 1].type == TokenType.MulOp)
        {
            var path2 = Term(path1.last + 2, end);
            if (path2.last == -1)
                return ParserServices.CreateError(
                    $"expected term at line: {lexemes[path1.last + 2].line}");
            return ParserServices.CreateResult(
                path2.last, "",
                ParserServices.CreateNode("Term", start, path2.last + 1, path1.node,
                                          path2.node));
        }

        // If no multiplication operator is found, return the result of the factor
        return ParserServices.CreateResult(
            path1.last, "",
            ParserServices.CreateNode("Term", start, path1.last + 1, path1.node));
    }

    public Result Factor(int start, int end)
    {
        // factor -> ( expression ) | var | call | args | Num
        if (start > end - 1)
            return ParserServices.CreateError("expected factor");

        // If the factor is a number
        if (lexemes[start].type == TokenType.Number)
            return ParserServices.CreateResult(
                               start, "",
                                                  ParserServices.CreateNode("Factor", start, start + 1));

        // If the factor is an open parenthesis (expression inside parentheses)
        if (lexemes[start].type == TokenType.OpenPar)
        {
            var path1 = Expression(start + 1, end);
            if (path1.last == -1)
                return ParserServices.CreateError(
                    $"expected expression at line: {lexemes[start].line}");
            if (path1.last + 1 < end &&
                lexemes[path1.last + 1].type != TokenType.ClosePar)
            {
                return ParserServices.CreateError(
                    $"expected ) at line: {lexemes[path1.last].line}");
            }
            return ParserServices.CreateResult(
                path1.last + 1, "",
                ParserServices.CreateNode("Factor", start, path1.last + 2,
                                          path1.node));
        }

        // Try parsing as a variable
        var path = Var(start, end);
        if (path.last != -1)
            return path;

        // Try parsing as a function call
        path = Call(start, end);

        return path;
    }

    public Result Call(int start, int end)
    {
        // call -> ident ( args )
        if (start > end - 2)
            return ParserServices.CreateError("expected call");

        // Check for valid function name (ident)
        if (lexemes[start++].type != TokenType.Ident)
            return ParserServices.CreateError("expected ident");

        // Check for opening parenthesis
        if (lexemes[start++].type != TokenType.OpenPar)
            return ParserServices.CreateError("expected (");

        var path1 = Args(start, end);
        if (path1.last == -1)
            return ParserServices.CreateError(
                $"expected args at line: {lexemes[start].line}");

        // Check for closing parenthesis
        if (lexemes[path1.last].type != TokenType.ClosePar)
            return ParserServices.CreateError(
                $"expected ) at line: {lexemes[path1.last].line}");

        return ParserServices.CreateResult(
            path1.last, "",
            ParserServices.CreateNode("Call", start, path1.last + 1, path1.node));
    }

    public Result Args(int start, int end)
    {
        // args -> expression | expression , args
        if (start > end)
            return new Result { last = -1 };

        var path1 = Expression(start, end);
        if (path1.last == -1)
        {
            return path1;
        }

        // Check for additional arguments separated by commas
        if (path1.last + 1 == end)
            return path1;
        if (lexemes[path1.last].type != TokenType.Comma)
            return path1;

        var path2 = Args(path1.last + 1, end);
        if (path2.last == -1)
        {
            return path1;
        }

        // Create and return a new Args node
        Node node = ParserServices.CreateNode("Args", start, path2.last + 1,
                                              path1.node, path2.node);
        Result result = new Result { node = node, last = path2.last };
        return result;
    }

    public Result ArgList(int start, int end)
    {
        // argList -> expression | expression , argList
        if (start > end)
            return new Result { last = -1 };

        var path1 = Expression(start, end);
        if (path1.last == -1)
        {
            return path1;
        }

        // Check for additional arguments separated by commas
        if (path1.last + 1 == end)
            return path1;
        if (lexemes[path1.last].type != TokenType.Comma)
            return path1;

        var path2 = ArgList(path1.last + 1, end);
        if (path2.last == -1)
        {
            return path1;
        }

        // Create and return a new ArgList node
        Node node = ParserServices.CreateNode("ArgList", start, path2.last + 1,
                                              path1.node, path2.node);
        Result result = new Result { node = node, last = path2.last };
        return result;
    }

}