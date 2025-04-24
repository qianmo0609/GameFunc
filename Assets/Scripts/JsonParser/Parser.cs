// ===========================================
// 1. ������������
// ===========================================
using System.Collections.Generic;
using System;

public class Parser
{
    // -------------------------------------
    // 1.1 ��Ա����
    // -------------------------------------
    private readonly List<Token> tokens; // �ʷ���Ԫ�б�
    private int current;                 // ��ǰ����λ������

    // -------------------------------------
    // 1.2 ���캯��
    // -------------------------------------
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        current = 0; // ��ʼ����ǰλ��
    }

    // ===========================================
    // 2. ������ڷ���
    // ===========================================
    public object Parse()
    {
        Token token = Advance();      // ��ȡ��һ����Чtoken
        return ParseFromToken(token); // �����ݹ����
    }

    // ===========================================
    // 3. �ݹ���������߼�
    // ===========================================
    private object ParseFromToken(Token token)
    {
        switch (token.Type)
        {
            // ��������ֱ�ӷ���ֵ
            case TokenType.STRING:
            case TokenType.NUMBER:
            case TokenType.BOOLEAN:
            case TokenType.NULL:
                return token.Value;

            // �������ͽ���
            case TokenType.LEFT_BRACE:
                return ParseObject(); // ��������������

            // �������ͽ���
            case TokenType.LEFT_BRACKET:
                return ParseArray(); // ���������������

            default:
                throw new ArgumentException($"�Ƿ���ʼtoken����: {token.Type}", nameof(token));
        }
    }

    // ===========================================
    // 4. JSON����������ֵ����ͣ�
    // ===========================================
    private Dictionary<string, object> ParseObject()
    {
        var jsonObject = new Dictionary<string, object>();
        Token keyToken = Advance(); // ��ȡ��һ��key��token

        // ѭ�������ֵ�ԣ�ֱ�������Ҵ�����
        while (keyToken.Type != TokenType.RIGHT_BRACE)
        {
            // �߽��飺��ֹδ�պ϶���
            if (keyToken.Type == TokenType.EOF)
                throw new ArgumentException("JSON����δ��ȷ�պ�");

            // ���������ַ�������
            if (keyToken.Type != TokenType.STRING)
                throw new ArgumentException($"������������ַ������ͣ�ʵ������Ϊ: {keyToken.Type}");

            // �����ֵ�Խṹ
            Consume(TokenType.COLON, "��ֵ��ȱ��ð�ŷָ���"); // ��֤ð��
            Token valueToken = Advance();                     // ��ȡֵtoken
            jsonObject[(string)keyToken.Value] = ParseFromToken(valueToken); // �ݹ����ֵ

            // �����Ż������
            ConsumeCommaUnless(TokenType.RIGHT_BRACE);
            keyToken = Advance(); // ��ȡ��һ�����������
        }

        return jsonObject;
    }

    // ===========================================
    // 5. JSON����������б����ͣ�
    // ===========================================
    private List<object> ParseArray()
    {
        var jsonArray = new List<object>();
        Token token = Advance(); // ��ȡ��һ��Ԫ��token

        // ѭ������Ԫ�أ�ֱ�������ҷ�����
        while (token.Type != TokenType.RIGHT_BRACKET)
        {
            // �߽��飺��ֹδ�պ�����
            if (token.Type == TokenType.EOF)
                throw new ArgumentException("JSON����δ��ȷ�պ�");

            jsonArray.Add(ParseFromToken(token)); // �ݹ����Ԫ��ֵ
            ConsumeCommaUnless(TokenType.RIGHT_BRACKET); // ����ָ���
            token = Advance(); // ��ȡ��һ��Ԫ�ػ������
        }

        return jsonArray;
    }

    // ===========================================
    // 6. ���߷���
    // ===========================================

    // -------------------------------------
    // 6.1 ǰ������һ��token
    // -------------------------------------
    private Token Advance()
    {
        if (current >= tokens.Count)
            throw new IndexOutOfRangeException("�����token����ֹ");

        return tokens[current++]; // ���ص�ǰtoken���ƶ�ָ��
    }

    // -------------------------------------
    // 6.2 ��֤������ָ������token
    // -------------------------------------
    private void Consume(TokenType expectedType, string errorMessage)
    {
        if (Peek().Type != expectedType)
            throw new ArgumentException($"{errorMessage}. Ԥ������: {expectedType}, ʵ������: {Peek().Type}");

        current++; // ��֤ͨ�����ƶ�ָ��
    }

    // -------------------------------------
    // 6.3 �����ŷָ��߼�
    // -------------------------------------
    private void ConsumeCommaUnless(TokenType exceptionType)
    {
        // ������ڵķָ������
        if (Peek().Type == TokenType.COMMA)
        {
            current++; // ���Ѷ���
            return;
        }

        // �����������������
        if (Peek().Type != exceptionType)
            throw new ArgumentException($"��token {Peek().Type} ǰȱ�ٶ��ŷָ���");
    }

    // -------------------------------------
    // 6.4 �鿴��ǰtoken�����ƶ�ָ�룩
    // -------------------------------------
    private Token Peek()
    {
        if (current >= tokens.Count)
            return new Token(TokenType.EOF, null); // �������������

        return tokens[current]; // ���ص�ǰtoken
    }
}
