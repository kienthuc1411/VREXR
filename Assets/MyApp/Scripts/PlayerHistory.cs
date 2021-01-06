using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHistory
{
    public int user_id;
    public int scenario_id;
    public PlayerHistory(int user_id, int scenario_id)
    {
        this.user_id = user_id;
        this.scenario_id = scenario_id;
    }
}

public class ReceivePlayerHistory
{
    public int player_history_id;
    public string status;
}