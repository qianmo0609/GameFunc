using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -------------------- 词法分析器部分 --------------------
public enum XMLTokenType
{
    TagOpen,         // 开始标签 <element
    TagClose,        // 闭合标签 </element
    SelfClosingTag,  // 自闭合标签 <element/>
    AttributeName,   // 属性名 name
    Equals,          // 等号 =
    AttributeValue,  // 属性值 "value"
    Text,            // 文本内容
    EndOfFile,       // 文件结束
    Error            // 错误标记
}

public class XMLToken
{
    public XMLTokenType Type { get; }
    public string Value { get; }
    public int Position { get; }  // 用于错误定位

    public XMLToken(XMLTokenType type, string value = null, int pos = -1)
    {
        Type = type;
        Value = value;
        Position = pos;
    }
}
