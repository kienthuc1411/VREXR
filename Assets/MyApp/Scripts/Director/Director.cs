using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JSAM;

public class Director : MonoBehaviour
{
    #region Variables
    [Header("Buttons")]
    public Button showResultConvoLogButton;

    [Header("Panel Sections")]
    public GameObject conversationLog;
    public GameObject conversationLogSection;
    public GameObject filterPanel;
    public GameObject loadingSceneSection; // Loading section appeared when load another scene
    public GameObject loadingSection;
    public GameObject modeSelectSection;
    public GameObject NPCSpeechSection;
    public GameObject playerSpeechSection;
    public GameObject playSection;
    public GameObject openingSection;
    public GameObject resultSection;
    public GameObject resultSectionButton;
    public GameObject resultFinalRank;
    public GameObject scenarioSelectSection;
    public GameObject soundBar;
    public GameObject speechToTextSection;

    [Header("Scripts-Classes")]
    public GameObject NPCSpeaker;
    public GameObject PlayerSpeaker;
    public GameObject ScenarioSelect;
    private NPCSpeak npcSpeaker;
    private PlayerSpeak playerSpeaker;
    private Request request;
    private Request requestCreateRecord;
    private Request requestCreateRecordDetail;
    private URLAPI urlAPI = new URLAPI();

    [Header("Sprites")]
    public Sprite sprConvoLogShowBtnActive;
    public Sprite sprConvoLogShowBtnDisable;
    public List<Sprite> rankSprites = new List<Sprite>();

    [Header("Texts")]
    public Text playerText;
    public Text npcText;
    public Text speechToText;
    public Text resultTotalPlaytime;
    public Text resultAccuracy;
    public Text resultCashEarned;

    // Other Variables
    // Public
    [Header("Other Variables")]
    [HideInInspector]
    public float speechPoint;
    [HideInInspector]
    public float totalPoint;
    [HideInInspector]
    public int conversationLength = 0;
    [HideInInspector]
    public int timeWaiting;
    [HideInInspector]
    public ReceivePlayerHistory receivePlayerHistory;
    [HideInInspector]
    public List<ScenarioDetail> listConversation = new List<ScenarioDetail>();

    public SpeechManager speechManager;

    // Private
    [Header("Private Variables")]
    [SerializeField]
    private Color color_0056D7;
    [SerializeField]
    private Color color_333333;
    private bool isPaused = false;
    private bool isStoppedGame = false;
    private bool isShowResultConvoLog = false;
    private System.Diagnostics.Stopwatch watch;

    // Difficulty Selection
    enum Difficulty
    {
        beginner, //0.4f
        intermediate, //0.6f
        advanced, //0.8f
        expert //1.0f
    }
    Difficulty currentDifficulty = Difficulty.beginner;
    private float difficultyConfidence = 0.4f;
    #endregion

    #region Unity Methods
    void Start()
    {
        SetupNewGame();
    }
    #endregion

    #region Methods
    private void SetupNewGame()
    {
        // Sections
        playSection.SetActive(false);
        conversationLogSection.SetActive(false);
        filterPanel.SetActive(false);
        loadingSceneSection.SetActive(false);
        loadingSection.SetActive(false);
        modeSelectSection.SetActive(true);
        NPCSpeechSection.SetActive(false);
        openingSection.SetActive(false);
        playerSpeechSection.SetActive(false);
        resultSection.SetActive(false);
        resultSectionButton.SetActive(false);
        scenarioSelectSection.SetActive(false);
        soundBar.SetActive(false);

        // Speakers
        npcSpeaker = NPCSpeaker.GetComponent<NPCSpeak>();
        playerSpeaker = PlayerSpeaker.GetComponent<PlayerSpeak>();
        listConversation = new List<ScenarioDetail>();
        // Points
        totalPoint = 0;
        speechPoint = 0;

        // Difficulty
        switch (currentDifficulty)
        {
            case Difficulty.beginner:
                difficultyConfidence = 0.4f;
                break;
            case Difficulty.intermediate:
                difficultyConfidence = 0.6f;
                break;
            case Difficulty.advanced:
                difficultyConfidence = 0.8f;
                break;
            case Difficulty.expert:
                difficultyConfidence = 1.0f;
                break;
            default:
                break;
        }

        // Stopwatch for Playtime count
        watch = new System.Diagnostics.Stopwatch();
    }

    public void repeatAgain()
    {
        playerText.gameObject.SetActive(true);

        playerSpeaker.Speaking();
    }
    
    public JsonDataDetail convertStringToData()
    {
        string data = this.request.receiveData;
        JsonDataDetail jsonData = JsonUtility.FromJson<JsonDataDetail>(data);
        data = this.requestCreateRecord.receiveData;
        int check = data.IndexOf("We're sorry, but something went wrong");
        if (data.Length > 0)
        {
            receivePlayerHistory = JsonUtility.FromJson<ReceivePlayerHistory>(data);
            this.requestCreateRecord.receiveData = "";
        }
        return jsonData;
    }
    
    void StartConversation(int indexToStart = 0)
    {
        JsonDataDetail data = convertStringToData();
        conversationLength = data.data.Count;
        listConversation.AddRange(data.data); 
    }

    private void StartRequesting()
    {
        request = new Request();
        requestCreateRecord = new Request();
        requestCreateRecordDetail = new Request();

        //OptionModel optionModel = new OptionModel(27, "easy"); // script for testing
        OptionModel optionModel = new OptionModel(SelectScenarioFromServer.Id, "easy");

        PlayerInfo playerInfo = new PlayerInfo(5);
        PlayerHistory playerHistory = new PlayerHistory(playerInfo.id, GetScenarioFromServer.Id);

        // get Speech script 
        string url = urlAPI.URL_CMS + "/" + urlAPI.endpointPostOptionCMS;
        string data = JsonUtility.ToJson(optionModel);
        StartCoroutine(request.request(url, data, "POST"));
        // post data create record user 
        // test url cms http://192.168.1.30:3000/
        // = urlAPI.URL_CMS + "/" + urlAPI.endpointCreatePlayerRecord;
        url = urlAPI.URL_CMS + "/" + urlAPI.endpointCreatePlayerRecord;

        data = JsonUtility.ToJson(playerHistory);
        StartCoroutine(requestCreateRecord.request(url, data, "POST"));
    }
    
    public void ConversationFinished(int totalScore, int countSpeechPlayer, int player_history_id)
    {
        StartCoroutine(FinishScenario(totalScore, countSpeechPlayer, player_history_id));
    }

    private IEnumerator FinishScenario(int totalScore, int countSpeechPlayer, int player_history_id)
    {
        // Start Loading
        soundBar.SetActive(false);
        playSection.SetActive(false);
        conversationLogSection.SetActive(false);
        speechToTextSection.SetActive(false);
        loadingSection.SetActive(true);
        yield return new WaitForSeconds(8);

        // Calculate final score
        totalScore = (int)(totalScore / countSpeechPlayer);
        FinalScore.Score = totalScore;
        string url = this.urlAPI.URL_CMS + "/" + this.urlAPI.endpointCreatePlayerRecord + "/" + player_history_id;
        TotalPoint postScoreToCms = new TotalPoint(totalScore);
        string dataPOST = JsonUtility.ToJson(postScoreToCms);
        StartCoroutine(requestCreateRecordDetail.request(url, dataPOST, "PUT"));

        // Finish loading
        loadingSection.SetActive(false);

        // Display results
        // Playtime
        watch.Stop();
        TimeSpan ts = watch.Elapsed;
        resultTotalPlaytime.text = ts.Minutes.ToString() + "分" + ts.Seconds.ToString() + "秒";
        // Accuracy
        resultAccuracy.text = totalScore.ToString() + "%";
        // Rank
        // 0~20=D,21~40=C,41~60=B
        // 61~80 = A,81~100 = S
        if (totalScore > 0 && totalScore <= 20) // D
        {
            resultFinalRank.GetComponent<Image>().sprite = rankSprites[0];
        }
        else if (totalScore > 20 && totalScore <= 40) // C
        {
            resultFinalRank.GetComponent<Image>().sprite = rankSprites[1];
        }
        else if (totalScore > 40 && totalScore <= 60) // B
        {
            resultFinalRank.GetComponent<Image>().sprite = rankSprites[2];
        }
        else if (totalScore > 60 && totalScore <= 80) // A
        {
            resultFinalRank.GetComponent<Image>().sprite = rankSprites[3];
        }
        else if (totalScore > 80 && totalScore <= 100) // S
        {
            resultFinalRank.GetComponent<Image>().sprite = rankSprites[4];
        }

        resultSection.SetActive(true);
        resultSectionButton.SetActive(true);
    }
    #endregion

    #region Buttons
    public void ClickBackToModeSelection(GameObject invoker)
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);
        speechToTextSection.SetActive(false);//ThaoEm
        SetupNewGame();
        invoker.SetActive(false);
        modeSelectSection.SetActive(true);

        // Clear mode selection
        modeSelectSection.GetComponent<ModeSelectScene>().isLockSelection = false;
        modeSelectSection.GetComponent<ModeSelectScene>().currentMode = ModeSelectScene.Mode.none;


        // Clear log
        foreach (Transform child in conversationLog.transform)
        {
            Destroy(child.gameObject);
        }

        if (invoker.name == "RestartGame")
        {
            invoker.SetActive(true);
        }
    }

    public void ClickBacktoScenarioSelect()
    {
        scenarioSelectSection.SetActive(true);
        NPCSpeechSection.SetActive(false);
        playerSpeechSection.SetActive(false);
        resultSection.SetActive(false);
        resultSectionButton.SetActive(false);
        speechToTextSection.SetActive(false);//ThaoEm
        conversationLogSection.SetActive(false); //ThaoEm
        foreach (Transform child in conversationLog.transform)//ThaoEm
        {
            Destroy(child.gameObject);
        }
        listConversation = new List<ScenarioDetail>();//ThaoEm
    }

    // Load City scene
    public void ClickMoveToCityScene(GameObject invokeGameObject)
    {
        ClickMoveToScene(invokeGameObject, "CityScene");
    }


    // Load a scene
    public void ClickMoveToScene(GameObject invokeGameObject, string scene)
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);

        switch (invokeGameObject.name)
        {
            case "ScenarioSelectPanel":
                scenarioSelectSection.SetActive(false);
                break;
            case "ModeSelectPanel":
                modeSelectSection.SetActive(false);
                break;
            case "ResultSection":
                resultSection.SetActive(false);
                resultSectionButton.SetActive(false);
                conversationLog.SetActive(false);
                break;
            default:
                break;
        }

        StartCoroutine(LoadYourAsyncScene(scene));
    }

    private IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        // Show Loading section
        loadingSceneSection.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        loadingSceneSection.SetActive(false);
    }

    public void ClickReplayScenario()
    {
        speechManager.convoLogs = new List<GameObject>();
        AudioManager.PlaySound(Sounds.Click);
        // Clear conversation log
        foreach (Transform child in conversationLog.transform)
        {
            Destroy(child.gameObject);
        }
        //setnewlistConlog


        SetupNewGame();
        modeSelectSection.SetActive(false);
        StartRequesting();
        
        playSection.SetActive(true);
        NPCSpeechSection.SetActive(false);
        playerSpeechSection.SetActive(false);
        conversationLogSection.SetActive(false);
        speechToTextSection.SetActive(false);
        soundBar.SetActive(false);

        openingSection.SetActive(true);
    }

    public void ClickStopMiddle(bool isMiddle)
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);

        NPCSpeechSection.SetActive(false);
        playerSpeechSection.SetActive(false);

        ClickStop();
    }

    public void ClickStart()
    {
        AudioManager.PlaySound(Sounds.Sta);
        isPaused = false;
        openingSection.SetActive(false);
        NPCSpeechSection.SetActive(true);
        playerSpeechSection.SetActive(true);
        conversationLogSection.SetActive(true);
        soundBar.SetActive(true);

        // Start counting playtime
        watch.Start();

        StartConversation(0);
    }

    public void ClickStop()
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);
        playerSpeaker.isReceivedData = false;
        isPaused = true; // lam nut dung luon nhung co them phan kiem tra diem so xem co du de pass qua hay khong
        isStoppedGame = true; // lam nut dung
    }

    // Start game with the current selected scenario
    public void ClickStartScenario()
    {
        AudioManager.PlaySound(Sounds.ClickButtonStart);
        StartRequesting();
        scenarioSelectSection.SetActive(false);
        playSection.SetActive(true);
        openingSection.SetActive(true);
    }

    public void ClickShowFinalConversationLog()
    {
        AudioManager.PlaySound(Sounds.Click);
        if (!isShowResultConvoLog)
        {
            conversationLogSection.SetActive(true);
            isShowResultConvoLog = true;
            showResultConvoLogButton.GetComponent<Image>().sprite = sprConvoLogShowBtnDisable;
            showResultConvoLogButton.transform.GetChild(0).GetComponent<Text>().color = color_333333;
        }
        else
        {
            conversationLogSection.SetActive(false);
            isShowResultConvoLog = false;
            showResultConvoLogButton.GetComponent<Image>().sprite = sprConvoLogShowBtnActive;
            showResultConvoLogButton.transform.GetChild(0).GetComponent<Text>().color = color_0056D7;
        }
    }

    public async void ClickLogOut()
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);
        long logoutResponseCode = await Request.Logout();
        ProcessLogoutResult(logoutResponseCode);
    }

    void ProcessLogoutResult(long statusCode)
    {
        // 200 = OK!
        if (statusCode == 200)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("Logout failed! Response code: " + statusCode);
        }
    }

    #endregion

    #region ThaoEm
    public void stopeNPCSpeach()
    {
        GameObject obj = GameObject.Find("One shot audio");
        obj?.SetActive(false);

    }
    #endregion
}