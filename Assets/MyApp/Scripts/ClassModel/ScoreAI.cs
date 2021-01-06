using System.Collections;
using System.Collections.Generic;
//using System.Text.Json.Serialization;
using UnityEngine;

public class ScoreAI
{
    public string speech;
    public string originalText;
    public string[] keyWord;
    public float confident;

    public ScoreAI(string speech, string originalText,string[] keyWord, float confident)
    {
        this.speech = speech;
        this.originalText = originalText;
        this.keyWord = keyWord;
        this.confident = confident;
    }
}
