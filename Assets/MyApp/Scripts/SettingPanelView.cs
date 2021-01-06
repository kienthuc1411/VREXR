using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanelView : UIView
{
    [Header("Avatar")]
    [SerializeField] private RawImage avatarImg;
    [Header("Profile")]
    [SerializeField] private Text playerNameLabel;
    [SerializeField] private Text currencyLabel;
    [SerializeField] private Button editAvatarButton;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.TriggerEvent(GameEvent.SHUFFLE_TO_LAST);
    }
    private void OnEnable()
    {
        OnCreate(new UIContext(PlayerData.profile));
    }
    public override async void OnCreate(UIContext context)
    {
        if (context.playerProfile != null)
        {
            try
            {
                loading.SetActive(true);
                Texture texture = await Request.DownloadImage(PlayerData.profile.avatar_thumb_url);
                avatarImg.texture = texture;
                loading.SetActive(false);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        Setup();

        EventManager.StartListening(GameEvent.ON_PROFILE_CHANGED, UpdateData);
        EventManager.StartListening(GameEvent.ON_CLOSE_AVATAR_LIST, OnCloseAvatarList);
    }

    private async void UpdateData(EventParam obj)
    {
        try
        {
            loading.SetActive(true);
            long code = await Request.GetProfile(PlayerData.sessionToken);
            OnCreate(new UIContext(PlayerData.profile));
            loading.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void Setup()
    {
        try
        {
            Profile profile = PlayerData.profile;
            playerNameLabel.text = profile.fullname;
            currencyLabel.text = profile.credit_amount;
        }
        catch (System.Exception e)
        {
            Debug.Log("Profile null! Have you logged in?....");
        }
    }

    // Event called by Edit button
    private UIView avatarlistView;
    public void OnClickEditAvatar()
    {
        if (avatarlistView == null)
        {
            AvatarListPanelFactory factory = new AvatarListPanelFactory();
            avatarlistView = factory.Create(new UIContext(PlayerData.profile));
            editAvatarButton.interactable = false;
        }
    }

    //public async void _onClickEditAtartAsync()
    //{
    //    AvatarsListResponse lstAvatarRespone = await Request.GetAvatarsList();
    //}   
    private async void OnCloseAvatarList(EventParam obj)
    {
        avatarlistView = null;
        editAvatarButton.interactable = true;
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
        EventManager.TriggerEvent(GameEvent.SHUFFLE_TO_LAST);
        Close();
    }
    public async void OnLogout()
    {
        long logoutResponseCode = await Request.Logout();

        ProcessLogoutResult(logoutResponseCode);

        AudioManager.PlaySound(Sounds.ClickButtonCancel);
    }
    void ProcessLogoutResult(long statusCode)
    {
        // 200 = OK!
        if (statusCode == 200)
        {
            SetStatusLabel("Logout completed! Response code: " + statusCode);
            loadSceneLogin();
            //LoggedOut();
        }
        else
        {
            SetStatusLabel("Logout failed! Response code: " + statusCode);
        }
    }
    void SetStatusLabel(string text)
    {
        statusLabel.gameObject.SetActive(true);
        statusLabel.GetComponentInChildren<Text>().text = text;
        Invoke("hidelogoutInfo", 0.5f);
    }
    void hidelogoutInfo()
    {
        statusLabel.SetActive(false);
    }
    void loadSceneLogin()
    {
        SceneManager.LoadScene(0);
    }
}
