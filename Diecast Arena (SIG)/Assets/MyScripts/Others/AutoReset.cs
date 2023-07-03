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
    InputManager input;
    PostProcessing postFX;

    [SerializeField] TMP_Text idleHint;
    [SerializeField] float lastInputTime;
    [SerializeField] int autoResetIn;

    // Tunables
    int hintAtSecond = 30; // 30s remaining
    int resetAtSecond = 180; // 3 mins

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        input = master.input;
        postFX = master.postFX;
    }

    void Update()
    {
        switch (input.currentInputType)
        {
            case inputType.MouseKeyboard:
                if (Input.anyKey)
                    lastInputTime = Time.time;
                break;
            case inputType.Gamepad:
                if (input.lastGamepadInputTime > lastInputTime)
                    lastInputTime = input.lastGamepadInputTime;
                break;
        }

        if (master.ready)
        {
            autoResetIn = Mathf.FloorToInt(lastInputTime + resetAtSecond - Time.time);

            if (autoResetIn < hintAtSecond)
            {
                idleHint.enabled = true;
                idleHint.text = "No input detected. Auto-reset in " + autoResetIn + " seconds.";
                postFX.ToggleSaturation(true);
            }
            else
            {
                idleHint.enabled = false;
                postFX.ToggleSaturation(false);
            }

            if (autoResetIn <= 0)
            {
                Relaunch();
            }
        }
        else idleHint.enabled = false;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (input.allowReset)
            {
                Relaunch();
            }
        }
    }

    public static void Relaunch()
    {
        string path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));
        string file = "Diecast Arena (SIG).exe";
        Process.Start(path + file);
        Application.Quit();
    }
}