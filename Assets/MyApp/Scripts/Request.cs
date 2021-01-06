using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Request
{
    public string receiveData = "";
    public bool isPostDataSuccess = false;
    public IEnumerator request(string url ,string data , string method = "GET")
    {
        isPostDataSuccess = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        UnityWebRequest webRequest = new UnityWebRequest(url, method);
        webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError)
        {
            Debug.LogError("Networking error!");
        }
        if (webRequest.isDone)
        {
             this.receiveData = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
            isPostDataSuccess = true;
            Debug.Log(this.receiveData);
        }
    }

 


    /// <summary>
    ///  API Region
    /// </summary>
    /// Refer to https://documenter.getpostman.com/view/8755358/Szzj7d6M#cc8cf5a9-2bba-4f8e-840e-c8bc8e3d5ab7 for API document
    /// 
    const string LOGIN_URL = "https://vrelearning.online/api/v1/auth/sign_in";
    const string VALIDATE_TOKEN = "https://vrelearning.online/api/v1/auth/validate_token";
    //const string AVATARS_LIST = "https://vrelearning.online/api/v1/avatars"; //
    const string AVATARS_LIST = "https://vrelearning.online/api/v1/characters"; //vrelearning.online/api/v1/characters
    const string LOGOUT_URL = "https://vrelearning.online/api/v1/auth/sign_out";
    const string GET_PROFILE = "https://vrelearning.online/api/v1/users/get_profile";
    //const string UPDATE_AVATAR = "https://vrelearning.online/api/v1/users/update_avatar";
    const string UPDATE_AVATAR = "https://vrelearning.online/api/v1/users/update_character_avatar";
    public static UnityWebRequest cacheLoginRequest;

    public static async Task<long> Login(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(LOGIN_URL, form);
        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Login response code:" + www.responseCode);
            cacheLoginRequest = www;

            PlayerData.hasSignedIn = true;
        }
        return www.responseCode;
    }

    public static async Task<long> ValidateToken(SessionToken sessionToken)
    {
        UnityWebRequest www = UnityWebRequest.Get(VALIDATE_TOKEN);

        www.SetRequestHeader("access-token", sessionToken.accessToken);
        www.SetRequestHeader("client", sessionToken.client);
        www.SetRequestHeader("uid", sessionToken.uid);
        www.SetRequestHeader("expiry", sessionToken.expiry.ToString());
        www.SetRequestHeader("token-type", sessionToken.tokenType);

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("ValidateToken response code:" + www.responseCode);
            cacheLoginRequest = www;

            PlayerData.hasSignedIn = true;
        }
        return www.responseCode;
    }

    public static async Task<AvatarsListResponse> GetAvatarsList()
    {
        UnityWebRequest www = UnityWebRequest.Get(AVATARS_LIST);

        AvatarsListResponse avatarListResponse = null;

        www.SetRequestHeader("access-token", cacheLoginRequest.GetResponseHeader("access-token"));
        www.SetRequestHeader("client", cacheLoginRequest.GetResponseHeader("client"));
        www.SetRequestHeader("uid", cacheLoginRequest.GetResponseHeader("uid"));
        www.SetRequestHeader("expiry", cacheLoginRequest.GetResponseHeader("expiry"));
        www.SetRequestHeader("token-type", cacheLoginRequest.GetResponseHeader("token-type"));
        long p = await ValidateToken(PlayerData.sessionToken) ;

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("GetAvatarsList response code:" + www.responseCode);

            string json = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            avatarListResponse = JsonUtility.FromJson<AvatarsListResponse>(json); 
        }

        return avatarListResponse;
    }

    public static async Task<long> Logout()
    {
        UnityWebRequest www = UnityWebRequest.Delete(LOGOUT_URL);

        www.SetRequestHeader("access-token", cacheLoginRequest.GetResponseHeader("access-token"));
        www.SetRequestHeader("client", cacheLoginRequest.GetResponseHeader("client"));
        www.SetRequestHeader("uid", cacheLoginRequest.GetResponseHeader("uid"));
        www.SetRequestHeader("expiry", cacheLoginRequest.GetResponseHeader("expiry"));
        www.SetRequestHeader("token-type", cacheLoginRequest.GetResponseHeader("token-type"));

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Logout response code:" + www.responseCode);

            PlayerData.hasSignedIn = false;

            PlayerData.DeleteSessionToken();
        }
        return www.responseCode;
    }
    public static async Task<long> GetProfile(SessionToken sessionToken)
    {
        UnityWebRequest www = UnityWebRequest.Get(GET_PROFILE);

        www.SetRequestHeader("access-token", sessionToken.accessToken);
        www.SetRequestHeader("client", sessionToken.client);
        www.SetRequestHeader("uid", sessionToken.uid);
        www.SetRequestHeader("expiry", sessionToken.expiry.ToString());
        www.SetRequestHeader("token-type", sessionToken.tokenType);

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("GetProfile response code:" + www.responseCode);
            PlayerData.ParseProfileData(www);
        }

        return www.responseCode;
    }

    public static async Task<long> UpdateAvatar(int avatarId, int user_id)
    {
        WWWForm form = new WWWForm();
        //form.AddField("avatar_id", avatarId);
        form.AddField("character_type", "default");
        form.AddField("character_id", avatarId);
        form.AddField("user_id", user_id);
        
            

        UnityWebRequest www = UnityWebRequest.Post(UPDATE_AVATAR, form);

        www.SetRequestHeader("access-token", cacheLoginRequest.GetResponseHeader("access-token"));
        www.SetRequestHeader("client", cacheLoginRequest.GetResponseHeader("client"));
        www.SetRequestHeader("uid", cacheLoginRequest.GetResponseHeader("uid"));
        www.SetRequestHeader("expiry", cacheLoginRequest.GetResponseHeader("expiry"));
        www.SetRequestHeader("token-type", cacheLoginRequest.GetResponseHeader("token-type"));

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("UpdateAvatar response code:" + www.responseCode);
            EventManager.TriggerEvent(GameEvent.ON_PROFILE_CHANGED);
        }

        return www.responseCode;
    }
    public static async Task<Texture> DownloadImage(string URL)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL);

        await www.SendWebRequest();

        Texture texture = null;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Retrieved Image code " + www.responseCode);
            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        return texture;
    }
}
