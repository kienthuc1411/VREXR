using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ModeSelectScene modeSelectScene;

    public void OnMouseOver()
    {
        switch (gameObject.name)
        {
            case "ScenarioSelect":
                modeSelectScene.currentMode = ModeSelectScene.Mode.single;
                break;
            case "FillModeSelect":
                modeSelectScene.currentMode = ModeSelectScene.Mode.fill;
                break;
            case "FreeModeSelect":
                modeSelectScene.currentMode = ModeSelectScene.Mode.free;
                break;
            default:
                break;
        }
    }

    public void OnMouseExit()
    {
        if (!modeSelectScene.isLockSelection)
        {
            modeSelectScene.currentMode = ModeSelectScene.Mode.none;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseOver();
    }
}
