// ===========================================
// 1. 解析器核心类
// ===========================================
using System.Collections.Generic;
using System;

public class Parser
{
    // -------------------------------------
    // 1.1 成员变量
    // -------------------------------------
    private readonly List<Token> tokens; // 词法单元列表
    private int current;                 // 当前处理位置索引

    // -------------------------------------
    // 1.2 构造函数
    // -------------------------------------
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        current = 0; // 初始化当前位置
    }

    // ===========================================
    // 2. 公开入口方法
    // ===========================================
    public object Parse()
    {
        Token token = Advance();      // 获取第一个有效token
        return ParseFromToken(token); // 启动递归解析
    }

    // ===========================================
    // 3. 递归解析核心逻辑
    // ===========================================
    private object ParseFromToken(Token token)
    {
        switch (token.Type)
        {
            // 基本类型直接返回值
            case TokenType.STRING:
            case TokenType.NUMBER:
            case TokenType.BOOLEAN:
            case TokenType.NULL:
                return token.Value;

            // 对象类型解析
            case TokenType.LEFT_BRACE:
                return ParseObject(); // 进入对象解析流程

            // 数组类型解析
            case TokenType.LEFT_BRACKET:
                return ParseArray(); // 进入数组解析流程

            default:
                throw new ArgumentException($"非法起始token类型: {token.Type}", nameof(token));
        }
    }

    // ===========================================
    // 4. JSON对象解析（字典类型）
    // ===========================================
    private Dictionary<string, object> ParseObject()
    {
        var jsonObject = new Dictionary<string, object>();
        Token keyToken = Advance(); // 获取第一个key的token

        // 循环处理键值对，直到遇到右大括号
        while (keyToken.Type != TokenType.RIGHT_BRACE)
        {
            // 边界检查：防止未闭合对象
            if (keyToken.Type == TokenType.EOF)
                throw new ArgumentException("JSON对象未正确闭合");

            // 键必须是字符串类型
            if (keyToken.Type != TokenType.STRING)
                throw new ArgumentException($"对象键必须是字符串类型，实际类型为: {keyToken.Type}");

            // 处理键值对结构
            Consume(TokenType.COLON, "键值对缺少冒号分隔符"); // 验证冒号
            Token valueToken = Advance();                     // 获取值token
            jsonObject[(string)keyToken.Value] = ParseFromToken(valueToken); // 递归解析值

            // 处理逗号或结束符
            ConsumeCommaUnless(TokenType.RIGHT_BRACE);
            keyToken = Advance(); // 获取下一个键或结束符
        }

        return jsonObject;
    }

    // ===========================================
    // 5. JSON数组解析（列表类型）
    // ===========================================
    private List<object> ParseArray()
    {
        var jsonArray = new List<object>();
        Token token = Advance(); // 获取第一个元素token

        // 循环处理元素，直到遇到右方括号
        while (token.Type != TokenType.RIGHT_BRACKET)
        {
            // 边界检查：防止未闭合数组
            if (token.Type == TokenType.EOF)
                throw new ArgumentException("JSON数组未正确闭合");

            jsonArray.Add(ParseFromToken(token)); // 递归解析元素值
            ConsumeCommaUnless(TokenType.RIGHT_BRACKET); // 处理分隔符
            token = Advance(); // 获取下一个元素或结束符
        }

        return jsonArray;
    }

    // ===========================================
    // 6. 工具方法
    // ===========================================

    // -------------------------------------
    // 6.1 前进到下一个token
    // -------------------------------------
    private Token Advance()
    {
        if (current >= tokens.Count)
            throw new IndexOutOfRangeException("意外的token流终止");

        return tokens[current++]; // 返回当前token后移动指针
    }

    // -------------------------------------
    // 6.2 验证并消费指定类型token
    // -------------------------------------
    private void Consume(TokenType expectedType, string errorMessage)
    {
        if (Peek().Type != expectedType)
            throw new ArgumentException($"{errorMessage}. 预期类型: {expectedType}, 实际类型: {Peek().Type}");

        current++; // 验证通过后移动指针
    }

    // -------------------------------------
    // 6.3 处理逗号分隔逻辑
    // -------------------------------------
    private void ConsumeCommaUnless(TokenType exceptionType)
    {
        // 允许存在的分隔符情况
        if (Peek().Type == TokenType.COMMA)
        {
            current++; // 消费逗号
            return;
        }

        // 必须满足结束符条件
        if (Peek().Type != exceptionType)
            throw new ArgumentException($"在token {Peek().Type} 前缺少逗号分隔符");
    }

    // -------------------------------------
    // 6.4 查看当前token（不移动指针）
    // -------------------------------------
    private Token Peek()
    {
        if (current >= tokens.Count)
            return new Token(TokenType.EOF, null); // 返回虚拟结束符

        return tokens[current]; // 返回当前token
    }
}
