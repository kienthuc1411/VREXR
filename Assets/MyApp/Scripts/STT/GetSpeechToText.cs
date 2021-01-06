using UnityEngine;
using UnityEngine.UI;

public class GetSpeechToText : MonoBehaviour
{
    public Text sttText;
    [HideInInspector]
    public string finalResult;
    [HideInInspector]
    public string outputInterim;
    [HideInInspector]
    public bool isFinalResult;

    private void Start()
    {
        finalResult = "";
        outputInterim = "";
        isFinalResult = false;
    }

    private void Update()
    {
        if (isFinalResult)
        {
            sttText.text = finalResult;
        }
        else
        {
            sttText.text = outputInterim;
        }
    }
}
