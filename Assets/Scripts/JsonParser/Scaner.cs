// ========================
// 3. 扫描器核心逻辑
// ========================
using System.Collections.Generic;
using System;
using UnityEngine;

public class Scanner
{
    // ----------
    // 3.1 成员变量
    // ----------
    private readonly string source;  // 输入源字符串
    private int start;               // 当前词素起始位置
    private int current;             // 当前扫描位置
    private int line = 1;            // 当前行号（用于错误报告）
    private readonly List<Token> tokens = new List<Token>(); // 生成的词素集合

    // 构造函数：接收输入字符串
    public Scanner(string source)
    {
        this.source = source;
    }

    // ----------
    // 3.2 公开方法
    // ----------
    public List<Token> Scan()
    {
        while (!IsAtEnd())
        {
            start = current;  // 记录词素起始位置
            ScanToken();      // 处理每个字符
        }

        tokens.Add(new Token(TokenType.EOF, null)); // 添加结束标记
        return tokens;
    }

    // ----------
    // 3.3 核心扫描逻辑
    // ----------
    private bool IsAtEnd() => current >= source.Length; // 是否扫描到末尾

    private void ScanToken()
    {
        char c = Advance();  // 获取当前字符并前进
        switch (c)
        {
            // 处理单字符词素
            case '{': AddToken(TokenType.LEFT_BRACE, c); break;
            case '}': AddToken(TokenType.RIGHT_BRACE, c); break;
            case '[': AddToken(TokenType.LEFT_BRACKET, c); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET, c); break;
            case ',': AddToken(TokenType.COMMA, c); break;
            case ':': AddToken(TokenType.COLON, c); break;

            // 处理换行（更新行号）
            case '\n': line++; break;

            // 忽略空格
            case ' ': break;

            // 处理字符串字面量
            case '"':               
                AddString(); break;

            // 处理负数（需后跟数字）
            case '-':
                if (char.IsDigit(Peek()))
                {
                    Advance(); // 消耗负号
                    AddNumber();
                }
                else
                {
                    throw new ArgumentException($"'-' must be followed by a digit at line {line}");
                }
                break;

            // 处理数字和关键字
            default:
                if (char.IsDigit(c)) AddNumber();
                else if (char.IsLetter(c)) AddKeyword();
                else throw new ArgumentException($"Unexpected character '{c}' at line {line}");
                break;
        }
    }

    // ----------
    // 3.4 工具方法
    // ----------
    private void AddToken(TokenType type, char character)
    {
        tokens.Add(new Token(type, character.ToString()));
    }

    private char Advance() => source[current++]; // 获取当前字符并移动指针

    // ----------
    // 3.5 字符串处理
    // ----------
    private void AddString()
    {
        // 持续扫描直到遇到闭合引号
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++; // 处理字符串内的换行
            Advance();
        }

        if (IsAtEnd())
            throw new ArgumentException($"Unterminated string at line {line}");

        Advance(); // 跳过闭合引号

        // 提取引号间的内容
        int startIndex = start + 1;  // 跳过开始引号
        int length = current - start - 2; // 计算有效长度
        string value = source.Substring(startIndex, length);
        tokens.Add(new Token(TokenType.STRING, value));
    }

    // ----------
    // 3.6 数字处理
    // ----------
    private void AddNumber()
    {
        // 扫描整数部分
        while (char.IsDigit(Peek())) Advance();

        // 处理小数部分（如果有）
        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance(); // 消耗小数点
            while (char.IsDigit(Peek())) Advance();
            ParseNumber(true); // 解析为浮点数
        }
        else
        {
            ParseNumber(false); // 解析为整数
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
    // 3.7 前瞻字符
    // ----------
    private char Peek() => IsAtEnd() ? '\0' : source[current]; // 查看当前字符
    private char PeekNext() => (current + 1 >= source.Length) ? '\0' : source[current + 1]; // 查看下一字符

    // ----------
    // 3.8 关键字处理
    // ----------
    private void AddKeyword()
    {
        // 扫描连续字母
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
