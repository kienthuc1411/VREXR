using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostScoreToCms
{
    // int player_history_id ,int scenario_detail_id,string speech_text,string voice,int score
    public int player_history_id;
    public int scenario_detail_id;
    public string speech_text;
    public string voice;
    public int score;
    public PostScoreToCms(int player_history_id, int scenario_detail_id, string speech_text, string voice, int score)
    {
        this.player_history_id = player_history_id;
        this.scenario_detail_id = scenario_detail_id;
        this.speech_text = speech_text;
        this.voice = voice;
        this.score = score;
    }
}