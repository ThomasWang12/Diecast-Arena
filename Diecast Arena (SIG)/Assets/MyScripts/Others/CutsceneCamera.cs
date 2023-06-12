using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    GameMaster master;
    VehicleManager vehicle;
    UIManager UI;
    Camera cam;
    Animator animator;

    [SerializeField] bool isPlaying = false;
    [SerializeField] bool staticView = false;

    [SerializeField] AnimationClip[] cinematics;
    [SerializeField] AnimationClip[] cinematicsStatic;
    float cinematicStopTime;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        vehicle = master.vehicle;
        UI = master.UI;
        cam = GetComponent<Camera>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.Play("Cinematic Initial", 0, 0.0f);
        animator.enabled = false;
        cam.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            staticView = !staticView;
            if (staticView) Debug.Log("Cinematic: Motion");
            else Debug.Log("Cinematic: Static");
        }

        if (master.ready)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1)) PlayCinematic(1);
            if (Input.GetKeyDown(KeyCode.Keypad2)) PlayCinematic(2);
            if (Input.GetKeyDown(KeyCode.Keypad3)) PlayCinematic(3);
            if (Input.GetKeyDown(KeyCode.Keypad4)) PlayCinematic(4);
            if (Input.GetKeyDown(KeyCode.Keypad5)) PlayCinematic(5);
            if (Input.GetKeyDown(KeyCode.Keypad6)) PlayCinematic(6);
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
        animator.enabled = true;

        string clipName;
        if (staticView)
        {
            clipName = (cinematicsStatic[index] != null) ? cinematicsStatic[index].name : cinematics[index].name;

            // Hardcode trigger teleport vehicles
            switch (index)
            {
                case 1:
                    Vector3 pos1 = transform.Find("Static 1 Car Pos").localPosition;
                    Quaternion rot1 = transform.Find("Static 1 Car Pos").localRotation;
                    master.TeleportPlayer(pos1, rot1);
                    break;
                case 2:
                    Vector3 pos2 = transform.Find("Static 2 Car Pos").localPosition;
                    Quaternion rot2 = transform.Find("Static 2 Car Pos").localRotation;
                    master.TeleportPlayer(pos2, rot2);
                    break;
            }
        }
        else clipName = cinematics[index].name;

        animator.Play(clipName, 0, 0.0f);
        isPlaying = true;
        cinematicStopTime = Time.time + cinematics[index].length;
    }

    void StopCinematic()
    {
        animator.enabled = false;
        cam.enabled = false;
        UI.ToggleUI(true);
        isPlaying = false;
    }
}
