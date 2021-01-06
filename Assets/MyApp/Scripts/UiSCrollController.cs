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

    private void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
    }


    private void Update()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 value))
        {
            inputValue = value;
            scollRect.verticalNormalizedPosition = inputValue.y;
        }
    }
}
