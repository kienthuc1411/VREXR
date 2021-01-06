using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem_TED : MonoBehaviour
{
    public RawImage avatar;
    public string avatarURL;
    public int avatarID;
    public TMP_Dropdown dropDown;
    public AvatarEditViewTED avatarEditViewTED;
    private Texture tempAvatar;

    private void Start()
    {
        tempAvatar = null;
    }

    public async void onChangeValueItemAvatar(bool check)
    {
        if (check)
        {
            //show loading status
            avatar.transform.GetChild(0).gameObject.SetActive(true);
            if (tempAvatar == null)
                if (!string.IsNullOrEmpty(avatarURL))
                {
                    avatar.texture = await Request.DownloadImage(avatarURL);
                    tempAvatar= avatar.texture;
                }
                else { }
            else
                avatar.texture = tempAvatar;
            //hide loading status
            avatar.transform.GetChild(0).gameObject.SetActive(false);

            dropDown.value = 0;
            avatarEditViewTED.AvatarIDSelected = avatarID;
        }
    }
}
