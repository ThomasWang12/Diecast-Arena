using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HuntActivity : MonoBehaviour
{
    GameMaster master;
    PlayerNetwork network;
    InputManager input;
    SoundManager sound;
    UIManager UI;
    ActivityOption activityOption;

    public int duration = 60; // seconds
    int activityIndex;
    Vector3 startPos;
    Quaternion startRot;
    //bool initialized = false;

    public enum playerRole { Initial, Target, Hunter };

    [Space(10)]

    public bool started = false;
    public bool finished = false;
    float startTime = 0;
    public int point = 0;
    float underSpeedTime;
    bool minSpeedEnabled = false;
    bool endCountdown = false;

    /* Tunables */
    float minSpeedEnableWaitDuration = 10.0f;
    int minSpeed = 20;
    int pointLimit = 15;
    int pointRedLight = 3;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        network = master.network;
        input = master.input;
        sound = master.sound;
        UI = master.UI;
    }

    void Start()
    {
        activityIndex = master.ActivityObjectToIndex(gameObject);
    }

    void Update()
    {
        if (started && !finished)
        {
            float remainingTime = duration - (Time.time - startTime);
            UI.UpdateHuntUI(pointLimit - point, remainingTime);

            if (minSpeedEnabled)
            {
                if (master.playerSpeed < minSpeed)
                {
                    if (!endCountdown)
                    {
                        underSpeedTime = Time.time + 5.0f;
                        UI.huntSpeedLimitTMP.enabled = true;
                        UI.huntSpeedLimitTMP.text = "Stay above 20 km/h!!";
                        UI.ActivityCountdown5("Play");
                        sound.Play(Sound.name.Countdown5);
                        endCountdown = true;
                    }
                    else
                    {
                        if (Time.time >= underSpeedTime)
                        {
                            // Car Hunt finished (Busted)
                            finished = true;
                            master.FinishActivity(activityIndex);
                            UI.ActivityCountdown5("Initial");
                            UI.ActivityCountdown("BUSTED");
                            UI.ResultHuntUI(activityIndex, point, false, 0);
                            sound.Play(Sound.name.GameLose);

                            // In case it is during countdown when finishing
                            UI.ActivityCountdown5("Initial");
                            sound.Stop(Sound.name.Countdown5);

                            ActivityLog.WriteActivityResult(Methods.GetPlayerName(), activityIndex, point + ", " + Methods.TimeFormat(remainingTime, true));
                        }
                    }
                }
                else
                {
                    UI.huntSpeedLimitTMP.enabled = false;
                    UI.ActivityCountdown5("Initial");
                    sound.Stop(Sound.name.Countdown5);
                    endCountdown = false;
                }
            }
            else
            {
                float remaining = startTime + minSpeedEnableWaitDuration - Time.time;
                if (remaining >= 0)
                {
                    UI.huntSpeedLimitTMP.enabled = true;
                    UI.huntSpeedLimitTMP.text = "Minimum speed " + minSpeed + " km/h enables in " + Methods.TimeFormat(remaining, false);
                }
                else
                {
                    minSpeedEnabled = true;
                    UI.huntSpeedLimitTMP.enabled = false;
                }
            }

            if (remainingTime <= 0)
            {
                // Car Hunt finished (Time's up)
                finished = true;
                master.FinishActivity(activityIndex);
                UI.ActivityCountdown5("Initial");
                UI.ActivityCountdown("FINISH");
                UI.ResultHuntUI(activityIndex, point, true, 0);
                sound.Play(Sound.name.CheckpointBold);

                // In case it is during countdown when finishing
                UI.ActivityCountdown5("Initial");
                sound.Stop(Sound.name.Countdown5);

                ActivityLog.WriteActivityResult(Methods.GetPlayerName(), activityIndex, point + ", " + Methods.TimeFormat(remainingTime, true));
            }
        }
    }

    public void InitializeActivity()
    {
        startPos = Methods.GetStartPosition(transform.Find("[Start Position]").gameObject, network.ownerPlayerId).transform.position;
        startRot = Methods.GetStartPosition(transform.Find("[Start Position]").gameObject, network.ownerPlayerId).transform.rotation;
        master.TeleportPlayer(startPos + Common.spawnHeightOffset, startRot);
        UI.InfoCollectUI(point);
        //initialized = true;
    }

    public void StartActivity()
    {
        if (started) return;

        started = true;
        startTime = Time.time;
        input.EnableDrive();
        UI.huntSpeedLimitTMP.enabled = true;

        string logActivityOption = activityOption.HuntOption(activityOption.currentHuntDuration).ToString();
        ActivityLog.WriteActivityStart(Methods.GetPlayerName(), activityIndex, logActivityOption);
    }

    public void RecordPoint()
    {
        point += pointRedLight;

        if (point >= pointLimit)
        {
            // Car Hunt finished (No points remaining)
            finished = true;
            master.FinishActivity(activityIndex);
            float remainingTime = duration - (Time.time - startTime);
            UI.ResultHuntUI(activityIndex, point, false, remainingTime);
            sound.Play(Sound.name.GameLose);

            // In case it is during countdown when finishing
            UI.ActivityCountdown5("Initial");
            sound.Stop(Sound.name.Countdown5);

            ActivityLog.WriteActivityResult(Methods.GetPlayerName(), activityIndex, point + ", " + Methods.TimeFormat(remainingTime, true));
        }
    }

    public void Reset()
    {
        //initialized = false;
        started = false;
        finished = false;
        startTime = 0;
        point = 0;
        minSpeedEnabled = false;
        endCountdown = false;
        UI.huntSpeedLimitTMP.enabled = false;
    }
}
