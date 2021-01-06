using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//public class ScenarioSelected : MonoBehaviour
public class ScenarioSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int id;
    public bool isClicked = false;
    private Color baseColor;
    private Color baseTagColor;

    [Header("Sprites")]
    public Sprite sprScenarioActive;
    public Sprite sprScenarioHover;
    public Sprite sprScenarioNormal;
    public Texture sprThumbnailInfo;
    public SelectScenarioApi selectScenarioApi;
    public int indexInsideListData;

    private void Start()
    {
        baseColor = transform.GetChild(1).GetChild(0).GetComponent<Text>().color;
        baseTagColor = transform.GetChild(5).GetComponent<Image>().color;
    }

    public void ClickScenario()
    {
        SelectScenarioFromServer.Id = id;
        isClicked = true;

        // Change image and shrink
        GetComponent<Image>().sprite = sprScenarioActive;
        transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

        // Change played mark (if active)
        if (transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }

        // Change title text to white
        foreach (Transform child in transform.GetChild(1))
        {
            if (child.GetComponent<Text>() != null)
                child.GetComponent<Text>().color = Color.white;
        }
        // Content
        foreach (Transform child in transform.GetChild(2))
        {
            if (child.GetComponent<Text>() != null)
                child.GetComponent<Text>().color = Color.white;
        }
        // Tag border
        transform.GetChild(5).GetComponent<Image>().color = Color.white;
        // Tag text
        transform.GetChild(5).GetChild(0).GetComponent<Text>().color = Color.white;
    }

    private void OnMouseOver()
    {
        if (!isClicked)
        {
            //GetScenarioFromServer.Id = id; ThaoEm
            GetComponent<Image>().sprite = sprScenarioHover;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        GetScenarioFromServer.Id = id; //ThaoEm                                     
        sprThumbnailInfo = transform.GetChild(3).GetChild(0).GetComponent<RawImage>().texture;
    }

    private void OnMouseExit()
    {
        if (!isClicked)
        {
            ResetLayout();
        }
    }

    public void ResetLayout()
    {
        // Image
        GetComponent<Image>().sprite = sprScenarioNormal;
        transform.localScale = new Vector3(1f, 1f, 1f);

        // Change played mark (if active)
        if (transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }

        // Title
        foreach (Transform child in transform.GetChild(1))
        {
            if (child.GetComponent<Text>() != null)
                child.GetComponent<Text>().color = baseColor;
        }
        // Content
        foreach (Transform child in transform.GetChild(2))
        {
            if (child.GetComponent<Text>() != null)
                child.GetComponent<Text>().color = baseColor;
        }

        // Tag border
        transform.GetChild(5).GetComponent<Image>().color = baseTagColor;
        // Tag text
        transform.GetChild(5).GetChild(0).GetComponent<Text>().color = baseTagColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver();
        selectScenarioApi.ScenarioOnMouseHover(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit();
        selectScenarioApi.ScenarioOnMouseExit(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectScenarioApi.ScenarioOnMouseClick(eventData, id, indexInsideListData);
    }


}
