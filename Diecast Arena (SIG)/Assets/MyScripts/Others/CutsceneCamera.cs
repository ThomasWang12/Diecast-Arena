using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    GameMaster master;
    UIManager UI;
    Camera cam;
    Animator animator;

    [SerializeField] Transform staticCarPos;
    [SerializeField] GameObject cinematic6Props;
    [SerializeField] GameObject cinematic7Props;


    [SerializeField] bool isPlaying = false;
    [SerializeField] bool staticView = false;
    bool isTeleported = false;

    [SerializeField] AnimationClip[] cinematics;
    [SerializeField] AnimationClip[] cinematicsStatic;
    float cinematicStopTime;
    float initialFOV;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        UI = master.UI;
        cam = GetComponent<Camera>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.Play("Cinematic Initial", 0, 0.0f);
        animator.enabled = false;
        cam.enabled = false;
        initialFOV = cam.fieldOfView;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
            staticView = !staticView;

        if (master.ready)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1)) PlayCinematic(1);
            if (Input.GetKeyDown(KeyCode.Keypad2)) PlayCinematic(2);
            if (Input.GetKeyDown(KeyCode.Keypad3)) PlayCinematic(3);
            if (Input.GetKeyDown(KeyCode.Keypad4)) PlayCinematic(4);
            if (Input.GetKeyDown(KeyCode.Keypad5)) PlayCinematic(5);
            if (Input.GetKeyDown(KeyCode.Keypad6)) PlayCinematic(6);
            if (Input.GetKeyDown(KeyCode.Keypad7)) PlayCinematic(7);
            if (Input.GetKeyDown(KeyCode.Keypad8)) PlayCinematic(8);
            if (Input.GetKeyDown(KeyCode.Keypad9)) PlayCinematic(9);
        }

        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) // Time.time >= cinematicStopTime
                StopCinematic();
        }
    }

    void PlayCinematic(int index)
    {
        UI.ToggleUI(false);
        cam.enabled = true;
        master.cam = cam;
        animator.enabled = true;

        // Hardcode trigger teleport vehicles
        if (!isTeleported)
        {
            switch (index)
            {
                case 1:
                    Vector3 pos1 = staticCarPos.Find("Static 1 Car Pos").position;
                    Quaternion rot1 = staticCarPos.Find("Static 1 Car Pos").rotation;
                    master.TeleportPlayer(pos1, rot1);
                    break;
                case 2:
                    Vector3 pos2 = staticCarPos.Find("Static 2 Car Pos").position;
                    Quaternion rot2 = staticCarPos.Find("Static 2 Car Pos").rotation;
                    master.TeleportPlayer(pos2, rot2);
                    break;
                case 3:
                    cam.fieldOfView = 55;
                    Vector3 pos3 = staticCarPos.Find("Static 3 Car Pos").position;
                    Quaternion rot3 = staticCarPos.Find("Static 3 Car Pos").rotation;
                    master.TeleportPlayer(pos3, rot3);
                    break;
                case 6:
                    cinematic6Props.SetActive(true);
                    master.ToggleActivity(2, false);
                    break;
                case 7:
                    cinematic7Props.SetActive(true);
                    Vector3 pos7 = staticCarPos.Find("Static 7 Car Pos").position;
                    Quaternion rot7 = staticCarPos.Find("Static 7 Car Pos").rotation;
                    master.TeleportPlayer(pos7, rot7);
                    master.ToggleActivity(2, false);
                    break;
                case 8:
                    master.ToggleAllActivities(false);
                    break;
                case 9:
                    master.ToggleAllActivities(false);
                    break;
            }
            isTeleported = true;
        }

        // Play the animation clip
        string clipName;
        if (staticView)
            clipName = (cinematicsStatic[index] != null) ? cinematicsStatic[index].name : cinematics[index].name;
        else clipName = cinematics[index].name;
        animator.Play(clipName, 0, 0.0f);

        isPlaying = true;
        cinematicStopTime = Time.time + cinematics[index].length;
    }

    void StopCinematic()
    {
        animator.enabled = false;
        master.cam = Camera.main;
        cam.fieldOfView = initialFOV;
        cam.enabled = false;
        UI.ToggleUI(true);
        isPlaying = false;
        isTeleported = false;
        cinematic6Props.SetActive(false);
        cinematic7Props.SetActive(false);
        master.ToggleAllActivities(true);
    }
}
