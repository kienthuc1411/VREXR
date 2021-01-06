using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FinalScore 
{
    private static int score;
    public static int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
        }
    }

}

public static class GetScenarioFromServer
{
    private static int id;
    public static int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }
}

public static class SelectScenarioFromServer
{
    private static int id;
    public static int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }
}

public static class HoverScenarioGetData
{
    public static Scenario scenario;
}

