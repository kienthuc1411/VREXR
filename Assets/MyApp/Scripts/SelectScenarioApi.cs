using JSAM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectScenarioApi : MonoBehaviour, IPointerDownHandler
{
    #region Variables
    // Constants
    private static readonly string BaseUrl = "https://vrelearning.online/api/v1/scenarios/";
    private static readonly string FetchTagUrl = "https://vrelearning.online/api/v1/tags/";
    private string currentUrl = BaseUrl;
    private const string SCENARIOS_SORT_ASC = "昇順";
    private const string SCENARIOS_SORT_DSC = "降順";
    private const string SCENARIO_VOICE_REAL = "肉声";
    private const string SCENARIO_VOICE_MACHINE = "機械音声";

    // Private
    private bool autoScrollToBottom;
    [SerializeField]
    private Color color_3070DB;
    [SerializeField]
    private Color color_fb6900;
    private jsonDataClass jsonData;

    // Direction
    private bool isAsc = false;
    // Tags
    private List<Tag> listTags = new List<Tag>();
    private List<int> selectedTags = new List<int>();
    // Sort
    private List<GameObject> sortOptions = new List<GameObject>();
    private List<string> sorts;
    private string selectedSort = "";
    // Voice
    private List<GameObject> voiceOptions = new List<GameObject>();
    private List<string> voices;
    private string selectedVoice = "";

    [Header("Button")]
    public GameObject filterButton;
    public Button selectScenarioButton;
    public Button scenarioSortButton;

    [Header("ContentText")]
    public Text TitleContent;
    public Text DescContent;
    public Text EditorContent;
    public Text TimePlayContent;
    public Text PlayCountContent;
    public Text CreatedAtContent;
    public Text OpeningTextContent;

    [Header("GameObjects")]
    // Sections
    [HideInInspector]
    public List<GameObject> scenarioOptions;
    public GameObject baseScenarioSelection;
    public GameObject disablePanel;
    public GameObject filterSection;
    public GameObject hoverScenarioSection;
    // Parent holder
    public GameObject sortParent;
    public GameObject tagOnHoverParent;
    public GameObject tagParent;
    public GameObject voiceParent;
    // Contents
    public GameObject playedMark;
    public RawImage thumbnailHoverInfo;
    public Transform parentHolder;

    [Header("Prefabs")]
    public GameObject tagPrefab;
    public GameObject tagOnHoverPrefab;

    [Header("Scripts")]
    public SpeechManager smanager;
    [HideInInspector]
    public List<Scenario> ListScenario = new List<Scenario>();

    [Header("Sprites")]
    public Sprite sprFilterActive;
    public Sprite sprFilterNormal;
    public Sprite sprSortBtnAsc;
    public Sprite sprSortBtnDsc;
    public Sprite sprTagActive;
    public Sprite sprTagNormal;
    #endregion

    #region Unity Methods
    void Start()
    {
        // Sorts
        sorts = new List<string>()
        {
            "製作者",
            "プレイ回数",
            "作成日"
        };

        // Voices
        voices = new List<string>()
        {
            "機械音声",
            "肉声"
        };

        // SetActive
        disablePanel.SetActive(false);
        hoverScenarioSection.SetActive(false);
        EnableScenarioButton(false);


        // Request data
        if (isAsc)
            StartCoroutine(GetRequest(BaseUrl + "?direction=asc"));
        else
            StartCoroutine(GetRequest(BaseUrl + "?direction=desc"));

        StartCoroutine(GetRequest(FetchTagUrl, "tag"));

        // Spawn
        SpawnSortOptions();
        SpawnVoiceOptions();

        // autoscroll
        autoScrollToBottom = true;

        // Transparent button
        Color col = selectScenarioButton.GetComponent<Image>().color;
        col.a = 0.4f;
        selectScenarioButton.GetComponent<Image>().color = col;
    }

    //private void OnEnable()
    //{
    //    Start();
    //}

    void Update()
    {
        // Auto scroll convoLog to bottom
        // Scroll only works in Clamped mode. Have to return to Elastic manually
        //if (autoScrollToBottom)
        //{
        //    if (transform.GetChild(1).GetComponent<ScrollRect>().movementType != ScrollRect.MovementType.Clamped)
        //    {
        //        transform.GetChild(1).GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
        //    }
        //    transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -20 * Time.deltaTime);
        //}
        //else
        //{
        //    if (transform.GetChild(1).GetComponent<ScrollRect>().movementType != ScrollRect.MovementType.Elastic)
        //    {
        //        transform.GetChild(1).GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
        //    }
        //}
    }
    #endregion

    #region Process API Request
    IEnumerator GetRequest(string url, string task = "scenario")
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.LogError("Networking Error!");
            }
            if (webRequest.isDone)
            {
                string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                switch (task)
                {
                    case "scenario":
                        ProcessScenarioData(data);
                        break;
                    case "tag":
                        ProcessTagsData(data);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Get tags data
    private void ProcessTagsData(string data)
    {
        TagDataClass tagData = JsonUtility.FromJson<TagDataClass>(data);
        listTags.AddRange(tagData.data);
        foreach (Tag tag in listTags)
        {
            // Set size and position
            var cloneTag = Instantiate(tagPrefab);
            cloneTag.transform.SetParent(tagParent.transform, false);
            cloneTag.transform.localScale = new Vector3(1f, 1f, 1f);
            cloneTag.transform.localPosition = new Vector3(cloneTag.transform.localPosition.x, cloneTag.transform.localPosition.y, 0f);

            // Set text
            cloneTag.transform.GetChild(0).GetComponent<Text>().text = tag.name;

            // Add function
            EventTrigger trigger = cloneTag.GetComponent<EventTrigger>();

            EventTrigger.Entry mouseClickTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            mouseClickTrigger.callback.AddListener((_data) =>
            {
                ClickSelectTag((PointerEventData)_data, tag.id, cloneTag.GetComponent<Image>(), cloneTag.transform.GetChild(0).GetComponent<Text>());
            });
            trigger.triggers.Add(mouseClickTrigger);
        }
    }

    // Get scenarios data. Display and add functions.
    private void ProcessScenarioData(string data)
    {
        jsonData = JsonUtility.FromJson<jsonDataClass>(data);
        int length = jsonData.data.Count;
        ListScenario.AddRange(jsonData.data);
        for (int i = 0; i < length; i++)
        {
            // Clone + position
            var cloneScenarioSelection = Instantiate(baseScenarioSelection);
            cloneScenarioSelection.gameObject.SetActive(true);
            cloneScenarioSelection.transform.GetComponent<ScenarioSelected>().selectScenarioApi = transform.GetComponent<SelectScenarioApi>();
            //cloneScenarioSelection.transform.localScale = new Vector3(3f, 3f, 3f);
            cloneScenarioSelection.transform.SetParent(parentHolder, false);
            cloneScenarioSelection.transform.localPosition =
                new Vector3(cloneScenarioSelection.transform.localPosition.x, cloneScenarioSelection.transform.localPosition.y, 0f);

            // Add function
            cloneScenarioSelection.GetComponent<Button>().onClick.AddListener(ClickSelectScenario);

            // Get attached Scenario
            ScenarioSelected scenarioSelected = cloneScenarioSelection.GetComponent<ScenarioSelected>();
            scenarioSelected.id = jsonData.data[i].id;
            scenarioSelected.indexInsideListData = i;
            /*
            // Add Event Triggers
            EventTrigger trigger = cloneScenarioSelection.GetComponent<EventTrigger>();
        
            EventTrigger.Entry mouseHoverTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            mouseHoverTrigger.callback.AddListener((_data) => { ScenarioOnMouseHover(((PointerEventData)_data)); });

            EventTrigger.Entry mouseExitTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            mouseExitTrigger.callback.AddListener((_data) => { ScenarioOnMouseExit((PointerEventData)_data); });

            EventTrigger.Entry mouseClickTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            
            mouseClickTrigger.callback.AddListener((_data) =>
            {
                ScenarioOnMouseClick((PointerEventData)_data, cloneScenarioSelection.GetComponent<ScenarioSelected>().id,i);
            });

            EventTrigger.Entry dragTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.BeginDrag
            };

            dragTrigger.callback.AddListener((_data) => { OnDraggingLog((PointerEventData)_data); });

            trigger.triggers.Add(mouseHoverTrigger);
            trigger.triggers.Add(mouseExitTrigger);
            trigger.triggers.Add(mouseClickTrigger);
            trigger.triggers.Add(dragTrigger);
            */
            // Add scenario info to texts. If response is null, add "" instead.
            // Scenario name (2)(0)
            cloneScenarioSelection.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = jsonData.data[i].title ?? "";

            // Description (2)(1)
            cloneScenarioSelection.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = jsonData.data[i].description ?? "";

            // Editor (2)(2)
            cloneScenarioSelection.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = jsonData.data[i].editor ?? "";

            // Timeplay (2)(3)
            string est_time = jsonData.data[i].experience_time ?? "";
            if (est_time != "")
            {
                cloneScenarioSelection.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = est_time.Substring(0, 2) + "分";
            }

            // Play count (2)(4)
            if (!String.IsNullOrEmpty(jsonData.data[i].play_count))
            {
                cloneScenarioSelection.transform.GetChild(2).GetChild(4).GetComponent<Text>().text = jsonData.data[i].play_count + "回";
                if (jsonData.data[i].play_count == "0")
                    cloneScenarioSelection.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                else
                    cloneScenarioSelection.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                cloneScenarioSelection.transform.GetChild(2).GetChild(4).GetComponent<Text>().text = "";
                cloneScenarioSelection.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }

            // Created At (2)(5)
            cloneScenarioSelection.transform.GetChild(2).GetChild(5).GetComponent<Text>().text = jsonData.data[i].created_at ?? "";

            // Voice_type (5)(0)
            if (jsonData.data[i].voice_type == "machine_voice")
            {
                cloneScenarioSelection.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = SCENARIO_VOICE_MACHINE;
            }
            else if (jsonData.data[i].voice_type == "real_voice")
            {
                cloneScenarioSelection.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = SCENARIO_VOICE_REAL;
            }
            else
            {
                cloneScenarioSelection.transform.GetChild(5).gameObject.SetActive(false);
            }

            // Add thumbnail image
            if (!String.IsNullOrEmpty(jsonData.data[i].thumbnail_image))
            {
                StartCoroutine(DownloadImage(jsonData.data[i].thumbnail_image, cloneScenarioSelection.transform.GetChild(3).GetChild(0).GetComponent<RawImage>()));
            }

            // Add cloned scenario selection to list
            scenarioOptions.Add(cloneScenarioSelection);
        }
    }

    // Download thumbnail image
    IEnumerator DownloadImage(string url, RawImage img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.LogError(request.error);
        else
            yield return img.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
    #endregion

    #region Scenario Mouse Events
    // Hover on scenario to show more info
    public void ScenarioOnMouseHover(PointerEventData _data)
    {
        // Hover Show Info
        hoverScenarioSection.gameObject.SetActive(true);
        for (int i = 0; i < ListScenario.Count; i++)
        {
            if (ListScenario[i].id == GetScenarioFromServer.Id)
            {
                // StartCoroutine(DownloadImage(ListScenario[i].thumbnail_image, thumbnailHoverInfo));

                //ThaoEm
                if (_data.pointerCurrentRaycast.gameObject.transform.GetComponent<ScenarioSelected>() != null)
                    if (_data.pointerCurrentRaycast.gameObject.transform.GetComponent<ScenarioSelected>().sprThumbnailInfo != null)
                        thumbnailHoverInfo.texture = _data.pointerCurrentRaycast.gameObject.transform.GetComponent<ScenarioSelected>().sprThumbnailInfo;
                    else
                    {
                        StartCoroutine(DownloadImage(ListScenario[i].thumbnail_image, thumbnailHoverInfo));
                    }
                // Scenario name
                TitleContent.text = ListScenario[i].title ?? "";

                // Description
                DescContent.text = ListScenario[i].description ?? "";

                // Editor
                EditorContent.text = ListScenario[i].editor ?? "";

                // Timeplay
                TimePlayContent.text = ListScenario[i].experience_time;
                string est_time = ListScenario[i].experience_time ?? "";
                if (est_time != "")
                {
                    TimePlayContent.text = est_time.Substring(0, 2) + "分";
                    TimePlayContent.text += est_time.Substring(3, 2) + "秒";
                }

                // Play count
                if (!String.IsNullOrEmpty(ListScenario[i].play_count))
                {
                    PlayCountContent.text = ListScenario[i].play_count + "回";
                    if (ListScenario[i].play_count == "0")
                        playedMark.SetActive(false);
                    else
                        playedMark.SetActive(true);
                }
                else
                {
                    PlayCountContent.text = "";
                    playedMark.SetActive(false);
                }

                // Created At
                CreatedAtContent.text = ListScenario[i].created_at ?? "";

                // Spawn tags
                if (ListScenario[i].tag.Length > 0)
                {
                    foreach (string tag in ListScenario[i].tag)
                    {
                        // Set size and position
                        var cloneTag = Instantiate(tagOnHoverPrefab);
                        cloneTag.transform.SetParent(tagOnHoverParent.transform, false);
                        cloneTag.transform.localScale = new Vector3(1f, 1f, 1f);
                        cloneTag.transform.localPosition = new Vector3(cloneTag.transform.localPosition.x, cloneTag.transform.localPosition.y, 0f);

                        // Set text
                        cloneTag.transform.GetChild(0).GetComponent<Text>().text = tag;
                    }
                }

                return;
            }
        }
    }

    // Exit from hovering scenario
    public void ScenarioOnMouseExit(PointerEventData _data)
    {
        hoverScenarioSection.gameObject.SetActive(false);
        // Destroy all child
        if (tagOnHoverParent.transform.childCount > 0)
        {
            foreach (Transform child in tagOnHoverParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    //Dragging Log
    public void OnDraggingLog(PointerEventData _data)
    {
        autoScrollToBottom = false;
    }

    // Click on scenario
    public void ScenarioOnMouseClick(PointerEventData _data, int scenarioID, int dataID)
    {
        // Enable Select scenario button
        EnableScenarioButton(true);

        // Deselect other scenario options
        foreach (GameObject scenario in scenarioOptions)
        {
            ScenarioSelected scenarioScript = scenario.GetComponent<ScenarioSelected>();
            if (scenarioScript.isClicked && scenarioScript.id != scenarioID)
            {
                scenarioScript.ResetLayout();
            }
        }

        // Add Opening content text
        for (int i = 0; i < jsonData.data.Count; i++)
        {
            if (jsonData.data[i].id == scenarioID)
            {

                OpeningTextContent.text = jsonData.data[i].opening_text ?? "";
                // Change voice_type
                if (jsonData.data[i].voice_type == "machine_voice")
                    smanager.isRealVoice = false;
                else
                    smanager.isRealVoice = true;
            }
        }
    }
    #endregion

    #region Buttons
    private void ClickSelectVoice(PointerEventData _data, string voice, Image img, Text text)
    {
        AudioManager.PlaySound(Sounds.Click);
        if (selectedVoice == voice)
        {
            selectedVoice = "";
            img.sprite = sprTagNormal;
            text.color = color_3070DB;
        }
        else
        {
            selectedVoice = voice;
            img.sprite = sprTagActive;
            text.color = Color.white;

            // Remove other options
            foreach (GameObject vo in voiceOptions)
            {
                if (vo.transform.GetChild(0).GetComponent<Text>().text != selectedVoice)
                {
                    vo.GetComponent<Image>().sprite = sprTagNormal;
                    vo.transform.GetChild(0).GetComponent<Text>().color = color_3070DB;
                }
            }
        }
    }

    private void ClickSelectSort(PointerEventData _data, string sort, Image img, Text text)
    {
        AudioManager.PlaySound(Sounds.Click);
        if (selectedSort == sort)
        {
            selectedSort = "";
            img.sprite = sprTagNormal;
            text.color = color_3070DB;
        }
        else
        {
            selectedSort = sort;
            img.sprite = sprTagActive;
            text.color = Color.white;

            // Remove other options
            foreach (GameObject so in sortOptions)
            {
                if (so.transform.GetChild(0).GetComponent<Text>().text != selectedSort)
                {
                    so.GetComponent<Image>().sprite = sprTagNormal;
                    so.transform.GetChild(0).GetComponent<Text>().color = color_3070DB;
                }
            }
        }
    }

    private void ClickSelectTag(PointerEventData _data, int id, Image img, Text text)
    {
        AudioManager.PlaySound(Sounds.Click);
        if (!selectedTags.Exists(x => x == id))
        {
            selectedTags.Add(id);
            img.sprite = sprTagActive;
            text.color = Color.white;
        }
        else
        {
            selectedTags.Remove(id);
            img.sprite = sprTagNormal;
            text.color = color_3070DB;
        }
    }

    // Select a scenario
    public void ClickSelectScenario()
    {
        Color col = selectScenarioButton.GetComponent<Image>().color;
        col.a = 1.0f;
        selectScenarioButton.GetComponent<Image>().color = col;
    }

    public void ClickShowFilter()
    {
        AudioManager.PlaySound(Sounds.Click);
        // Activate filter section
        filterButton.GetComponent<Image>().sprite = sprFilterActive;
        filterButton.transform.GetChild(0).GetComponent<Text>().color = color_fb6900;
        filterSection.SetActive(true);

        // Deactivate Scenario panel
        Color col = GetComponent<Image>().color;
        col.a = 0.2f;
        GetComponent<Image>().color = col;
        disablePanel.SetActive(true);
    }

    public void ClickCancelFilter()
    {
        AudioManager.PlaySound(Sounds.ClickButtonCancel);
        // Deactivate filter section
        filterButton.GetComponent<Image>().sprite = sprFilterNormal;
        filterButton.transform.GetChild(0).GetComponent<Text>().color = color_3070DB;
        filterSection.SetActive(false);

        // Activate Scenario panel
        Color col = GetComponent<Image>().color;
        col.a = 1f;
        GetComponent<Image>().color = col;
        disablePanel.SetActive(false);
    }

    public void ClickFilter()
    {
        AudioManager.PlaySound(Sounds.Click);

        ClearAllScenarios();
        currentUrl = BaseUrl;

        // Tags
        if (selectedTags.Count > 0)
        {
            currentUrl += "?tag_ids=";
            foreach (int id in selectedTags)
            {
                currentUrl += id.ToString() + ",";
            }
        }

        // Sort
        if (!String.IsNullOrEmpty(selectedSort))
        {
            string sort = "";
            switch (sorts.IndexOf(selectedSort))
            {
                case 0:
                    sort = "editor";
                    break;
                case 1:
                    sort = "play_count";
                    break;
                case 2:
                    sort = "created_at";
                    break;
                default:
                    break;
            }
            if (currentUrl == BaseUrl)
                currentUrl += "?sort=" + sort;
            else
                currentUrl += "&sort=" + sort;
        }

        // Voices
        if (!String.IsNullOrEmpty(selectedVoice))
        {
            string voice = "";
            switch (voices.IndexOf(selectedVoice))
            {
                case 0:
                    voice = "machine_voice";
                    break;
                case 1:
                    voice = "real_voice";
                    break;
                default:
                    break;
            }
            if (currentUrl == BaseUrl)
                currentUrl += "?voice_type=" + voice;
            else
                currentUrl += "&voice_type=" + voice;
        }

        // Request final url
        StartCoroutine(GetRequest(currentUrl));

        // Deactivate filter section
        filterButton.GetComponent<Image>().sprite = sprFilterNormal;
        filterButton.transform.GetChild(0).GetComponent<Text>().color = color_3070DB;
        filterSection.SetActive(false);
        // Activate Scenario panel
        Color col = GetComponent<Image>().color;
        col.a = 1f;
        GetComponent<Image>().color = col;
        disablePanel.SetActive(false);
    }
    #endregion

    #region Other Methods
    private void ClearAllScenarios()
    {
        foreach (GameObject scenario in scenarioOptions)
        {
            Destroy(scenario);
        }
        scenarioOptions.Clear();
    }

    // Sort scenarios with different features
    public void SortScenario()
    {
        ClearAllScenarios();
        if (isAsc)
        {
            scenarioSortButton.GetComponent<Image>().sprite = sprSortBtnDsc;
            scenarioSortButton.transform.GetChild(0).GetComponent<Text>().text = SCENARIOS_SORT_DSC;
            if (currentUrl == BaseUrl)
                StartCoroutine(GetRequest(currentUrl + "?direction=asc"));
            else
                StartCoroutine(GetRequest(currentUrl + "&direction=asc"));
        }
        else
        {
            scenarioSortButton.GetComponent<Image>().sprite = sprSortBtnAsc;
            scenarioSortButton.transform.GetChild(0).GetComponent<Text>().text = SCENARIOS_SORT_ASC;
            if (currentUrl == BaseUrl)
                StartCoroutine(GetRequest(currentUrl + "?direction=desc"));
            else
                StartCoroutine(GetRequest(currentUrl + "&direction=desc"));
        }
        isAsc = !isAsc;
    }

    // On click to the panel to unselect scenarios
    public void OnPointerDown(PointerEventData eventData)
    {
        // Reset scenario options
        foreach (GameObject scenario in scenarioOptions)
        {
            scenario.GetComponent<ScenarioSelected>().isClicked = false;
            scenario.GetComponent<ScenarioSelected>().ResetLayout();
            EnableScenarioButton(false);
            

        }
    }

    public void EnableScenarioButton(bool state)
    {
        Color col = selectScenarioButton.GetComponent<Image>().color;
        if (state)
        {
            col.a = 1f;
            selectScenarioButton.GetComponent<Image>().color = col;
            selectScenarioButton.transform.GetChild(0).GetComponent<Text>().color = col;
        }
        else
        {
            col.a = 0.2f;
            selectScenarioButton.GetComponent<Image>().color = col;
            selectScenarioButton.transform.GetChild(0).GetComponent<Text>().color = col;
        }
        selectScenarioButton.GetComponent<Button>().enabled = state;
    }

    // Spawn sort options
    private void SpawnSortOptions()
    {
        foreach (string sort in sorts)
        {
            // Set size and position
            var cloneSort = Instantiate(tagPrefab);
            cloneSort.transform.SetParent(sortParent.transform, false);
            cloneSort.transform.localScale = new Vector3(1f, 1f, 1f);
            cloneSort.transform.localPosition = new Vector3(cloneSort.transform.localPosition.x, cloneSort.transform.localPosition.y, 0f);

            // Set text
            cloneSort.transform.GetChild(0).GetComponent<Text>().text = sort;

            // Add function
            EventTrigger trigger = cloneSort.GetComponent<EventTrigger>();

            EventTrigger.Entry mouseClickTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            mouseClickTrigger.callback.AddListener((_data) =>
            {
                ClickSelectSort((PointerEventData)_data, sort, cloneSort.GetComponent<Image>(), cloneSort.transform.GetChild(0).GetComponent<Text>());
            });
            trigger.triggers.Add(mouseClickTrigger);

            sortOptions.Add(cloneSort);
        }
    }

    // Spawn voice options
    private void SpawnVoiceOptions()
    {
        foreach (string voice in voices)
        {
            // Set size and position
            var cloneVoice = Instantiate(tagPrefab);
            cloneVoice.transform.SetParent(voiceParent.transform, false);
            cloneVoice.transform.localScale = new Vector3(1f, 1f, 1f);
            cloneVoice.transform.localPosition = new Vector3(cloneVoice.transform.localPosition.x, cloneVoice.transform.localPosition.y, 0f);

            // Set text
            cloneVoice.transform.GetChild(0).GetComponent<Text>().text = voice;

            // Add function
            EventTrigger trigger = cloneVoice.GetComponent<EventTrigger>();

            EventTrigger.Entry mouseClickTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            mouseClickTrigger.callback.AddListener((_data) =>
            {
                ClickSelectVoice((PointerEventData)_data, voice, cloneVoice.GetComponent<Image>(), cloneVoice.transform.GetChild(0).GetComponent<Text>());
            });
            trigger.triggers.Add(mouseClickTrigger);

            voiceOptions.Add(cloneVoice);
        }
    }

    public void OnDraggingLog()
    {
        autoScrollToBottom = false;
    }
    #endregion
}