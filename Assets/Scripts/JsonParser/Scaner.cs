// ========================
// 3. ɨ���������߼�
// ========================
using System.Collections.Generic;
using System;
using UnityEngine;

public class Scanner
{
    // ----------
    // 3.1 ��Ա����
    // ----------
    private readonly string source;  // ����Դ�ַ���
    private int start;               // ��ǰ������ʼλ��
    private int current;             // ��ǰɨ��λ��
    private int line = 1;            // ��ǰ�кţ����ڴ��󱨸棩
    private readonly List<Token> tokens = new List<Token>(); // ���ɵĴ��ؼ���

    // ���캯�������������ַ���
    public Scanner(string source)
    {
        this.source = source;
    }

    // ----------
    // 3.2 ��������
    // ----------
    public List<Token> Scan()
    {
        while (!IsAtEnd())
        {
            start = current;  // ��¼������ʼλ��
            ScanToken();      // ����ÿ���ַ�
        }

        tokens.Add(new Token(TokenType.EOF, null)); // ��ӽ������
        return tokens;
    }

    // ----------
    // 3.3 ����ɨ���߼�
    // ----------
    private bool IsAtEnd() => current >= source.Length; // �Ƿ�ɨ�赽ĩβ

    private void ScanToken()
    {
        char c = Advance();  // ��ȡ��ǰ�ַ���ǰ��
        switch (c)
        {
            // �����ַ�����
            case '{': AddToken(TokenType.LEFT_BRACE, c); break;
            case '}': AddToken(TokenType.RIGHT_BRACE, c); break;
            case '[': AddToken(TokenType.LEFT_BRACKET, c); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET, c); break;
            case ',': AddToken(TokenType.COMMA, c); break;
            case ':': AddToken(TokenType.COLON, c); break;

            // �����У������кţ�
            case '\n': line++; break;

            // ���Կո�
            case ' ': break;

            // �����ַ���������
            case '"':               
                AddString(); break;

            // ���������������֣�
            case '-':
                if (char.IsDigit(Peek()))
                {
                    Advance(); // ���ĸ���
                    AddNumber();
                }
                else
                {
                    throw new ArgumentException($"'-' must be followed by a digit at line {line}");
                }
                break;

            // �������ֺ͹ؼ���
            default:
                if (char.IsDigit(c)) AddNumber();
                else if (char.IsLetter(c)) AddKeyword();
                else throw new ArgumentException($"Unexpected character '{c}' at line {line}");
                break;
        }
    }

    // ----------
    // 3.4 ���߷���
    // ----------
    private void AddToken(TokenType type, char character)
    {
        tokens.Add(new Token(type, character.ToString()));
    }

    private char Advance() => source[current++]; // ��ȡ��ǰ�ַ����ƶ�ָ��

    // ----------
    // 3.5 �ַ�������
    // ----------
    private void AddString()
    {
        // ����ɨ��ֱ�������պ�����
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++; // �����ַ����ڵĻ���
            Advance();
        }

        if (IsAtEnd())
            throw new ArgumentException($"Unterminated string at line {line}");

        Advance(); // �����պ�����

        // ��ȡ���ż������
        int startIndex = start + 1;  // ������ʼ����
        int length = current - start - 2; // ������Ч����
        string value = source.Substring(startIndex, length);
        tokens.Add(new Token(TokenType.STRING, value));
    }

    // ----------
    // 3.6 ���ִ���
    // ----------
    private void AddNumber()
    {
        // ɨ����������
        while (char.IsDigit(Peek())) Advance();

        // ����С�����֣�����У�
        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance(); // ����С����
            while (char.IsDigit(Peek())) Advance();
            ParseNumber(true); // ����Ϊ������
        }
        else
        {
            ParseNumber(false); // ����Ϊ����
        }
    }

    private void ParseNumber(bool isDouble)
    {
        string numberStr = source.Substring(start, current - start);
        try
        {
            if (isDouble)
                tokens.Add(new Token(TokenType.NUMBER, double.Parse(numberStr)));
            else
                tokens.Add(new Token(TokenType.NUMBER, int.Parse(numberStr)));
        }
        catch
        {
            throw new ArgumentException($"Invalid number format '{numberStr}' at line {line}");
        }
    }

    // ----------
    // 3.7 ǰհ�ַ�
    // ----------
    private char Peek() => IsAtEnd() ? '\0' : source[current]; // �鿴��ǰ�ַ�
    private char PeekNext() => (current + 1 >= source.Length) ? '\0' : source[current + 1]; // �鿴��һ�ַ�

    // ----------
    // 3.8 �ؼ��ִ���
    // ----------
    private void AddKeyword()
    {
        // ɨ��������ĸ
        while (char.IsLetter(Peek())) Advance();

        string keyword = source.Substring(start, current - start);
        switch (keyword)
        {
            case "true": tokens.Add(new Token(TokenType.BOOLEAN, true)); break;
            case "false": tokens.Add(new Token(TokenType.BOOLEAN, false)); break;
            case "null": tokens.Add(new Token(TokenType.NULL, null)); break;
            default: throw new ArgumentException($"Unexpected keyword '{keyword}' at line {line}");
        }
    }
}
