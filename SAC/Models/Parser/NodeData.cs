namespace SAC9.Parser;

// Represents a node in the parse tree
public record Node
{
    // The starting position of this node in the input
    public int left { get; set; }

    // The ending position of this node in the input
    public int right { get; set; }

    // The type of this node (e.g., "Expression", "Statement")
    public string Type { get; set; } = string.Empty;

    // The child nodes of this node, representing sub-structures in the parse tree
    public List<Node> Children { get; } = new List<Node>();
}

// Represents the result of a parsing operation
public record Result
{
    // The position of the last successfully parsed token (-1 indicates an error)
    public int last { get; set; }

    // Error message, if any, describing what went wrong during parsing
    public string error { get; set; } = string.Empty;

    // The parse tree node generated from this parsing operation (null if an error occurred)
    public Node? node { get; set; } = new Node();
}
