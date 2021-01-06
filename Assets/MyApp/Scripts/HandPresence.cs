using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedHanmodel;
    private Animator handAnimator;

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
            spawnedHanmodel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHanmodel.GetComponent<Animator>();
        }
    }

    private void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }

        if(targetDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 value))
        {

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
            UpdateHandAnimation();
        }
    }
}
