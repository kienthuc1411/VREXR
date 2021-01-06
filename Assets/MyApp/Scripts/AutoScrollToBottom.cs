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
            scrollRect.verticalScrollbar.value = 0;
            count = transform.childCount;
        }
    }
}
