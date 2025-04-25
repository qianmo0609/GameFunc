using System.Text;
using UnityEngine;

public class XMLScanner : MonoBehaviour
{
    private int _position;    // 当前扫描位置
    private string _input;     // 输入XML字符串

    // 构造函数
    public XMLScanner(string input)
    {
        _input = input.Trim();    // 去除首尾空白
        _position = 0;            // 初始化扫描位置
    }

    // 获取下一个词法单元
    public XMLToken NextToken()
    {
        while (true)
        {  // 使用循环代替递归防止栈溢出
            SkipWhitespace();  // 跳过空白字符

            // 检查是否到达文件末尾
            if (_position >= _input.Length)
            {
                return new XMLToken(XMLTokenType.EndOfFile, pos: _position);
            }

            char current = _input[_position];  // 获取当前字符

            // 处理标签结构 -------------------------------------------------
            if (current == '<')
            {
                _position++;  // 跳过'<'

                // 处理闭合标签 </tag>
                if (_position < _input.Length && _input[_position] == '/')
                {
                    _position++;  // 跳过'/'
                    string tagName = ReadName();  // 读取标签名
                    SkipWhitespace();

                    // 跳过闭合标签的'>'
                    if (_position < _input.Length && _input[_position] == '>')
                    {
                        _position++;
                    }
                    return new XMLToken(XMLTokenType.TagClose, tagName);
                }
                // 处理开始标签或自闭合标签
                else
                {
                    string tagName = ReadName();  // 读取标签名
                    SkipWhitespace();
                    return new XMLToken(XMLTokenType.TagOpen, tagName);
                }
            }

            // 处理自闭合标签结尾 -------------------------------------------
            if (current == '/' && _position + 1 < _input.Length && _input[_position + 1] == '>')
            {
                _position += 2;  // 跳过'/>'
                return new XMLToken(XMLTokenType.SelfClosingTag);
            }

            // 处理属性名 ---------------------------------------------------
            //<name id = ""><name> 和<name> John </name>
            if (IsNameStartChar(current) && _input[_position - 1] != '>')
            {
                string name = ReadName();  // 读取属性名
                return new XMLToken(XMLTokenType.AttributeName, name);
            }

            // 处理等号 -----------------------------------------------------
            if (current == '=')
            {
                _position++;  // 跳过'='
                return new XMLToken(XMLTokenType.Equals);
            }

            // 处理属性值 ---------------------------------------------------
            if (current == '"' || current == '\'')
            {
                char quote = current;      // 记录引号类型
                _position++;               // 跳过起始引号
                int start = _position;     // 记录值起始位置

                // 查找闭合引号
                while (_position < _input.Length && _input[_position] != quote)
                {
                    _position++;
                }

                // 错误处理：未闭合的引号
                if (_position >= _input.Length)
                {
                    return new XMLToken(XMLTokenType.Error, "Unclosed quotation");
                }

                string value = _input.Substring(start, _position - start);
                _position++;  // 跳过闭合引号
                return new XMLToken(XMLTokenType.AttributeValue, value);
            }

            // 处理标签闭合符 -----------------------------------------------
            if (current == '>')
            {
                _position++;  // 跳过'>'
                continue;     // 继续处理后续内容
            }

            // 处理文本内容 -------------------------------------------------
            if (!char.IsWhiteSpace(current))
            {
                int start = _position;
                // 收集直到下一个'<'之前的内容
                while (_position < _input.Length && _input[_position] != '<')
                {
                    _position++;
                }
                string text = _input.Substring(start, _position - start).Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    return new XMLToken(XMLTokenType.Text, text);
                }
            }

            // 未知字符处理 -------------------------------------------------
            return new XMLToken(XMLTokenType.Error, $"Unexpected character: {current}");
        }
    }

    // 读取符合XML规范的名称（标签名/属性名）
    private string ReadName()
    {
        var sb = new StringBuilder();
        while (_position < _input.Length && IsNameChar(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
        }
        return sb.ToString();
    }

    // 检查是否为名称起始字符
    private bool IsNameStartChar(char c)
    {
        // char.IsLetter(c) 指示指定的 Unicode 字符是否属于 Unicode 字母类别。
        return char.IsLetter(c) || c == '_' || c == ':';  // 允许字母、下划线和冒号
    }

    // 检查是否为名称有效字符
    private bool IsNameChar(char c)
    {
        return IsNameStartChar(c) ||      // 包含起始字符
               char.IsDigit(c) ||         // 允许数字
               c == '-' ||                // 允许连字符
               c == '.';                  // 允许点号
    }

    // 跳过空白字符
    private void SkipWhitespace()
    {
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            _position++;
        }
    }
}
