using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIContext
{
    public Profile playerProfile;

    public UIContext(Profile playerProfile)
    {
        this.playerProfile = playerProfile;
    }
}

public abstract class UIFactory
{

    public abstract UIView Create(UIContext context);
}
