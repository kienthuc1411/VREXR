using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    [SerializeField] private RawImage avatarImg;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject loadingImage;

    private AvatarElement avatarElement;
    private AvatarListPanelView avatarPanelView;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClick()
    {
        try
        {
            avatarPanelView.ChooseAvatar(avatarElement.id.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public async void LoadImage(AvatarElement avatarElement, AvatarListPanelView avatarPanelView)
    {
        try
        {
            loadingImage.SetActive(true);

            this.avatarElement = avatarElement;
            this.avatarPanelView = avatarPanelView;

            avatarImg.texture = await Request.DownloadImage(avatarElement.image);

            toggleGroup = this.GetComponentInParent<ToggleGroup>();

            this.GetComponent<Toggle>().group = toggleGroup;

            loadingImage.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
