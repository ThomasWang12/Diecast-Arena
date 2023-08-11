using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using static InputManager;

public class AutoReset : MonoBehaviour
{
    GameMaster master;
    PlayerNetwork network;
    InputManager input;
    PostProcessing postFX;

    [SerializeField] TMP_Text idleHint;
    [SerializeField] float lastInputTime;
    [SerializeField] int autoResetIn;
    public bool autoResetReached = false;

    // Tunables
    int hintAtSecond = 30; // 30s remaining
    int resetAtSecond = 180; // 3 mins

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        network = master.network;
        input = master.input;
        postFX = master.postFX;
    }

    void Update()
    {
        switch (input.currentInputType)
        {
            case inputType.MouseKeyboard:
                if (Input.anyKey)
                {
                    lastInputTime = Time.time;
                    if (autoResetReached) AutoResetState(false);
                }
                break;
            case inputType.Gamepad:
                if (input.lastGamepadInputTime > lastInputTime)
                {
                    lastInputTime = input.lastGamepadInputTime;
                    if (autoResetReached) AutoResetState(false);
                }
                break;
        }

        if (master.ready)
        {
            autoResetIn = Mathf.FloorToInt(lastInputTime + resetAtSecond - Time.time);

            if (autoResetIn < hintAtSecond)
            {
                if (!idleHint.enabled) ActivityLog.Write(Dev.logType.ShowIdleHint);

                idleHint.enabled = true;
                idleHint.text = "No input detected. Auto-reset in " + autoResetIn + " seconds.";
                postFX.ToggleSaturation(true);
            }
            else
            {
                if (idleHint.enabled) ActivityLog.Write(Dev.logType.HideIdleHint);

                idleHint.enabled = false;
                postFX.ToggleSaturation(false);
            }

            if (autoResetIn <= 0)
            {
                // %& Local Play / Network
                if (network.localPlay)
                    Relaunch(Dev.logType.AutoRelaunch);
                else
                {
                    try
                    {
                        // Try to tell the server this player is available for relaunch
                        AutoResetState(true);
                    }
                    catch
                    {
                        // If cannot tell the server, relaunch itself
                        Relaunch(Dev.logType.AutoRelaunch);
                    }
                }
            }
        }
        else idleHint.enabled = false;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (input.allowReset)
            {
                // %& Local Play / Network
                if (network.localPlay)
                    Relaunch(Dev.logType.ManualRelaunch);
                else
                {
                    try
                    {
                        // Try to tell the server to relaunch the game for all players
                        network.RelaunchServerRpc();
                    }
                    catch
                    {
                        // If cannot tell the server, relaunch itself
                        Relaunch(Dev.logType.ManualRelaunch);
                    }
                }
            }
        }
    }

    void AutoResetState(bool state)
    {
        autoResetReached = state;
        network.AutoResetStateServerRpc(network.ownerPlayerId, state);
    }

    public static void Relaunch(Dev.logType type)
    {
        if (type == Dev.logType.ManualRelaunch) ActivityLog.Write(Dev.logType.ManualRelaunch);
        if (type == Dev.logType.AutoRelaunch) ActivityLog.Write(Dev.logType.AutoRelaunch);
        if (type == Dev.logType.PassiveRelaunch) ActivityLog.Write(Dev.logType.PassiveRelaunch);

        string path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));
        Process.Start(path + Common.fileName);
        Application.Quit();
    }
}