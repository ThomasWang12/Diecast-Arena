using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HuntActivity : MonoBehaviour
{
    GameMaster master;
    InputManager input;
    SoundManager sound;
    UIManager UI;
    public int duration = 60; // seconds
    int activityIndex;
    Vector3 startPos;
    Quaternion startRot;
    bool initialized = false;

    public enum playerRole { Initial, Target, Hunter };

    [Space(10)]

    public bool started = false;
    public bool finished = false;
    float startTime = 0;
    public int point = 15;
    bool endCountdown = false;

    /* Tunables */
    int pointDeduct = 3;

    void Awake()
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
        input = master.ManagerObject(Manager.type.input).GetComponent<InputManager>();
        sound = master.ManagerObject(Manager.type.sound).GetComponent<SoundManager>();
        UI = master.ManagerObject(Manager.type.UI).GetComponent<UIManager>();
    }

    void Start()
    {
        activityIndex = master.ActivityObjectToIndex(gameObject);
        startPos = transform.Find("[Start Position]").position;
        startRot = transform.Find("[Start Position]").rotation;
    }

    void Update()
    {
        if (started && !finished)
        {
            // ...
        }
    }

    public void InitializeActivity()
    {
        master.TeleportPlayer(startPos + Common.spawnHeightOffset, startRot);
        UI.InfoCollectUI(point);
    }

    public void StartActivity()
    {
        if (started) return;

        started = true;
        startTime = Time.time;
        input.EnableDrive();
    }

    public void Reset()
    {
        initialized = false;
        started = false;
        finished = false;
        startTime = 0;
        endCountdown = false;
    }
}
