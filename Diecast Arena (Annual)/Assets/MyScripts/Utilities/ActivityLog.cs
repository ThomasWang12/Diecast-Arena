using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static GameMaster;
using static Dev;

public static class ActivityLog
{
    static GameMaster master;
    static string file;

    public static void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
    }

    public static void Start()
    {
        file = Application.dataPath + "/ActivityLog_" + Methods.DateFormat() + ".txt";
    }

    static void Log(string message)
    {
        string line = Methods.DateTimeFormat() + " - " + message;
        if (!File.Exists(file))
        {
            File.WriteAllText(file, line);
        }
        else
        {
            using var writer = new StreamWriter(file, true);
            writer.AutoFlush = true;
            writer.WriteLine(line);
        }
    }

    public static void Write(logType logType)
    {
        string message = "";
        switch (logType)
        {
            case logType.LocalPlay:
                message = "Local Session";
                break;
            case logType.NetworkPlay:
                message = "Network Session";
                break;
            case logType.RaceLap:
                message = Methods.GetPlayerName() + " completed a lap in the race";
                break;
            case logType.RedLight:
                message = Methods.GetPlayerName() + " passed a red light";
                break;
            case logType.ShowIdleHint:
                message = "Show idle hint 'No input detected. Auto-reset in...'";
                break;
            case logType.HideIdleHint:
                message = "Hide idle hint 'No input detected. Auto-reset in...'";
                break;
            case logType.ManualRelaunch:
                message = "Manual Relaunch";
                break;
            case logType.AutoRelaunch:
                message = "Auto Relaunch";
                break;
            case logType.PassiveRelaunch:
                message = "Passive Relaunch";
                break;
            case logType.DisconnectRelaunch:
                message = "Disconnect Relaunch";
                break;
        }

        Log(message);
    }

    public static void WriteChangePlayerName(string customName)
    {
        Log("Player changed name to " + customName);
    }

    public static void WriteActivityStart(string playerName, int activityIndex, string activityOption)
    {
        Log(ActivityStartExitInfo(logType.ActivityStart, playerName, activityIndex, activityOption));
    }

    public static void WriteActivityExit(string playerName, int activityIndex)
    {
        Log(ActivityStartExitInfo(logType.ActivityExit, playerName, activityIndex, ""));
    }

    public static void WriteActivityResult(string playerName, int activityIndex, string result)
    {
        Log(ActivityResultInfo(playerName, activityIndex, result));
    }

    static string ActivityStartExitInfo(logType logType, string playerName, int activityIndex, string activityOption)
    {
        activityType activityType = master.activityList[activityIndex].type;
        string activityName = master.activityList[activityIndex].name;

        if (activityType == activityType.RaceCircuit || activityType == activityType.RaceDestination)
        {
            if (logType == logType.ActivityStart)
                return playerName + " started Race \"" + activityName + "\" (" + activityOption + " laps)";
            if (logType == logType.ActivityExit)
                return playerName + " exited Race \"" + activityName + "\"";
        }

        if (activityType == activityType.CollectionBattle)
        {
            if (logType == logType.ActivityStart)
                return playerName + " started Collection Battle \"" + activityName + "\" (" + activityOption + " mins)";
            if (logType == logType.ActivityExit)
                return playerName + " exited Collection Battle \"" + activityName + "\"";
        }

        if (activityType == activityType.CarHunt)
        {
            if (logType == logType.ActivityStart)
                return playerName + " started Car Hunt" + " (" + activityOption + " mins)";
            if (logType == logType.ActivityExit)
                return playerName + " exited Car Hunt";
        }

        return "";
    }

    static string ActivityResultInfo(string playerName, int activityIndex, string result)
    {
        activityType activityType = master.activityList[activityIndex].type;
        string activityName = master.activityList[activityIndex].name;

        if (activityType == activityType.RaceCircuit || activityType == activityType.RaceDestination)
        {
            return playerName + " finished Race \"" + activityName + "\" in: " + result;
        }

        if (activityType == activityType.CollectionBattle)
        {
            return playerName + " finished Collection Battle \"" + activityName + "\" with score: " + result;
        }

        if (activityType == activityType.CarHunt)
        {
            return playerName + " finished Car Hunt with points and remaining time: " + result;
        }

        return "";
    }
}
