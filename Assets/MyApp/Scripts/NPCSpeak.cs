using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NPCSpeak:MonoBehaviour
{
    [SerializeField]
    private AudioSource audio;
    public bool isFinish = false;
    public bool isSpeaking = false;
    private bool isStop = false;
    private UnityWebRequest www;
    public IEnumerator ReadySpeaking(string speech)
    {
        isFinish = false;
        if (!isStop)
        {
            isSpeaking = true;
            www = new UnityWebRequest();
            string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=" + speech + "&tl=Ja-gb";
            using (www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                AudioSource.PlayClipAtPoint(myClip, transform.position, 10f);
                yield return new WaitForSeconds(myClip.length + 1f); //ThaoEm
                //yield return new WaitForSeconds(5.5f);
                isFinish = true;
                isSpeaking = false;

            }
        }
    }

    public IEnumerator RealSpeaking(string url)
    {
        isFinish = false;
        if (!isStop)
        {
            isSpeaking = true;
            www = new UnityWebRequest();
            using (www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                yield return www.SendWebRequest();
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                AudioSource.PlayClipAtPoint(myClip, transform.position, 10f);
                yield return new WaitForSeconds(myClip.length + 1f); //ThaoEm
                //yield return new WaitForSeconds(5.5f); 
                isFinish = true;
                isSpeaking = false;
            }
        }
    }

    public void Speaking(ScenarioDetail scenarioDetail, bool isRealVoice = false)
    {
        if (scenarioDetail.detail_ja.Length > 0)
        {
            if(isRealVoice)
            {
                StartCoroutine(RealSpeaking(scenarioDetail.voice_url));
            }
            else
            {
                StartCoroutine(ReadySpeaking(scenarioDetail.detail_ja));
            }
        }
    }

    public void StopLoadSpeech()
    {
        AbortDownload();
        StopAllCoroutines();
    }
    public void AbortDownload()
    {
        if (www != null && !www.isDone)
        {
            Debug.Log("Aborting download ...", this);
            www.disposeDownloadHandlerOnDispose = true;
            www.Dispose();
            www.Abort();
        }
        else
        {
            Debug.Log("Not downloading, nothing to do ...", this);
        }
    }
}
