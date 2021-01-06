using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    public void Close()
    {
        Destroy(gameObject);
    }

    public abstract void OnCreate(UIContext context);
}
