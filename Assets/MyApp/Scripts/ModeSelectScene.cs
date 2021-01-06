using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using JSAM;

public class ModeSelectScene : MonoBehaviour, IPointerDownHandler
{
    [Header("Button")]
    public Button selectModeButton;

    [Header("Modes")]
    public GameObject modeSinglePlayer;
    public GameObject modeFill;
    public GameObject modeFree;

    [Header("Scripts")]
    public GameObject scenarioSelectSection;

    [Header("Tooltips")]
    public GameObject tooltipFill;
    public GameObject tooltipFree;
    public GameObject tooltipScenario;

    // Others
    [HideInInspector]
    public bool isLockSelection = false;

    // Mode Selection
    public enum Mode
    {
        none,
        single,
        fill,
        free
    }
    [HideInInspector]
    public Mode currentMode = Mode.none;

    private void Update()
    {
        if (!isLockSelection)
        {
            EnableAllModes();
            // Mode selection
            switch (currentMode)
            {
                case Mode.none:
                    Color col = modeSinglePlayer.GetComponent<Image>().color;
                    col.a = 1f;
                    modeSinglePlayer.GetComponent<Image>().color = col;
                    modeFill.GetComponent<Image>().color = col;
                    modeFree.GetComponent<Image>().color = col;

                    modeSinglePlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                    modeFill.transform.localScale = new Vector3(1f, 1f, 1f);
                    modeFree.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Mode tooltips
                    tooltipFill.SetActive(false);
                    tooltipFree.SetActive(false);
                    tooltipScenario.SetActive(false);

                    // Select Mode Button
                    DisableSelectModeButton();
                    break;
                case Mode.single:
                    col = modeSinglePlayer.GetComponent<Image>().color;
                    col.a = 1f;
                    modeSinglePlayer.GetComponent<Image>().color = col;
                    col.a = 0.5f;
                    modeFill.GetComponent<Image>().color = col;
                    modeFree.GetComponent<Image>().color = col;

                    modeSinglePlayer.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    modeFill.transform.localScale = new Vector3(1f, 1f, 1f);
                    modeFree.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Mode tooltips
                    tooltipFill.SetActive(false);
                    tooltipFree.SetActive(false);
                    tooltipScenario.SetActive(true);
                    break;
                case Mode.fill:
                    col = modeFill.GetComponent<Image>().color;
                    col.a = 1f;
                    modeFill.GetComponent<Image>().color = col;
                    col.a = 0.5f;
                    modeSinglePlayer.GetComponent<Image>().color = col;
                    modeFree.GetComponent<Image>().color = col;

                    modeFill.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    modeSinglePlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                    modeFree.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Mode tooltips
                    tooltipFill.SetActive(true);
                    tooltipFree.SetActive(false);
                    tooltipScenario.SetActive(false);
                    break;
                case Mode.free:
                    col = modeFree.GetComponent<Image>().color;
                    col.a = 1f;
                    modeFree.GetComponent<Image>().color = col;
                    col.a = 0.5f;
                    modeSinglePlayer.GetComponent<Image>().color = col;
                    modeFill.GetComponent<Image>().color = col;

                    modeFree.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    modeSinglePlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                    modeFill.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Mode tooltips
                    tooltipFill.SetActive(false);
                    tooltipFree.SetActive(true);
                    tooltipScenario.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            DisableAllModes();
        }
    }

    public void ClickSelectMode(int mode)
    {
        AudioManager.PlaySound(Sounds.Click);
        isLockSelection = true;
        switch (mode)
        {
            case 0:
                currentMode = Mode.single;
                EnableSelectModeButton();
                break;
            case 1:
                currentMode = Mode.fill;
                EnableSelectModeButton();
                break;
            case 2:
                currentMode = Mode.free;
                EnableSelectModeButton();
                break;
            default:
                currentMode = Mode.none;
                DisableSelectModeButton();
                isLockSelection = false;
                break;
        }
    }

    public void ClickStartMode()
    {
        AudioManager.PlaySound(Sounds.Click);
        scenarioSelectSection.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // Disable all modes when lock selection
    private void DisableAllModes()
    {
        modeSinglePlayer.GetComponent<EventTrigger>().enabled = false;
        modeFill.GetComponent<EventTrigger>().enabled = false;
        modeFree.GetComponent<EventTrigger>().enabled = false;
    }

    // Enable all modes
    private void EnableAllModes()
    {
        modeSinglePlayer.GetComponent<EventTrigger>().enabled = true;
        modeFill.GetComponent<EventTrigger>().enabled = true;
        modeFree.GetComponent<EventTrigger>().enabled = true;
    }

    private void DisableSelectModeButton()
    {
        Color col = selectModeButton.GetComponent<Image>().color;
        col.a = 0.2f;
        selectModeButton.GetComponent<Image>().color = col;
        selectModeButton.transform.GetChild(0).GetComponent<Text>().color = col;
        selectModeButton.GetComponent<Button>().enabled = false;
    }

    private void EnableSelectModeButton()
    {
        Color col = selectModeButton.GetComponent<Image>().color;
        col.a = 1f;
        selectModeButton.GetComponent<Image>().color = col;
        selectModeButton.transform.GetChild(0).GetComponent<Text>().color = col;
        selectModeButton.GetComponent<Button>().enabled = true;
    }

    // Click on Panel to unselect mode
    public void OnPointerDown(PointerEventData _data)
    {
        isLockSelection = false;
        currentMode = Mode.none;
    }
}
