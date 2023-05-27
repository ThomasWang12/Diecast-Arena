using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class Misc : MonoBehaviour
{
    GameMaster master;
    InputManager input;
    UIManager UI;

    bool toggleQuit = false;
    bool toggleConsole = false;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        input = master.input;
        UI = master.UI;

        // Help static classes to call Awake() for their initialization
        Methods.Awake();
        CheckpointCol.Awake();
    }

    void Start()
    {
        GameObject.Find("IngameDebugConsole").GetComponent<Canvas>().enabled = toggleConsole;
    }

    void Update()
    {
        // Quit game
        UI.msg_quitGame = toggleQuit;
        if (master.currentState == gameState.Session)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                toggleQuit = !toggleQuit;

            if (toggleQuit && Input.GetKeyDown(KeyCode.Return))
                Application.Quit();
        }
        else toggleQuit = false;

        // Debug console visibility
        if (input.ToggleConsole())
        {
            toggleConsole = !toggleConsole;
            GameObject.Find("IngameDebugConsole").GetComponent<Canvas>().enabled = toggleConsole;
        }
    }
}
