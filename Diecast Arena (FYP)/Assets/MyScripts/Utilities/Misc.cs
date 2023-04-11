using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc : MonoBehaviour
{
    bool toggleConsole = true;

    void Awake()
    {
        // Help static classes to call Awake() for their initialization
        CheckpointCol.Awake();
    }

    void Update()
    {
        // Debug console visibility
        if (Input.GetKeyDown(KeyCode.F1))
        {
            toggleConsole = !toggleConsole;
            GameObject.Find("IngameDebugConsole").GetComponent<Canvas>().enabled = toggleConsole;
        }
    }
}
