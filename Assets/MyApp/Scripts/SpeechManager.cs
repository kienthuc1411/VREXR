using GoogleCloudStreamingSpeechToText;
using JSAM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SpeechManager : MonoBehaviour
{
    #region Variables
    // Constants
    private const string npc_log_mode = "NPC_LOG";
    private const string npc_log_with_hint_mode = "NPC_HINT_LOG";
    private const string player_log_mode = "PLAYER_LOG";
    private const string player_log_with_hint_mode = "PLAYER_HINT_LOG";
    private const string hint_life_mode = "LIFE_HINT";
    private const string hint_language_mode = "LANGUAGE_HINT";
    private const string hint_life_title = "生活のヒント";
    private const string hint_language_title = "言語のヒント";

    [Header("Change language buttons")]
    public Button changeLanguageJap;
    public Button changeLanguageRmj;
    public Button changeLanguageVie;

    [Header("Conversations")]
    [HideInInspector]
    public List<ScenarioDetail> listConversation;
    [HideInInspector]
    public ScenarioDetail currentConversation;
    [HideInInspector]
    public ScenarioDetail nextConversation;
    [HideInInspector]
    public ScenarioDetail lastConversation;

    [Header("Gameobject")]
    public GameObject conversationLog;
    public GameObject gameDirector;
    public GameObject getSTT;
    public GameObject hintSection;
    public GameObject loadingTextDuringPlay;
    public GameObject NPCSpeakPrefab;
    public GameObject PlayerSpeakModel;
    public GameObject scoreRank;
    public GameObject soundBar;
    public GameObject STTHandler;
    [HideInInspector]
    public List<GameObject> convoLogs = new List<GameObject>();

    [Header("Sections")]
    public GameObject conversationLogSection;
    public GameObject npcSpeechSection;
    public GameObject speechToTextSection;
    public GameObject resultSection;

    [Header("Other variables")]
    [HideInInspector]
    public bool isRealVoice = false;
    public ScrollRect convoLogScrollRect;
    public Image viewPortScrollRect;
    public float timeAnimationLoadScore = 0.25f;
    [HideInInspector]
    public int nextIndex = 0;
    [HideInInspector]
    public int stopIndex = 0;
    private playerSpeakIm playerSpeak;
    private GetSpeechToText getSpeechToText;
    private STTHandler sttHandler;
    private Director director;
    private bool autoScrollToBottom;
    private bool isPlayerPassed = false;
    private int conversationLength;
    private int player_history_id;
    private NPCSpeak npcSpeak;
    private URLAPI urlAPI = new URLAPI();
    private Request requestCreateRecordDetail = new Request();
    private float minT = 0f;
    private float maxT = 1f;
    private float t = 1f;

    [Header("Prefabs")]
    public GameObject baseNPCConvoLog;
    [Tooltip("NPC Conversation Log prefab but with Buttons section.")]
    public GameObject baseNPCConvoLogWithBtn;
    public GameObject basePlayerConvoLog;
    [Tooltip("Player Conversation Log prefab but with Buttons section.")]
    public GameObject basePlayerConvoLogWithBtn;

    [Header("Record button")]
    public Sprite pauseSprite;
    public Sprite microphoneSprite;
    public Button recordButton;
    public Image loadingImg;
    private float waitTime = 60.0f;
    private int totalScore = 0;
    private int countSpeechPlayer = 0;
    private bool isNpcRepeat = false;

    [Header("RepeatNPC")]
    public Button repeatNpcSpeakButton;
    private string lastNPCSpeech = "";

    [Header("Sprites")]
    // Hints
    public Sprite sprHintLife;
    public Sprite sprHintLanguage;
    // Rank animations
    public Sprite sprRankS;
    public Sprite sprRankA;
    public Sprite sprRankB;
    public Sprite sprRankC;
    public Sprite sprRankD;
    // Convolog with ranks
    public Sprite sprConvoLogRankS;
    public Sprite sprConvoLogRankA;
    public Sprite sprConvoLogRankB;
    public Sprite sprConvoLogRankC;
    public Sprite sprConvoLogRankD;

    [Header("Text")]
    public Text npcText;
    public Text playerText;
    public Text speechToText;
    public Text debugText;
    [HideInInspector]
    public List<string> npcConvoList = new List<string>();
    [HideInInspector]
    public List<string> playerConvoList = new List<string>();

    // Language Selection
    private enum Lang
    {
        Jap,
        Rmj,
        Vie
    }
    Lang currentLang = Lang.Jap;
    #endregion

    #region UnityMethod
    void Start()
    {
        getSTT.SetActive(false);
        hintSection.SetActive(false);
        scoreRank.SetActive(false);
        loadingTextDuringPlay.SetActive(false);

        sttHandler = STTHandler.GetComponent<STTHandler>();
        getSpeechToText = getSTT.GetComponent<GetSpeechToText>(); //get GetSpeechToText
        playerSpeak = PlayerSpeakModel.GetComponent<playerSpeakIm>();
        director = gameDirector.GetComponent<Director>();
        npcSpeak = NPCSpeakPrefab.GetComponent<NPCSpeak>();
    }
    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SpawnConversationLog("A", baseNPCConvoLogWithBtn);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    SpawnConversationLog("A", basePlayerConvoLogWithBtn);
        //}

        // Auto scroll convoLog to bottom
        // Scroll only works in Clamped mode. Have to return to Elastic manually
        if (autoScrollToBottom)
        {
            if (convoLogScrollRect.movementType != ScrollRect.MovementType.Clamped)
            {
                convoLogScrollRect.movementType = ScrollRect.MovementType.Clamped;
            }
            conversationLog.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 20 * Time.deltaTime);
        }
        else
        {
            if (convoLogScrollRect.movementType != ScrollRect.MovementType.Elastic)
            {
                convoLogScrollRect.movementType = ScrollRect.MovementType.Elastic;
            }
        }

        if (director.conversationLength > 0)
        {
            player_history_id = director.receivePlayerHistory.player_history_id;
            conversationLength = director.conversationLength;
            listConversation = director.listConversation;
            director.conversationLength = 0;
            StartConversation(0);
        }

        if (getSpeechToText.isFinalResult) // neu ma user ngung noi 
        {
            // thong bao ngung thu am
            getSpeechToText.isFinalResult = false;
            playerSpeak.speech = currentConversation.detail_ja;
            //playerSpeak.StopRecord();
            // StartConversation(stopIndex);
        }

        if (sttHandler.dataBase64.Length > 0)
        {
            debugText.text = sttHandler.dataBase64;
            //moi sua dong 165
            sttHandler.isReadyToTranscribe = false; // de cho khong post data 2 lan 
            playerSpeak.dataBase64 = sttHandler.dataBase64;
            playerText.text = playerSpeak.dataBase64; // moi them de t
            loadingTextDuringPlay.SetActive(true);
            playerSpeak.GetScore();
            sttHandler.dataBase64 = "";
        }

        if(playerSpeak.isReceivedData)
        {
            loadingTextDuringPlay.SetActive(false);
            Debug.Log("result " + playerSpeak.getDataDemo);
            speechToText.text = playerSpeak.getDataDemo;
            int check = playerSpeak.getDataDemo.IndexOf("Confidence");
            playerSpeak.isReceivedData = false;
            if(check != -1)
            {
                // hien h dang set la cu co confident la qua 
                ReceiveScoreAI data = JsonUtility.FromJson<ReceiveScoreAI>(playerSpeak.result);
                int confidence = (int)(data.response.Confidence * 100);
                if(confidence != 0)
                {
                    sttHandler.StopListening();
                    getSpeechToText.finalResult = "";
                    getSpeechToText.outputInterim = "";
                    getSpeechToText.sttText.text = "";

                    PostScoreToCms postScoreToCms = new PostScoreToCms(player_history_id, currentConversation.id, data.response.Sentences, playerSpeak.dataBase64, confidence);
                    string url = urlAPI.URL_CMS + "/" + urlAPI.endpointCreatePlayerDetail;
                    string dataPOST = JsonUtility.ToJson(postScoreToCms);
                    StartCoroutine(requestCreateRecordDetail.request(url, dataPOST, "POST"));
                    totalScore += confidence;
                    countSpeechPlayer += 1;

                    // Determine the rank based on the score
                    if (confidence > 0 && confidence <= 20) // D
                    {
                        AudioManager.PlaySound(Sounds.Bad);
                        StartCoroutine(LoadAnimationToScore(confidence, sprRankD));
                        // Add to conversation log
                        if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
                        {
                            SpawnConversationLog(player_log_with_hint_mode, basePlayerConvoLogWithBtn, sprConvoLogRankD);
                        }
                        else
                        {
                            SpawnConversationLog(player_log_mode, basePlayerConvoLog);
                        }
                    }
                    else if (confidence > 20 && confidence <= 40) // C
                    {
                        AudioManager.PlaySound(Sounds.OK);
                        StartCoroutine(LoadAnimationToScore(confidence, sprRankC));
                        // Add to conversation log
                        if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
                        {
                            SpawnConversationLog(player_log_with_hint_mode, basePlayerConvoLogWithBtn, sprConvoLogRankC);
                        }
                        else
                        {
                            SpawnConversationLog(player_log_mode, basePlayerConvoLog);
                        }
                    }
                    else if (confidence > 40 && confidence <= 60) // B
                    {
                        AudioManager.PlaySound(Sounds.Good);
                        StartCoroutine(LoadAnimationToScore(confidence, sprRankB));
                        // Add to conversation log
                        if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
                        {
                            SpawnConversationLog(player_log_with_hint_mode, basePlayerConvoLogWithBtn, sprConvoLogRankB);
                        }
                        else
                        {
                            SpawnConversationLog(player_log_mode, basePlayerConvoLog);
                        }
                    }
                    else if (confidence > 60 && confidence <= 80) // A
                    {
                        AudioManager.PlaySound(Sounds.VeryGood);
                        StartCoroutine(LoadAnimationToScore(confidence, sprRankA));
                        // Add to conversation log
                        if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
                        {
                            SpawnConversationLog(player_log_with_hint_mode, basePlayerConvoLogWithBtn, sprConvoLogRankA);
                        }
                        else
                        {
                            SpawnConversationLog(player_log_mode, basePlayerConvoLog);
                        }
                    }
                    else if (confidence > 80 && confidence <= 100) // S
                    {
                        AudioManager.PlaySound(Sounds.Excellent);
                        StartCoroutine(LoadAnimationToScore(confidence, sprRankS));
                        // Add to conversation log
                        if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
                        {
                            SpawnConversationLog(player_log_with_hint_mode, basePlayerConvoLogWithBtn, sprConvoLogRankS);
                        }
                        else
                        {
                            SpawnConversationLog(player_log_mode, basePlayerConvoLog);
                        }
                    }

                    //StartConversation(stopIndex);
                }
                else
                { //trong truong hop nguoi dung noi sai thi se tiep tuc noi
                    sttHandler.isReadyToTranscribe = true;
                }
            }
        }

        if (isPlayerPassed || npcSpeak.isFinish) //neu nguoi dung noi tren>0 hoac npc da noi het cau thi se qua cau tiep theo
        {
            if(!isNpcRepeat)
            {
                npcSpeak.isFinish = false;
                isPlayerPassed = false;
                if (nextIndex < conversationLength)
                {
                    StartConversation(nextIndex);
                }
                else
                {
                    director.ConversationFinished(totalScore, countSpeechPlayer, player_history_id);
                }
            }
            else
            {
                isNpcRepeat = false;
                npcSpeak.isFinish = false;

                StartConversation(stopIndex);
            }
        }
        if (scoreRank.GetComponent<Image>().fillAmount < 1)
        {
            scoreRank.GetComponent<Image>().fillAmount += 1.0f / timeAnimationLoadScore * Time.deltaTime;
        }

        if (sttHandler.isReadyToTranscribe)
        {
            recordButton.GetComponent<Image>().sprite = pauseSprite;
            loadingImg.fillAmount += 1.0f / waitTime * Time.deltaTime;
        }
        else
        {
            recordButton.GetComponent<Image>().sprite = microphoneSprite;
            loadingImg.fillAmount = 0f;
        }

        switch (currentLang)
        {
            case Lang.Jap:
                Color col = changeLanguageJap.GetComponent<Image>().color;
                col.a = 0.2f;
                changeLanguageRmj.GetComponent<Image>().color = col;
                changeLanguageVie.GetComponent<Image>().color = col;
                col.a = 1f;
                changeLanguageJap.GetComponent<Image>().color = col;

                // Change Language Instantly
                if (listConversation.Count > 0)
                {
                    if (currentConversation.model == "npc")
                    {
                        npcText.text = currentConversation.detail_ja;
                    }
                    else
                    {
                        //npcText.text = listConversation[nextIndex - 2].detail_ja; ThaoEm
                       // npcText.text = listConversation[nextIndex - 1].detail_ja;
                        playerText.text = currentConversation.detail_ja;
                    }
                }
                
                break;
            case Lang.Rmj:
                col = changeLanguageRmj.GetComponent<Image>().color;
                col.a = 0.2f;
                changeLanguageJap.GetComponent<Image>().color = col;
                changeLanguageVie.GetComponent<Image>().color = col;
                col.a = 1f;
                changeLanguageRmj.GetComponent<Image>().color = col;

                // Change Language Instantly
                if (listConversation.Count > 0)
                {
                    if (currentConversation.model == "npc")
                    {
                        npcText.text = currentConversation.detail_romaji;
                    }
                    else
                    {
                        try
                        {
                            //ThaoEm
                          //  npcText.text = listConversation[nextIndex - 2].detail_romaji;
                            playerText.text = currentConversation.detail_romaji;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }

                break;
            case Lang.Vie:
                col = changeLanguageVie.GetComponent<Image>().color;
                col.a = 0.2f;
                changeLanguageRmj.GetComponent<Image>().color = col;
                changeLanguageJap.GetComponent<Image>().color = col;
                col.a = 1f;
                changeLanguageVie.GetComponent<Image>().color = col;

                // Change Language Instantly
                if (listConversation.Count > 0)
                {
                    if (currentConversation.model == "npc")
                    {
                        npcText.text = currentConversation.detail_vi;
                    }
                    else
                    {  //Thaoem
                       // npcText.text = listConversation[nextIndex - 2].detail_vi;
                        playerText.text = currentConversation.detail_vi;
                    }
                }

                break;
            default:
                break;
        }

        // Loading Text effect
        if (loadingTextDuringPlay.activeInHierarchy)
        {
            Color col = loadingTextDuringPlay.GetComponent<Text>().color;
            col.a = Mathf.Lerp(minT, maxT, t);
            t -= 0.5f * Time.deltaTime;
            if (t < 0f)
            {
                float temp = maxT;
                maxT = minT;
                minT = temp;
                t = 1f;
            }
            loadingTextDuringPlay.GetComponent<Text>().color = col;
        }
    }
    #endregion

    #region Methods
    private void SpawnConversationLog(string mode, GameObject baseLog, Sprite rankLogSprite = null)
    {
        // Clone + position
        var cloneConvoLog = Instantiate(baseLog);
        //cloneScenarioSelection.transform.localScale = new Vector3(150f, 150f, 150f);
        cloneConvoLog.transform.SetParent(conversationLog.transform, false);
        cloneConvoLog.transform.localPosition = new Vector3(cloneConvoLog.transform.localPosition.x, cloneConvoLog.transform.localPosition.y, 0f);

        // Change convoLog sprite according the the rank
        if (rankLogSprite != null)
        {
            cloneConvoLog.GetComponent<Image>().sprite = rankLogSprite;
            cloneConvoLog.GetComponent<Image>().type = Image.Type.Sliced;
        }

        // Add info to ConversationLog script
        cloneConvoLog.GetComponent<ConvoLogDetail>().id = currentConversation.id;
        cloneConvoLog.GetComponent<ConvoLogDetail>().logDetail = currentConversation.detail_ja;
        cloneConvoLog.GetComponent<ConvoLogDetail>().hintLife = currentConversation.life_tip;
        cloneConvoLog.GetComponent<ConvoLogDetail>().hintLanguage = currentConversation.language_tip;
        cloneConvoLog.GetComponent<ConvoLogDetail>().logIndex = convoLogs.Count;

        // Add convo log to texts. If response is null, add "" instead.
        cloneConvoLog.transform.GetChild(1).GetComponent<Text>().text = cloneConvoLog.GetComponent<ConvoLogDetail>().logDetail ?? "";

        // Add function
        if (mode == player_log_with_hint_mode || mode == npc_log_with_hint_mode)
        {
            if (!string.IsNullOrEmpty(currentConversation.life_tip))
            {
                cloneConvoLog.transform.GetChild(6).transform.GetChild(0).GetComponent<Button>().onClick.AddListener(
                    () => ShowHintClick(hint_life_mode, 
                    cloneConvoLog.GetComponent<ConvoLogDetail>().hintLife,
                    cloneConvoLog.GetComponent<ConvoLogDetail>().logIndex));
            }
            else
            {
                // Text
                cloneConvoLog.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);

                // Button
                cloneConvoLog.transform.GetChild(6).transform.GetChild(0).gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(currentConversation.language_tip))
            {
                cloneConvoLog.transform.GetChild(6).transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                    () => ShowHintClick(hint_language_mode, 
                    cloneConvoLog.GetComponent<ConvoLogDetail>().hintLanguage,
                    cloneConvoLog.GetComponent<ConvoLogDetail>().logIndex));
            }
            else
            {
                // Text
                cloneConvoLog.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);

                // Button
                cloneConvoLog.transform.GetChild(6).transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        // Add cloned convo log to list
        convoLogs.Add(cloneConvoLog);

        autoScrollToBottom = true;
    }

    /// <summary>
    /// Event call by EventTrigger of ConvoLog ScrollView
    /// </summary>
    public void OnDraggingLog()
    {
        autoScrollToBottom = false;
    }

    #endregion

    #region ButtonClick
    public void RecordClick()
    {
        AudioManager.PlaySound(Sounds.Click);
        if (!sttHandler.isReadyToTranscribe)
        {
            sttHandler.isReadyToTranscribe = true; // co the gui du lieu
        }
        else
        {
            sttHandler.isReadyToTranscribe = false; // khong duoc gui du lieu 
        }
    }

    public void SkipClick()
    {
        AudioManager.PlaySound(Sounds.Click);
        sttHandler.isReadyToTranscribe = false;
        sttHandler.StopListening();
        int nextConversationIndex = FindIndexNPCSpeech(nextIndex);
        nextConversation = listConversation[nextConversationIndex];
        StartConversation(nextConversationIndex);
        // kiem vi tri cua doan hoi thoai npc noi trong lan tiep theo 
        // bat hoi thoi do len 
        // gui data len backend bao doan hoi thoai thu bao nhieu do skip
    }

    public void LanguageChangeClick(int selection)
    {
        AudioManager.PlaySound(Sounds.Click);
        switch ((Lang)selection)
        {
            case (Lang.Jap):
                currentLang = Lang.Jap;
                break;
            case (Lang.Rmj):
                currentLang = Lang.Rmj;
                break;
            case (Lang.Vie):
                currentLang = Lang.Vie;
                break;
            default:
                break;
        }
    }

    public void RepeatNpcClick()
    {
        AudioManager.PlaySound(Sounds.Click);
        //ClickStop();
        //isNpcRepeating = true;
        //npcSpeaker.Speaking(npcSpeech);
        getSTT.SetActive(false);
        repeatNpcSpeakButton.gameObject.SetActive(false);

        if (!isRealVoice)
            npcSpeak.Speaking(lastConversation); // always speak Japanese
        else
            npcSpeak.Speaking(lastConversation, true); // always speak Japanese

        sttHandler.StopListening();
        sttHandler.isReadyToTranscribe = false;
        isNpcRepeat = true;

    }

    public void ShowHintClick(string mode, string hintContent, int logIndex)
    {
        AudioManager.PlaySound(Sounds.Click);
        switch (mode)
        {
            case hint_life_mode:
                hintSection.transform.GetChild(0).GetComponent<Text>().text = hint_life_title; // Set title
                hintSection.GetComponent<Image>().sprite = sprHintLife;
                break;
            case hint_language_mode:
                hintSection.transform.GetChild(0).GetComponent<Text>().text = hint_language_title; // Set title
                hintSection.GetComponent<Image>().sprite = sprHintLanguage;
                break;
            default:
                break;
        }
        hintSection.transform.GetChild(1).GetComponent<Text>().text = hintContent;
        hintSection.SetActive(true);

        Color col = conversationLogSection.GetComponent<Image>().color;
        col.a = 0.2f;

        // Blur convolog section
        conversationLogSection.GetComponent<Image>().color = col;

        // Blur Result section (If Active)
        if (resultSection.activeInHierarchy)
        {
            resultSection.GetComponent<Image>().color = col;
            Debug.Log("AAA");
            foreach (Transform child in resultSection.transform)
            {
                Debug.Log("AAA1");
                if (child.GetComponent<Image>() != null)
                    child.GetComponent<Image>().color = col;
            }
        }

        // Blur NPC speech section (If Active)
        if (npcSpeechSection.activeInHierarchy)
            npcSpeechSection.GetComponent<Image>().color = col;



        // Blur speech-to-text section (If Active)
        if (speechToTextSection.activeInHierarchy)
        {
            speechToTextSection.GetComponent<Image>().color = col;
            foreach (Transform child in speechToTextSection.transform)
            {
                if (child.GetComponent<Image>() != null)
                    child.GetComponent<Image>().color = col;
            }
        }

        // Blur Result section (If Active)
        if (resultSection.activeInHierarchy)
        {
            resultSection.GetComponent<Image>().color = col;
            Debug.Log("AAA");
            foreach (Transform child in resultSection.transform)
            {
                Debug.Log("AAA1");
                if (child.GetComponent<Image>() != null)
                    child.GetComponent<Image>().color = col;
            }
        }

        viewPortScrollRect.color = col;

        // Blur other convo logs
        for (int i = 0; i < convoLogs.Count; i++)
        {
            if (i != logIndex)
            {
                convoLogs[i].GetComponent<Image>().color = col;
                foreach (Transform child in convoLogs[i].transform.GetChild(6).transform)
                {
                    if (child.GetComponent<Image>() != null && child.gameObject.activeInHierarchy)
                        child.GetComponent<Image>().color = col;
                }
            }
        }

       
    }

    public void ExitHintClick()
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);
        hintSection.SetActive(false);

        Color col = conversationLogSection.GetComponent<Image>().color;
        col.a = 1f;

        // Unblur convolog section
        conversationLogSection.GetComponent<Image>().color = col;

        

        // Unblur NPC speech section
        if (npcSpeechSection.activeInHierarchy)
            npcSpeechSection.GetComponent<Image>().color = col;

        // Unblur speech-to-text section
        if (speechToTextSection.activeInHierarchy)
        {
            speechToTextSection.GetComponent<Image>().color = col;
            foreach (Transform child in speechToTextSection.transform)
            {
                if (child.GetComponent<Image>() != null)
                    child.GetComponent<Image>().color = col;
            }
        }
        
        // Unblur Result section (If Active)
        if (resultSection.activeInHierarchy)
        {
            resultSection.GetComponent<Image>().color = col;
            foreach (Transform child in resultSection.transform)
            {
                if (child.GetComponent<Image>() != null)
                    child.GetComponent<Image>().color = col;
            }
        }

        viewPortScrollRect.color = col;

        // Unblur other convo logs
        foreach (GameObject convo in convoLogs)
        {
            convo.GetComponent<Image>().color = col;
            foreach (Transform child in convo.transform.GetChild(6).transform)
            {
                if (child.GetComponent<Image>() != null && child.gameObject.activeInHierarchy)
                    child.GetComponent<Image>().color = col;
            }
        }
    }
    #endregion

    #region HandleFlowSpeak
    public void StartConversation(int startIndex =0) // bat dau hoi thoai
    {
        scoreRank.gameObject.SetActive(false);
        repeatNpcSpeakButton.gameObject.SetActive(false); // ThaoEm
        //isRecord = true;
        currentConversation = listConversation[startIndex];
        nextConversation = null;
        //if (startIndex < conversationLength - 1)
        //{
        //    nextConversation = listConversation[startIndex + 1];
        //}
        stopIndex = startIndex;
        nextIndex = startIndex + 1;
        DetectFlowConversation();
    }
    
    private void DetectFlowConversation()
    {
        // Language
        string detailString = "";
        switch (currentLang)
        {
            case Lang.Jap:
                detailString = currentConversation.detail_ja;
                break;
            case Lang.Vie:
                detailString = currentConversation.detail_vi;
                break;
            case Lang.Rmj:
                detailString = currentConversation.detail_romaji;
                break;
            default:
                break;
        }

        if (currentConversation.model == "npc")
        {
            soundBar.SetActive(false);
            getSTT.SetActive(false);
            npcText.text = detailString;

            if (!isRealVoice)
                npcSpeak.Speaking(currentConversation); // always speak Japanese
            else
                npcSpeak.Speaking(currentConversation, true); // always speak Japanese

            lastConversation = currentConversation;
            lastNPCSpeech = currentConversation.detail_ja;

            // Add to conversation log
            if (!string.IsNullOrEmpty(currentConversation.life_tip) || !string.IsNullOrEmpty(currentConversation.language_tip))
            {
                SpawnConversationLog(npc_log_with_hint_mode, baseNPCConvoLogWithBtn);
            }
            else
            {
                SpawnConversationLog(npc_log_mode, baseNPCConvoLog);
            }

            // khi npc noi thi moi thu tat het,gio chua lam
        }
        else
        {
            // bat thu am
            getSTT.SetActive(true);
            soundBar.SetActive(true);
            repeatNpcSpeakButton.gameObject.SetActive(true);
            playerText.text = detailString;
            getSpeechToText.finalResult = "";
            getSpeechToText.outputInterim = "";
            getSpeechToText.sttText.text = "";
            sttHandler.StartListening();

            // 1. isReadyToTranscribe == true thi hien micro va bat dau thu am 
            //    isReadyToTranscribe == false thi ngat micro realtime 
            // sau khi nguoi dung noi co diem roi thi se ngung thu am vaa ngung post du lieu
            // chua nghi ra
        }
    }
    #endregion

    #region HandleLogic
    private int FindIndexNPCSpeech(int curIndexConversation)
    {
        for(int i = curIndexConversation; i<conversationLength;i++)
        {
            if(listConversation[i].model == "npc") // tim doan hoi thoai tiep theo
            {
                curIndexConversation = i;
                break;
            }
        }
        return curIndexConversation;
    }
    #endregion

    #region LoadAnimation
    private IEnumerator LoadAnimationToScore(int totalScore, Sprite rankSprite)
    {
        scoreRank.GetComponent<Image>().sprite = rankSprite;
        scoreRank.gameObject.SetActive(true);
        scoreRank.GetComponent<Image>().fillAmount = 0;
        yield return new WaitForSeconds(2);
        isPlayerPassed = true;
    }
    #endregion
}
