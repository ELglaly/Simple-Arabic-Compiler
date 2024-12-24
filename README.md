# Lexical and Syntactic Analysis System

## Overview
This project implements a Lexical Analyzer and Parser for analyzing and interpreting a programming language that includes Arabic keywords. It provides:

1. **Tokenization (Lexical Analysis):** Breaking the input source code into meaningful components (tokens).
2. **Parsing (Syntactic Analysis):** Building an Abstract Syntax Tree (AST) to represent the syntactic structure of the source code.

---

## Components

### 1. **TokenType (Enumeration)**
Defines the different types of tokens that can be identified during lexical analysis:

- **Identifiers:** `Ident`
- **Numeric Constants:** `Number`
- **Operators:** `AddOp`, `MulOp`, `LogOp`, `relOp`
- **Punctuation:** `OpenPar`, `ClosePar`, `OpenBrace`, `CloseBrace`, `OpenBracket`, `CloseBracket`, `Simecolon`, `Comma`
- **Reserved Words:** Arabic keywords like `اذا`, `اخر`, `بينما`, `ارجع`, `حقيقي`, `صحيح`, `خالى`
- **Equality Operators:** `equal_`
- **Invalid Tokens:** `invalid`

### 2. **Lexeme (Record)**
Represents a token identified during lexical analysis:

- **Properties:**
  - `value`: The string value of the token.
  - `type`: The `TokenType` of the token.
  - `line`: The line number where the token appears.
  - `column`: The starting column number of the token.

### 3. **Lexer Methods**

- **scan(string input):** Analyzes input string to identify tokens.
- **isNumber(string input):** Checks if a string represents a valid number.
- **isReal(string input):** Checks for real numbers.
- **isInt(string input):** Checks for integers.
- **isIdent(string input):** Validates identifiers using a pattern.
- **ResWord(string input):** Identifies reserved words and maps them to `TokenType`.

### 4. **Interfaces**

#### **IParser Interface**
Defines methods for parsing syntactic constructs:

- **parse():** Initiates the parsing process.
- **DeclarationList, Declaration, VarDeclaration:** Handle variable declarations.
- **FunDeclaration:** Handles function declarations.
- **CompoundStmt, StatementList, Statement:** Parse statements.
- **Expression, SimpleExpression, AdditiveExpression, Term:** Handle expressions.
- **Call, Args:** Parse function calls and arguments.

#### **IParserServices Interface**
Provides auxiliary methods for parsing:

- **TypeSpecifier:** Checks if a lexeme represents a type.
- **addOp, mulOp, relOp:** Check if a lexeme represents operators.
- **CreateNode:** Creates AST nodes.

### 5. **Classes**

#### **Node Class**
Represents a node in the AST:

- **Properties:**
  - `left`: Leftmost character position.
  - `right`: Rightmost character position.
  - `Type`: Syntactic construct type.
  - `Children`: List of child nodes.

#### **Result Class**
Encapsulates the result of parsing:

- **Properties:**
  - `last`: Position of the last successfully parsed character.
  - `error`: Error message if parsing failed.
  - `node`: AST node resulting from parsing.

---

## Features

- **Arabic Keywords:** Supports tokens and reserved words in Arabic (e.g., `اذا`, `اخر`).
- **Lexical Analysis:** Accurately identifies tokens using regular expressions.
- **Syntactic Parsing:** Constructs an AST for the input source code.
- **Extensibility:** Modular design allows easy extension for new tokens and syntactic constructs.

---

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- A code editor like [Visual Studio Code](https://code.visualstudio.com/)

