using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpeakIm : MonoBehaviour
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
    public GameObject STTHandler;
    private AudioSource audioSource;
    public int timeToStop;
    private URLAPI urlApi = new URLAPI();
    private Request request = new Request();
    public string dataBase64;
    public bool isFinish = false;

    private void Start()
    {
        audioSource = STTHandler.GetComponent<AudioSource> ();
    }
    private void Update()
    {
        bool check = Microphone.IsRecording(null);
        if (check == false && isSpeaking == true)
        {
            Debug.Log("save file wav");
            isSpeaking = false;
            Save();
            GetScore();
        }
        if (request.isPostDataSuccess)
        {
            request.isPostDataSuccess = false;
            this.isPostData = true;
        }
        if (this.isPostData)
        {
            this.isPostData = false;
            this.isReceivedData = false;
            getDataDemo = getDataFromServer();

        }

    }
    public void Speaking()
    {
        if (isPause == false)
        {
            Debug.Log("Speaking");
           // audioSource = GetComponent<AudioSource>();
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
        bool check = Microphone.IsRecording(null);
        Debug.LogWarning(check);
        if (check == false && isSpeaking == true)
        {
            Debug.Log("save file wav");
            isSpeaking = false;
            Save();
            GetScore();
        }
    }

    public void GetScore()
    {
        this.isPostData = false;
        string url = urlApi.URL_AI + "/" + urlApi.endpointPostScriptAI;
        ScoreAI scoreAI = new ScoreAI(this.dataBase64, this.speech, this.keyWord, this.confident);
        string dataJson = JsonUtility.ToJson(scoreAI);
        dataJson = dataJson.Replace("originalText", "original-text").Replace("keyWord", "key-word");
        Debug.Log("61 " + dataJson);
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
