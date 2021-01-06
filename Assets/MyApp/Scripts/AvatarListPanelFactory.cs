using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarListPanelFactory : UIFactory
{
    public override UIView Create(UIContext context)
    {
        //Look for Theater
        Theater theater = GameObject.FindObjectOfType<Theater>();

        AvatarListPanelView panel = GameObject.Instantiate(Resources.Load("Project/UI/Avatar/AvatarListPanelView") as GameObject, theater.grid).GetComponent<AvatarListPanelView>();

        panel.OnCreate(context);

        return panel;
    }

}
