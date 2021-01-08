using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollToBottom : MonoBehaviour
{
    int count = 0;
    public ScrollRect scrollRect;


    private void Start()
    {
        count = transform.childCount;
    }

    private void Update()
    {
        if(count != transform.childCount)
        {
            
            StartCoroutine(ForceScrollDown());

        }
    }
    IEnumerator ForceScrollDown()
    {
        // Wait for end of frame AND force update all canvases before setting to bottom.
        yield return new WaitForSeconds(0.3f);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
        count = transform.childCount;
    }
}
