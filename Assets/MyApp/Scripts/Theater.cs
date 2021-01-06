using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Theater : MonoBehaviour
{
    [Header("Editor")]
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [Header("VR")]
    [SerializeField] private OVRRaycaster ovrRaycaster;

    [Header("------------------------------")]
    [SerializeField] private Canvas canvas;
    public Transform grid;
    private void Awake()
    {
        EventManager.StartListening(GameEvent.UPDATE_CAMERA_POSITION, OnUpdateCameraPosition);
        EventManager.StartListening(GameEvent.SHUFFLE_TO_LAST, ShuffleToLast);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            ShuffleToLast(null);
        }
    }

    void ShuffleToLast(EventParam evtParam)
    {
        UIView[] views = grid.GetComponentsInChildren<UIView>();

        float offset = 0;
        float lastWidth = 0;

        foreach(UIView view in views)
        {
            float width = view.GetComponent<RectTransform>().sizeDelta.x;
            lastWidth = width;
            offset += width;
        }
        offset -= lastWidth/2;

        grid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0 - offset, grid.GetComponent<RectTransform>().anchoredPosition.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        //try
        //{
        //    // Look for OVRCameraRig
        //    OVRCameraRig cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();

        //    //Replace canvas event camera
        //    Camera cam = cameraRig.centerEyeAnchor.GetComponent<Camera>();
        //    canvas.worldCamera = cam;
        //}
        //catch(System.Exception e)
        //{
        //    Debug.LogError("Exception! " + e.Message);
        //}
    }

    void OnUpdateCameraPosition(EventParam evt)
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.25f;
        transform.rotation = Camera.main.transform.rotation;
      //  transform.position = new Vector3(transform.position.x, transform.position.y,1.6f);
    }

    void OnEditor()
    {
        //if(graphicRaycaster)
        //{
        //    graphicRaycaster.enabled = true;
        //}

        //if (ovrRaycaster)
        //{
        //    ovrRaycaster.enabled = false;
        //}
    }

    void OnVR()
    {
        //if (graphicRaycaster)
        //{
        //    graphicRaycaster.enabled = false;
        //}

        //if (ovrRaycaster)
        //{
        //    ovrRaycaster.enabled = true;
        //}
    }

    public void AddPanel(UIView uiView)
    {
     
    }
}
