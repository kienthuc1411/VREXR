using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarListPanelView : UIView
{
    [SerializeField] private GameObject avatarItemPrefab;
    [SerializeField] private Transform listSpawnPoint;
    [SerializeField] private GameObject loading;
    [SerializeField] private string pendingAvatarID = string.Empty;

    void Start()
    {
        EventManager.TriggerEvent(GameEvent.SHUFFLE_TO_LAST);
    }

    //public override async void OnCreate(UIContext context)
    //{
    //    if(context.playerProfile == null)
    //    {
    //        Debug.Log("AvatarListPanelView canceled!");
    //        return;
    //    }

    //    AvatarsListResponse code = await Request.GetAvatarsList();

    //    for (int i = 0; i < code.data.Length; i++)
    //    {
    //        GameObject go = Instantiate(avatarItemPrefab);

    //        AvatarItem avatarItem = go.GetComponent<AvatarItem>();

    //        if (avatarItem)
    //        {
    //            avatarItem.LoadImage(code.data[i],this);
    //            go.transform.SetParent(listSpawnPoint);
    //            go.transform.localScale = Vector3.one;
    //        }
    //    }

    //    listSpawnPoint.GetComponent<RectTransform>().sizeDelta = new Vector2(0, code.data.Length / 2);
    //    loading.SetActive(false);
    //}

    public void ChooseAvatar(string id)
    {
        pendingAvatarID = id;
    }

    public async void OnClickConfirm()
    {
        loading.SetActive(true);
        //long code = await Request.UpdateAvatar(int.Parse(pendingAvatarID));
        long code = await Request.UpdateAvatar(int.Parse(pendingAvatarID),PlayerData.profile.id);
        loading.SetActive(false);
    }

    public void OnClickCLose()
    {
        gameObject.SetActive(false);
        EventManager.TriggerEvent(GameEvent.ON_CLOSE_AVATAR_LIST);
        EventManager.TriggerEvent(GameEvent.SHUFFLE_TO_LAST);
        Close();
    }

    public override void OnCreate(UIContext context)
    {
      
    }
}
