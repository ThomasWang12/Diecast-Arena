using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    GameMaster master;
    UIManager UI;
    Camera cam;
    Animator animator;

    [SerializeField] bool isPlaying = false;
    [SerializeField] AnimationClip[] cinematics;
    float cinematicStopTime;

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
    }

    void Update()
    {
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
            if (Time.time >= cinematicStopTime || Input.GetKeyDown(KeyCode.KeypadEnter))
                StopCinematic();
        }
    }

    void PlayCinematic(int index)
    {
        UI.ToggleUI(false);
        cam.enabled = true;
        animator.enabled = true;
        animator.Play(cinematics[index].name, 0, 0.0f);
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
