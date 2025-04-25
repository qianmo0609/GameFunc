using System;
using System.Collections.Generic;

// -------------------- 数据结构 --------------------
public class XMLElement
{
    public string Name { get; set; }
    public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
    public List<XMLElement> Children { get; } = new List<XMLElement>();
    public string TextContent { get; set; }
    public bool IsSelfClosing { get; set; }
    public int StartPosition { get; set; }
}

public class XMLParseException : Exception
{
    public int ErrorPosition { get; }

    public XMLParseException(string message, int position) : base($"{message} (at position {position})")
    {
        ErrorPosition = position;
    }
}