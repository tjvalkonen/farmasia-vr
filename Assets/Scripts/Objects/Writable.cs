﻿using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class Writable : WritingTarget {

    // Displays the text, must have a TextMeshPro component
    [SerializeField]
    private GameObject textObject;

    [SerializeField]
    public int MaxLines = 4;
    
    [SerializeField]
    private TextMeshPro textField;

    [SerializeField]
    public bool isAgar = false;

    public string Text {
        get { return textField.GetParsedText(); }
    }

    public Dictionary<WritingType, string> WrittenLines = new Dictionary<WritingType, string>();

    public void AddWrittenLines(Dictionary<WritingType, string> options) {
        foreach(var option in options)
        {
            WrittenLines.Add(option.Key, option.Value);
        }
        string resultText = "";
        int n = 0;
        foreach(string line in WrittenLines.Values)
        {
            resultText += line + '\n';
            n++;
            if (n == 2 && isAgar) {
                resultText += '\n';
                resultText += '\n';
                resultText += '\n';
                resultText += '\n';
                resultText += '\n';
            }
        }
        textField.SetText(resultText);
    }


    void Start() {
        if (textField == null) {
            Logger.Warning("Writable '" + gameObject.ToString() + "' does not have a valid textObject attached");
        }
    }

    public override Writable GetWritable() {
        return this;
    }
}
