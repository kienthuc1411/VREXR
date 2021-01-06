using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColliderController : MonoBehaviour
{
    public GameObject UI;
    public GameObject Effect;
    public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetComponent<ContinuousMoveProviderBase>().moveSpeed = 0; //user can't move
            EnableGameObject(UI, true);
            EnableGameObject(Effect, false);
        }
    }

    public void EnableGameObject(GameObject gameObject, bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void OnClickCancel()
    {
        Player.gameObject.GetComponent<ContinuousMoveProviderBase>().moveSpeed = 1; //user can move
        EnableGameObject(UI, false);
        EnableGameObject(Effect, true);
    }


}