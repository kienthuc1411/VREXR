using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSpeak:MonoBehaviour
{

    public string getDataDemo;

    public string speech;
    public string[] keyWord;
    public float confident;
    public string result;
    public bool isPostData = false;
    public bool isPause = false;
    public bool NPCisSpeaking = true;
    public bool isSpeaking = false;
    public bool isReceivedData = false;
    public AudioSource audioSource;
    public int timeToStop;
    private URLAPI urlApi = new URLAPI();
    private Request request = new Request();
    private string dataBase64;
    public bool isFinish = false;

    private void Update()
    {
        bool check = Microphone.IsRecording(null);
        if (check==false && isSpeaking==true)
        {
            Debug.Log("save file wav");
            isSpeaking = false;
            Save();
            GetScore();
        }
        if (request.isPostDataSuccess)
        {
            request.isPostDataSuccess = false;
            this.isPostData = true ;
        }
        if (this.isPostData)
        {
            getDataDemo= getDataFromServer();
            this.isPostData = false;
            this.isReceivedData = false;
        }

    }
    public void Speaking()
    {
        if (isPause == false)
        {
            Debug.Log("Speaking");
            audioSource = GetComponent<AudioSource>();
            isSpeaking = true;
            audioSource.clip = Microphone.Start(null, false, this.timeToStop, 16000);
        }
    }

    public void Save()
    {
        dataBase64 = SavWav.Save("myfile", audioSource.clip);
        //folderSrc = SavWav.folderSrc;
        isSpeaking = false; 
    }
    public void StopRecord()
    {
        Debug.LogWarning("StopRecord");
        Microphone.End(null);
        Save();
        GetScore();
    }

    public void GetScore()
    {
        this.isPostData = false;
        string url = urlApi.URL_AI + "/" + urlApi.endpointPostScriptAI;
        ScoreAI scoreAI = new ScoreAI(this.dataBase64, this.speech, this.keyWord, this.confident);
        string dataJson = JsonUtility.ToJson(scoreAI);
        dataJson = dataJson.Replace("originalText", "original-text").Replace("keyWord", "key-word");
        Debug.Log("61 "+ dataJson);
        StartCoroutine(request.request(url, dataJson, "POST"));
    }
    public string getDataFromServer()
    {
        
        Debug.Log("isPostData " + request.receiveData);
        result = request.receiveData;
        isReceivedData = true;
        return result;
    }

}
