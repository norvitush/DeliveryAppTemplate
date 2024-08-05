using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class FormatEmoji : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _output;
    [SerializeField] bool needRemoveEmoji = false;
    public void FormatInput(string input)
    {
        _output.text = ToUTF32FromPair(input);
    }
    public string ToUTF32FromPair(string input)
    {
        var output = input;

        Regex pattern = new Regex(@"\\u[a-zA-Z0-9]*\\u[a-zA-Z0-9]*");

        while (output.Contains(@"\u"))
        {
            output = pattern.Replace(output,
                m => {
                    var pair = m.Value;
                    var first = pair.Substring(0, 6);
                    var second = pair.Substring(6, 6);
                    var firstInt = Convert.ToInt32(first.Substring(2), 16);
                    var secondInt = Convert.ToInt32(second.Substring(2), 16);
                    var codePoint = (firstInt - 0xD800) * 0x400 + (secondInt - 0xDC00) + 0x10000;
                    return needRemoveEmoji ? string.Empty : @"\U" + codePoint.ToString("X8");
                },
                1
            );
        }

        return output;
    }
}
