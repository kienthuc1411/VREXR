using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData
{
    private static bool _hasSignedIn = false;
    public static bool hasSignedIn
    {
        get
        {
            return _hasSignedIn;
        }

        set
        {
            _hasSignedIn = value;

            if (value)
            {
                ParseProfileData(Request.cacheLoginRequest);
                SaveSessionToken();
            }
            else
            {
                profile = null;
            }
        }
    }

    private static Profile _profile;

    public static Profile profile
    {
        get
        {
            return _profile;
        }
        set
        {
            _profile = value;
        }
    }

    public static SessionToken sessionToken; // Session token of logged in player
    public static bool pendingSaveToken; // Should PlayerData save token ?
    public static void SaveSessionToken()
    {
        UnityWebRequest loginRequest = Request.cacheLoginRequest;

        if (loginRequest != null)
        {
            sessionToken = new SessionToken(loginRequest, pendingSaveToken);
        }
        else
        {
            Debug.Log("null cache login request!");
        }
    }

    public static void ParseProfileData(UnityWebRequest request)
    {
        string json = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);

        Debug.Log("Parsing json: " + json);

        LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);

        profile = response.data;
    }

    public static void DeleteSessionToken()
    {
        sessionToken = null;
        PlayerPrefs.DeleteKey("access-token");
        PlayerPrefs.DeleteKey("client");
        PlayerPrefs.DeleteKey("uid");
        PlayerPrefs.DeleteKey("expiry");
        PlayerPrefs.DeleteKey("token-type");
    }

}

[Serializable]
public class LoginResponse
{
    public string status = string.Empty;
    public Profile data;
    
}

[Serializable]
public class AvatarsListResponse
{
    public string status = string.Empty;
    public ListCharacterFromJson default_data;
    public ListCharacterFromJson user_data;

}
//ThaoEm
[Serializable]
public class ListCharacterFromJson
{
    public Characters[] data;
}

[Serializable]
public class Profile
{
    public int id = -1;
    public string email = "N/A";
    public string role = "N/A";
    public string address = "N/A";
    public string phone = "N/A";
    public string status = "N/A";
    public string fullname = "N/A";
    public string credit_amount = "N/A";
    public string avatar = "N/A";
    public string avatar_url = "N/A"; //ThaoEm
    public string avatar_thumb_url = "N/A"; //ThaoEm
    //public Characters[] characters = new Characters[3]; //ThaoEm
    public Characters[] characters ; //ThaoEm
    public School school; //ThaoEm
}

[Serializable]
public class AvatarElement
{
    public int id;
    public string name = string.Empty;
    public string image = string.Empty;
}


    

[Serializable]
public class Characters
{
    public int id = -1;
    public string title = string.Empty;
    public string voice = string.Empty;
    public int price = 0;
    public string image = string.Empty;
    public string image_thumb = string.Empty;
    public string image_thumb_light = string.Empty;
    public string vrm = string.Empty;
}
[Serializable]
public class School
{
    public int id = -1;
    public string name = string.Empty;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
    public string manager_name = string.Empty;
    public string phone = string.Empty;
    public string note = string.Empty;

}

[Serializable]
public class SessionToken
{
    public string accessToken = string.Empty;
    public string client = string.Empty;
    public string uid = string.Empty;
    public string expiry = string.Empty;
    public string tokenType = string.Empty;

    public SessionToken()
    {
    }

    public SessionToken(string accessToken, string client, string uid, string expiry, string tokenType)
    {
        this.accessToken = accessToken;
        this.client = client;
        this.uid = uid;
        this.expiry = expiry;
        this.tokenType = tokenType;

        SaveToPlayerPrefs();
    }

    public SessionToken(UnityWebRequest request, bool shouldRemember = false)
    {
        this.accessToken = request.GetResponseHeader("access-token");
        this.client = request.GetResponseHeader("client");
        this.uid = request.GetResponseHeader("uid");
        this.expiry = request.GetResponseHeader("expiry");
        this.tokenType = request.GetResponseHeader("token-type");

        if (shouldRemember)
            SaveToPlayerPrefs();
    }

    public void LoadSessionFromPrefs()
    {
        this.accessToken = PlayerPrefs.GetString("access-token");
        this.client = PlayerPrefs.GetString("client");
        this.uid = PlayerPrefs.GetString("uid");
        this.expiry = PlayerPrefs.GetString("expiry");
        this.tokenType = PlayerPrefs.GetString("token-type");
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString("access-token", accessToken);
        PlayerPrefs.SetString("client", client);
        PlayerPrefs.SetString("uid", uid);
        PlayerPrefs.SetString("expiry", expiry);
        PlayerPrefs.SetString("token-type", tokenType);
    }
}