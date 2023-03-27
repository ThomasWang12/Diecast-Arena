using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using static GameMaster;

public class CheckpointCircuit : MonoBehaviour
{
    GameMaster master;
    InputManager input;
    UIManager UI;
    [HideInInspector] public CheckpointArrows arrowsScript;
    [HideInInspector] public CheckpointFlag flagScript;
    [HideInInspector] public List<GameObject> checkpoints;
    CheckpointColTrigger[] checkpointCols;
    Vector3 startPos;
    Quaternion startRot;

    [HideInInspector] public activityType activityType;
    int activityIndex;

    public bool started = false;
    public bool finished = false;
    float startTime = 0;
    public int currentLap = 0;
    [Tooltip("0 implies this is a Destination Race (point-to-point).")]
    public int totalLap = 1;
    public int currentCheckpoint = -1;
    [SerializeField] int totalCheckpoint = 0;

    /* Tunables */
    [HideInInspector] public float inactiveAfter = 2.0f;

    void Awake()
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
        input = master.ManagerObject(Manager.type.input).GetComponent<InputManager>();
        UI = master.ManagerObject(Manager.type.UI).GetComponent<UIManager>();
        arrowsScript = transform.Find("[Arrows]").GetComponent<CheckpointArrows>();
        flagScript = transform.Find("[Finish Flag]").GetComponent<CheckpointFlag>();
    }

    void Start()
    {
        activityIndex = master.ActivityObjectToIndex(gameObject);
        activityType = master.activityList[activityIndex].type;
        if (activityType == activityType.RaceDestination)
        {
            totalLap = 0;
            startPos = transform.Find("[Start Position]").position;
            startRot = transform.Find("[Start Position]").rotation;
        }
        InitializeCheckpoints(activityType);
        totalCheckpoint = checkpoints.Count;
    }

    void Update()
    {
        if (started && !finished)
        {
            if (activityType == activityType.RaceDestination)
                UI.UpdateRaceDestinationUI(currentCheckpoint + 1, Time.time - startTime);

            if (activityType == activityType.RaceCircuit)
                UI.UpdateRaceCircuitUI(currentLap, currentCheckpoint + 1, Time.time - startTime);
        }
    }

    public void InitializeCheckpoints(activityType type)
    {
        // Store the checkpoints inside 'Checkpoints' object into 'checkpoints' list
        checkpoints = Methods.GetChildren(transform.Find("[Checkpoints]").gameObject);
        checkpointCols = new CheckpointColTrigger[checkpoints.Count];

        // For each checkpoint
        for (int i = 0; i < checkpoints.Count; i++)
        {
            GameObject col = Methods.GetChildContainsName(checkpoints[i].gameObject, "[Col]");
            checkpointCols[i] = col.GetComponent<CheckpointColTrigger>();
            checkpointCols[i].GetCheckpointIndex();
            checkpointCols[i].InitializeCheckpoint();

            if (type == activityType.RaceDestination)
            {
                // Spawn arrows at checkpoints (except the final one)
                if (i < checkpoints.Count - 1) checkpointCols[i].InitializeArrow();
                // Spawn flag at the final checkpoint
                else checkpointCols[i].InitializeFlag();
            }

            if (type == activityType.RaceCircuit)
            {
                // Spawn arrows at all checkpoints
                checkpointCols[i].InitializeArrow();
                // Final checkpoint (Start/Finish)
                if (i == checkpoints.Count - 1) {
                    checkpointCols[i].InitializeFlag(); // Spawn flag
                    startPos = checkpoints[i].transform.position; // Get the start position
                    startRot = checkpoints[i].transform.rotation; // Get the start rotation
                }
            }
            checkpoints[i].SetActive(false);
        }
    }

    public void InitializeRace()
    {
        InitializeCheckpoints(activityType);
        master.TeleportPlayer(startPos, startRot);
        UI.InfoRaceUI(totalLap, checkpoints.Count);
    }

    public void StartRace()
    {
        if (started) return;

        started = true;
        startTime = Time.time;
        input.EnableDrive();
        if (activityType == activityType.RaceCircuit) currentLap++;
        currentCheckpoint++;
        UpdateActive(currentCheckpoint);
    }

    void UpdateActive(int index)
    {
        checkpoints[index].SetActive(true);
        // If it is the final checkpoint
        if (index == checkpoints.Count - 1 && currentLap == totalLap)
            checkpointCols[index].InitializeFinalCheckpoint();
        else arrowsScript.UpdateActive(index);
    }

    public void CheckpointReached(int index)
    {
        // If it is not the final checkpoint
        if (index < checkpoints.Count - 1)
        {
            arrowsScript.PlayCheckpointSound();
            int nextCheckpoint = index + 1;
            currentCheckpoint = nextCheckpoint;
            UpdateActive(currentCheckpoint);
        }
        else
        {
            // If it is not the final lap in Circuit Race
            if (currentLap < totalLap)
            {
                currentLap++;
                arrowsScript.PlayCheckpointSound();
                int nextCheckpoint = 0;
                currentCheckpoint = nextCheckpoint;
                UpdateActive(currentCheckpoint);
            }
            else
            {
                // Race finished
                finished = true;
                flagScript.PlayFinishSound();
                master.FinishActivity(activityIndex);
                UI.ResultRaceUI(activityIndex, Time.time - startTime, Time.time);
            }
        }
    }

    public void Reset()
    {
        started = false;
        finished = false;
        startTime = 0;
        currentLap = 0;
        currentCheckpoint = -1;
        totalCheckpoint = 0;

        checkpoints.Clear();
        foreach (var col in checkpointCols) col.Reset();
        checkpointCols = null;
        arrowsScript.arrows.Clear();
        foreach (Transform child in arrowsScript.transform) Destroy(child.gameObject);
        flagScript.flag = null;
        foreach (Transform child in flagScript.transform) Destroy(child.gameObject);
    }
}
