using System.Text;
using UnityEngine;

public class XMLScanner : MonoBehaviour
{
    private int _position;    // ��ǰɨ��λ��
    private string _input;     // ����XML�ַ���

    // ���캯��
    public XMLScanner(string input)
    {
        _input = input.Trim();    // ȥ����β�հ�
        _position = 0;            // ��ʼ��ɨ��λ��
    }

    // ��ȡ��һ���ʷ���Ԫ
    public XMLToken NextToken()
    {
        while (true)
        {  // ʹ��ѭ������ݹ��ֹջ���
            SkipWhitespace();  // �����հ��ַ�

            // ����Ƿ񵽴��ļ�ĩβ
            if (_position >= _input.Length)
            {
                return new XMLToken(XMLTokenType.EndOfFile, pos: _position);
            }

            char current = _input[_position];  // ��ȡ��ǰ�ַ�

            // �����ǩ�ṹ -------------------------------------------------
            if (current == '<')
            {
                _position++;  // ����'<'

                // ����պϱ�ǩ </tag>
                if (_position < _input.Length && _input[_position] == '/')
                {
                    _position++;  // ����'/'
                    string tagName = ReadName();  // ��ȡ��ǩ��
                    SkipWhitespace();

                    // �����պϱ�ǩ��'>'
                    if (_position < _input.Length && _input[_position] == '>')
                    {
                        _position++;
                    }
                    return new XMLToken(XMLTokenType.TagClose, tagName);
                }
                // ����ʼ��ǩ���Ապϱ�ǩ
                else
                {
                    string tagName = ReadName();  // ��ȡ��ǩ��
                    SkipWhitespace();
                    return new XMLToken(XMLTokenType.TagOpen, tagName);
                }
            }

            // �����Ապϱ�ǩ��β -------------------------------------------
            if (current == '/' && _position + 1 < _input.Length && _input[_position + 1] == '>')
            {
                _position += 2;  // ����'/>'
                return new XMLToken(XMLTokenType.SelfClosingTag);
            }

            // ���������� ---------------------------------------------------
            //<name id = ""><name> ��<name> John </name>
            if (IsNameStartChar(current) && _input[_position - 1] != '>')
            {
                string name = ReadName();  // ��ȡ������
                return new XMLToken(XMLTokenType.AttributeName, name);
            }

            // ����Ⱥ� -----------------------------------------------------
            if (current == '=')
            {
                _position++;  // ����'='
                return new XMLToken(XMLTokenType.Equals);
            }

            // ��������ֵ ---------------------------------------------------
            if (current == '"' || current == '\'')
            {
                char quote = current;      // ��¼��������
                _position++;               // ������ʼ����
                int start = _position;     // ��¼ֵ��ʼλ��

                // ���ұպ�����
                while (_position < _input.Length && _input[_position] != quote)
                {
                    _position++;
                }

                // ������δ�պϵ�����
                if (_position >= _input.Length)
                {
                    return new XMLToken(XMLTokenType.Error, "Unclosed quotation");
                }

                string value = _input.Substring(start, _position - start);
                _position++;  // �����պ�����
                return new XMLToken(XMLTokenType.AttributeValue, value);
            }

            // �����ǩ�պϷ� -----------------------------------------------
            if (current == '>')
            {
                _position++;  // ����'>'
                continue;     // ���������������
            }

            // �����ı����� -------------------------------------------------
            if (!char.IsWhiteSpace(current))
            {
                int start = _position;
                // �ռ�ֱ����һ��'<'֮ǰ������
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

            // δ֪�ַ����� -------------------------------------------------
            return new XMLToken(XMLTokenType.Error, $"Unexpected character: {current}");
        }
    }

    // ��ȡ����XML�淶�����ƣ���ǩ��/��������
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

    // ����Ƿ�Ϊ������ʼ�ַ�
    private bool IsNameStartChar(char c)
    {
        // char.IsLetter(c) ָʾָ���� Unicode �ַ��Ƿ����� Unicode ��ĸ���
        return char.IsLetter(c) || c == '_' || c == ':';  // ������ĸ���»��ߺ�ð��
    }

    // ����Ƿ�Ϊ������Ч�ַ�
    private bool IsNameChar(char c)
    {
        return IsNameStartChar(c) ||      // ������ʼ�ַ�
               char.IsDigit(c) ||         // ��������
               c == '-' ||                // �������ַ�
               c == '.';                  // ������
    }

    // �����հ��ַ�
    private void SkipWhitespace()
    {
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            _position++;
        }
    }
}
