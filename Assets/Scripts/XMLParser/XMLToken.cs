using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -------------------- �ʷ����������� --------------------
public enum XMLTokenType
{
    TagOpen,         // ��ʼ��ǩ <element
    TagClose,        // �պϱ�ǩ </element
    SelfClosingTag,  // �Ապϱ�ǩ <element/>
    AttributeName,   // ������ name
    Equals,          // �Ⱥ� =
    AttributeValue,  // ����ֵ "value"
    Text,            // �ı�����
    EndOfFile,       // �ļ�����
    Error            // ������
}

public class XMLToken
{
    public XMLTokenType Type { get; }
    public string Value { get; }
    public int Position { get; }  // ���ڴ���λ

    public XMLToken(XMLTokenType type, string value = null, int pos = -1)
    {
        Type = type;
        Value = value;
        Position = pos;
    }
}
