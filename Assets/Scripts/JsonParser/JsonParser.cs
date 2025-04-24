using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonParser : MonoBehaviour
{
    string jsonString1 = @"{""name"": ""张三"",""age"": 28,""is_student"": false,""hobbies"": [""阅读"", ""游泳""],""address"": {""street"": ""人民路123号"",""city"": ""北京""}}";
    string jsonString2 = "{\"name\": \"张三\",\"age\": 28,\"is_student\": false,\"hobbies\": [\"阅读\", \"游泳\"],\"address\": {\"street\": \"人民路123号\",\"city\": \"北京\"}}";
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
                Debug.Log($"字符串值: {s}");
                break;
            case double d:
                Debug.Log($"数字值: {d}");
                break;
            case bool b:
                Debug.Log($"布尔值: {b}");
                break;
            case null:
                Debug.Log("空值");
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
