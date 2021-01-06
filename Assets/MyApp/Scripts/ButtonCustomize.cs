using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonCustomize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public Color highLightText;
    public Color defaultText;
    public Text textColor;
    public Sprite highlightBtn;
    public Sprite defaultBtn;
    public GameObject popupPanel;
    public bool isDecline;
    public GameObject gameManager;
    void Start()
    {
        
    }

    // Update is called once per frame

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseOver();
    }


    public void OnMouseOver()
    {
        highLightText.a = 1.0f;
        textColor.color = highLightText;
        gameObject.GetComponent<Image>().sprite = highlightBtn;
    }
    public void OnMouseExit()
    {
        defaultText.a = 1.0f;
        textColor.color = defaultText;
        gameObject.GetComponent<Image>().sprite = defaultBtn;
    }
    public void ClickOption()
    {
        if (isDecline)
        {
           // gameManager.GetComponent<MesureDistance>().isDecline = true;
            popupPanel.SetActive(false);
        }
        else 
        {
            SceneManager.LoadSceneAsync("ConvenienceStore");
            Load("ConvenienceStore");
            /*LoadScene load = new LoadScene();
            load.Load("ConvenienceStore");*/
        }
    }

    public void Load(string name)
    {
        StartCoroutine(LoadYourAsyncScene(name));
    }

    private IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}
