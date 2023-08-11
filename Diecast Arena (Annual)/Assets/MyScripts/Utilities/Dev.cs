using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public static class Dev
{
    public static bool log_gameState = false;
    public static bool log_inputType = false;

    public enum logType
    {
        LocalPlay, NetworkPlay, PlayerName,
        ActivityStart, ActivityExit,
        RaceLap, RedLight, ShowIdleHint, HideIdleHint,
        ManualRelaunch, AutoRelaunch, PassiveRelaunch, DisconnectRelaunch
    };
}

public static class Manager
{
    public enum type { network, input, vehicle, sound, UI, postFX, common };
}
