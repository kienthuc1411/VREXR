using System;

[Serializable]
public class Scenario
{
    public int id;
    public string title;
    public string description;
    public string opening_text;
    public string play_count;
    public string voice_type;
    public string scene_key;
    public string level;
    public string thumbnail_image;
    public string editor;
    public string experience_time;
    public string created_at;
    public string[] tag;
    public bool is_played;
}