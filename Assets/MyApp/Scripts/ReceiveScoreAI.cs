using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReceiveScoreAI 
{
    public Response response;
}
[Serializable]
public class Response
{
    public float Confidence;
    public string[] Keyword_appear;
    public string[] Miss_word;
    public string Sentences;
    public string[] True_word;
    public Dictionary<string, string> Word_error; 
}
