// ========================
// 1. 词法单元类型定义
// ========================
public enum TokenType
{
    STRING,     // 字符串类型 "abc"
    NUMBER,     // 数字类型 123 或 45.67
    BOOLEAN,    // 布尔值 true/false
    NULL,       // 空值 null
    LEFT_BRACE,    // 左花括号 {
    RIGHT_BRACE,   // 右花括号 }
    LEFT_BRACKET,  // 左方括号 [
    RIGHT_BRACKET, // 右方括号 ]
    COMMA,      // 逗号 ,
    COLON,      // 冒号 :
    EOF         // 输入结束标记
}

// ========================
// 2. 词法单元数据结构
// ========================
public class Token
{
    public TokenType Type { get; }  // 词法单元类型
    public object Value { get; }    // 对应的值（字符串、数字、布尔值等）

    // 构造函数：初始化类型和值
    public Token(TokenType type, object value)
    {
        Type = type;
        Value = value;
    }
}

