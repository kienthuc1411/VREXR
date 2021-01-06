using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    private Dictionary<string, Action<EventParam>> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.Log("There needs to be one active EventManger script on a GameObject in your scene.Creating one...");
                    GameObject newObj = new GameObject();
                    newObj.name = "Event Manager";
                    eventManager = newObj.AddComponent<EventManager>();
                    eventManager.Init();
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action<EventParam>>();
        }
    }

    public static void StartListening(string eventName, Action<EventParam> listener)
    {
        if(instance == null)
        {
            return;
        }

        Action<EventParam> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Add more event to the existing one
            thisEvent += listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            //Add event to the Dictionary for the first time
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<EventParam> listener)
    {
        if (eventManager == null) return;
        Action<EventParam> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Remove event from the existing one
            thisEvent -= listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, object obj, EvtCallback callback = null)
    {
        EventParam param = new EventParam();
        param.data = obj;
        TriggerEvent(eventName, param, callback);
    }

    public static void TriggerEvent(string eventName, EventParam eventParam = null, EvtCallback callback = null)
    {
        Action<EventParam> thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            if (eventParam == null)
            {
                eventParam = new EventParam();
            }

            if (callback != null)
            {
                eventParam.callback = callback;
            }
            thisEvent.Invoke(eventParam);
            // OR USE  instance.eventDictionary[eventName](eventParam);
        }
    }

    public delegate void EvtCallback(System.Object data = null); // declare delegate type
    protected EvtCallback Callback; // to store the function
}

//Re-usable structure/ Can be a class to. Add all parameters you need inside it
public class EventParam
{
    public System.Object data;
    public EventManager.EvtCallback callback;
}