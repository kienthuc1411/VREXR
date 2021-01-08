using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public class UiSCrollController : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    private InputDevice targetDevice;
    public ScrollRect scollRect;
    private Vector2 inputValue;
    public float speed = 0.5f;
    //public Text Text;

    private void Start()
    {
        TryInitialize();
    }

    private void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }
    private void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            Scroll();
        }
    }

    private void Scroll()
    {
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 inputValue))
        {
            //Text.text = inputValue.ToString();
            if(inputValue.y != 0)
                scollRect.normalizedPosition += inputValue * ( Time.deltaTime * speed);
         
        }
    }

   
}
