using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript
{
    public string model;
    public string speech;
    public bool isPass;
    public bool isFinish;
    public float timeoutSpeech;
    public PlayerScript(string model, string speech, float timeoutSpeech)
    {
        this.model = model;
        this.speech = speech;
        this.timeoutSpeech = timeoutSpeech;
        this.isPass = false;
        this.isFinish = false;
    }
    public void Speak()
    {
        
    }
    

}
