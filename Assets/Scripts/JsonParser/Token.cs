// ========================
// 1. �ʷ���Ԫ���Ͷ���
// ========================
public enum TokenType
{
    STRING,     // �ַ������� "abc"
    NUMBER,     // �������� 123 �� 45.67
    BOOLEAN,    // ����ֵ true/false
    NULL,       // ��ֵ null
    LEFT_BRACE,    // ������ {
    RIGHT_BRACE,   // �һ����� }
    LEFT_BRACKET,  // ������ [
    RIGHT_BRACKET, // �ҷ����� ]
    COMMA,      // ���� ,
    COLON,      // ð�� :
    EOF         // ����������
}

// ========================
// 2. �ʷ���Ԫ���ݽṹ
// ========================
public class Token
{
    public TokenType Type { get; }  // �ʷ���Ԫ����
    public object Value { get; }    // ��Ӧ��ֵ���ַ��������֡�����ֵ�ȣ�

    // ���캯������ʼ�����ͺ�ֵ
    public Token(TokenType type, object value)
    {
        Type = type;
        Value = value;
    }
}

