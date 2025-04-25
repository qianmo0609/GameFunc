using System;
using UnityEngine;

public class XMLTest : MonoBehaviour
{
    string xml = @"<root>
    <person  id = '123'>
        <name>John</name>
        <age>30</age>
    </person>
</root>"
;
    void Start()
    {
        var parser = new XMLParser();
        var root = parser.Parse(xml);
        Debug.Log($"������ԣ�{root.Children[0].Attributes["id"]}"); // ��� "123"
    }
}
