using JSAM;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanelView : UIView
{
    // Unused
    LoginPanelModel model;

    [Header("Input Fields")]
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;

    [Header("----------------")]
    [SerializeField] private GameObject statusLabel;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Toggle rememberToggle;

    void Start()
    {
        EventManager.TriggerEvent(GameEvent.SHUFFLE_TO_LAST);
    }

    // Start is called before the first frame update
    private void Update()
    {
        if(string.IsNullOrWhiteSpace(usernameInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            loginButton.interactable = false;
        }else
        {
            loginButton.interactable = true;
        }
    }

    public override async void OnCreate(UIContext context)
    {
        string accessToken = PlayerPrefs.GetString("access-token");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            Debug.Log("You have access token stored!");
            SessionToken sessionToken = new SessionToken();
            sessionToken.LoadSessionFromPrefs();

            long loginResponseCode = await Request.ValidateToken(sessionToken);
            ProcessLoginResult(loginResponseCode);
        }

        HideStatusLabel();

        EventManager.TriggerEvent(GameEvent.UPDATE_CAMERA_POSITION);
    }

    /// <summary>
    /// Event called by button login
    /// </summary>
    public async void OnLogin()
    {
        string email = usernameInputField.text;
        string password = passwordInputField.text;

        AudioManager.PlaySound(Sounds.Click);

        long loginResponseCode = await Request.Login(email, password);

        ProcessLoginResult(loginResponseCode);
    }

    /// <summary>
    /// Event called by button lougout
    /// </summary>
    public async void OnLogout()
    {
        long logoutResponseCode = await Request.Logout();

        ProcessLogoutResult(logoutResponseCode);

        AudioManager.PlaySound(Sounds.ClickButtonCancel);
    }

    /// <summary>
    /// Test login
    /// </summary>
    public async void OnTest()
    {
        usernameInputField.text = "student@example.com";
        passwordInputField.text = "changeme";
    }

    void ProcessLoginResult(long statusCode)
    {
        // 200 = OK!
        if(statusCode == 200)
        {
            //SetStatusLabel("Login completed! Response code: " + statusCode);

            LoggedIn();

            //SceneManager.LoadScene("AvatarScene");
        }
        else
        {
            SetStatusLabel("メールアドレスまたはパスワードが正しくありません。");

            PlayerData.DeleteSessionToken();
        }
    }

    void ProcessLogoutResult(long statusCode)
    {
        // 200 = OK!
        if (statusCode == 200)
        {
            SetStatusLabel("Logout completed! Response code: " + statusCode);

            LoggedOut();
        }
        else
        {
            SetStatusLabel("Logout failed! Response code: " + statusCode);
        }
    }

    private void OnEnable()
    {
        if(PlayerData.hasSignedIn)
        {
            LoggedIn();
        }
        else
        {
            LoggedOut();
        }
    }

    void LoggedIn()
    {
        //usernameInputField.text = PlayerData.profile.email;
        usernameInputField.interactable = false;
        passwordInputField.interactable = false;

        SceneManager.LoadSceneAsync("CityScene");
    }

   void LoggedOut()
    {
        usernameInputField.interactable = true;
        passwordInputField.interactable = true;
    }

    void SetStatusLabel(string text)
    {
        statusLabel.gameObject.SetActive(true);
        statusLabel.GetComponentInChildren<Text>().text = text;
    }

    void HideStatusLabel()
    {
        statusLabel.gameObject.SetActive(false);
    }
}
