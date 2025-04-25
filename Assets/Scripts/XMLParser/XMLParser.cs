// -------------------- 语法解析器部分 --------------------
using System.Collections.Generic;
using System.Linq;

public class XMLParser
{
    private XMLScanner _scanner;
    private XMLToken _currentToken;
    private Stack<XMLElement> _stack = new Stack<XMLElement>();
    private XMLElement _root;

    public XMLElement Parse(string xml)
    {
        _scanner = new XMLScanner(xml);
        _currentToken = _scanner.NextToken();
        _root = new XMLElement { Name = "__root__" };
        _stack.Push(_root);

        while (_currentToken.Type != XMLTokenType.EndOfFile)
        {
            UnityEngine.Debug.Log($"{_currentToken.Type},{_currentToken.Value}");
            switch (_currentToken.Type)
            {
                case XMLTokenType.TagOpen:
                    ParseOpenTag();
                    break;
                case XMLTokenType.TagClose:
                    ParseCloseTag();
                    break;
                case XMLTokenType.SelfClosingTag:
                    ParseSelfClosingTag();
                    break;
                case XMLTokenType.Text:
                    ParseText();
                    break;
                case XMLTokenType.Error:
                    throw new XMLParseException(_currentToken.Value, _currentToken.Position);
                default:
                    Advance();
                    break;
            }
        }

        ValidateStructure();
        return _root.Children.FirstOrDefault();
    }

    private void ParseOpenTag()
    {
        var element = new XMLElement
        {
            Name = _currentToken.Value,
            StartPosition = _currentToken.Position
        };

        _stack.Peek().Children.Add(element);
        _stack.Push(element);
        Advance();

        ParseAttributes();

        if (_currentToken.Type == XMLTokenType.SelfClosingTag)
        {
            _stack.Pop();  // 自闭合标签不保留在栈中
            Advance();
        }
    }

    private void ParseAttributes()
    {
        while (_currentToken.Type == XMLTokenType.AttributeName)
        {
            string key = _currentToken.Value;
            Advance();

            if (_currentToken.Type != XMLTokenType.Equals)
                throw new XMLParseException($"Missing '=' after attribute '{key}'", _currentToken.Position);
            Advance();

            if (_currentToken.Type != XMLTokenType.AttributeValue)
                throw new XMLParseException($"Missing value for attribute '{key}'", _currentToken.Position);

            _stack.Peek().Attributes[key] = _currentToken.Value;
            Advance();
        }
    }

    private void ParseCloseTag()
    {
        if (_stack.Count <= 1)  // 防止弹出根元素
            throw new XMLParseException("Unexpected closing tag", _currentToken.Position);

        string expected = _stack.Pop().Name;
        if (expected != _currentToken.Value)
            throw new XMLParseException($"Mismatched tag: </{_currentToken.Value}> expected </{expected}>", _currentToken.Position);

        Advance();
    }

    private void ParseSelfClosingTag()
    {
        var element = new XMLElement
        {
            Name = _currentToken.Value,
            IsSelfClosing = true
        };
        _stack.Peek().Children.Add(element);
        Advance();
    }

    private void ParseText()
    {
        if (!string.IsNullOrWhiteSpace(_currentToken.Value))
            _stack.Peek().TextContent += _currentToken.Value + " ";
        Advance();
    }

    private void ValidateStructure()
    {
        if (_stack.Count != 1)
            throw new XMLParseException($"Unclosed tag: {_stack.Peek().Name}", _stack.Peek().StartPosition);
    }

    private void Advance() => _currentToken = _scanner.NextToken();
}
