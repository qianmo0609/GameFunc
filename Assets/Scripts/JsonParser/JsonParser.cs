using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonParser : MonoBehaviour
{
    string jsonString1 = @"{""name"": ""����"",""age"": 28,""is_student"": false,""hobbies"": [""�Ķ�"", ""��Ӿ""],""address"": {""street"": ""����·123��"",""city"": ""����""}}";
    string jsonString2 = "{\"name\": \"����\",\"age\": 28,\"is_student\": false,\"hobbies\": [\"�Ķ�\", \"��Ӿ\"],\"address\": {\"street\": \"����·123��\",\"city\": \"����\"}}";
    // Start is called before the first frame update
    void Start()
    {
        var scanner = new Scanner(jsonString1);
        List<Token> tokens = scanner.Scan();
        var parser = new Parser(tokens);
        object parsed = parser.Parse();
        this.TranslateJsonObj(parsed);
    }

    void TranslateJsonObj(object result)
    {
        switch (result)
        {
            case Dictionary<string, object> obj:
                HandleObject(obj);
                break;
            case List<object> arr:
                HandleArray(arr);
                break;
            case string s:
                Debug.Log($"�ַ���ֵ: {s}");
                break;
            case double d:
                Debug.Log($"����ֵ: {d}");
                break;
            case bool b:
                Debug.Log($"����ֵ: {b}");
                break;
            case null:
                Debug.Log("��ֵ");
                break;
        }
    }

    void HandleObject(Dictionary<string, object> obj)
    {
        //if (obj.TryGetValue("name", out var name) && name is string nameStr)
        //{

        //}
        string typeName = "";
        foreach (var item in obj)
        {
            typeName = item.Value.GetType().Name;
            if (typeName.Contains("List`1") || typeName.Contains("Dictionary`2"))
            {
                Debug.Log($"Key: {item.Key}");
                TranslateJsonObj(item.Value);
            }
            else
            {
                Debug.Log($"Key: {item.Key}  Value: {item.Value}");
            }
        }
    }

    void HandleArray(List<object> arr)
    {
        foreach (var item in arr)
        {
            Debug.Log($"object: {item}");
        }
    }
}
