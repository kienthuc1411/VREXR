using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLAPI
{
    public string URL_CMS;
    public string URL_AI;
    public string endpointGetOptionCMS;
    public string endpointPostOptionCMS;
    public string endpointPostScriptAI;
    public string endpointCreatePlayerRecord;
    public string endpointCreatePlayerDetail;

    public URLAPI()
    {
        //  http://13.212.22.80/vre/api/v1.0/stt
        //this.URL_CMS = "https://18.139.121.226/api/v1";
        this.URL_CMS = "https://vrelearning.online/api/v1";
        //this.URL_CMS = "https://192.168.1.30:3000/api/v1";
        //this.URL_AI = "https://3.24.242.118/vre/api/v1.0";// server cũ
        //this.URL_AI = "https://18.139.121.226/vre/api/v1.0"; // server mới
        this.URL_AI = "https://vrelearning.online/vre/api/v1.0"; // server mới
        this.endpointGetOptionCMS = "scenarios";
        this.endpointPostOptionCMS = "scenarios/get_details_scenario";
        this.endpointPostScriptAI = "stt";
        this.endpointCreatePlayerRecord = "player_histories";
        this.endpointCreatePlayerDetail = "player_history_details";
    }
}