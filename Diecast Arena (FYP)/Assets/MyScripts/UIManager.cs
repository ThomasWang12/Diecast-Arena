using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameMaster;

public class UIManager : MonoBehaviour
{
    GameMaster master;
    InputManager input;
    Animator screenAnimator;
    Animator uiAnimator;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject screen;
    [SerializeField] GameObject gameUI;

    [Header("Screen")]
    public bool toogleHUD = true;
    [SerializeField] RawImage blackScreen;

    [Header("Game")]
    [SerializeField] TMP_Text gameSate;

    [Header("Activity")]
    [SerializeField] Animator activityTypeIcon;
    [SerializeField] Animator activityTypeText;
    [SerializeField] Animator activityName;
    [SerializeField] Animator activityPressStart;
    [SerializeField] Animator blackFadeOverlay;

    [SerializeField] List<Animator> activityTriggerCanvas;

    [SerializeField] Animator activityCountdown321;
    [SerializeField] Animator activityCountdownStart;
    [SerializeField] Animator activityRace;
    [SerializeField] Animator activityResultRace;

    [Header("Destination Race / Circuit Race")]
    [SerializeField] TMP_Text tmpActivityName;
    [SerializeField] TMP_Text tmpLap;
    [SerializeField] TMP_Text tmpCheckpoint;
    [SerializeField] TMP_Text tmpTime;
    [SerializeField] TMP_Text tmpFinishRank;
    [SerializeField] TMP_Text tmpPlayerResult;
    [SerializeField] public TMP_Text tmpReturnSession;

    [Header("Animations")]
    public AnimationClip[] animationClips;

    /* Circuit Race UI */
    int race_totalLap;
    int race_totalCheckpoint;

    float returnSessionTime;

    void Awake()
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
        input = master.ManagerObject(Manager.type.input).GetComponent<InputManager>();
        screenAnimator = screen.GetComponent<Animator>();
        uiAnimator = gameUI.GetComponent<Animator>();
    }

    void Start()
    {
        AnimationsInitial();
    }

    void AnimationsInitial()
    {
        // Screen
        screenAnimator.Play("Fade Black Initial", 0, 1.0f);
        // Game UI
        HideActivityInfo();
        foreach (var canvas in activityTriggerCanvas)
            if (canvas.transform.parent.gameObject.activeSelf) canvas.Play("Activity Trigger Canvas Initial", 0, 1.0f);
        activityCountdown321.Play("Activity Countdown 321 Initial", 0, 0.0f);
        activityCountdownStart.Play("Activity Countdown Start Initial", 0, 1.0f);
        activityRace.Play("Activity UI Initial", 0, 0.0f);
        activityResultRace.Play("Activity UI Initial", 0, 0.0f);
    }

    void Update()
    {
        if (input.ToggleHUD()) ToggleHUD();

        if (tmpReturnSession.isActiveAndEnabled)
        {
            int remaining = Mathf.FloorToInt(returnSessionTime + master.activityFinishWaitDuration - Time.time);
            tmpReturnSession.text = "Return to session (0:0" + remaining + ")";
        }
    }

    void ToggleHUD()
    {
        toogleHUD = !toogleHUD;
        int alpha = toogleHUD ? 1 : 0;
        canvas.GetComponent<CanvasGroup>().alpha = alpha;
    }

    public void DisplayGameSate(string state)
    {
        gameSate.text = state;
    }

    public void ShowActivityInfoAnim(int index)
    {
        activityType type = master.activityList[index].type;
        string name = master.activityList[index].name;

        activityTypeIcon.Play("Activity Type Icon In", 0, 0.0f);
        activityTypeText.gameObject.GetComponent<TextMeshProUGUI>().text = ActivityTypeName(type);
        activityTypeText.Play("Activity Type Text In", 0, 0.0f);
        activityName.gameObject.GetComponent<TextMeshProUGUI>().text = name;
        activityName.Play("Activity Name In", 0, 0.0f);

        activityPressStart.Play("Activity Press Start In", 0, 0.0f);
        blackFadeOverlay.Play("Black Fade Left Overlay In", 0, 0.0f);
    }

    public void HideActivityInfoAnim(int index)
    {
        activityTypeIcon.Play("Activity Type Icon Out", 0, 0.0f);
        activityTypeText.Play("Activity Type Text Out", 0, 0.0f);
        activityName.Play("Activity Name Out", 0, 0.0f);
        activityPressStart.Play("Activity Press Start Out", 0, 0.0f);
        blackFadeOverlay.Play("Black Fade Left Overlay Out", 0, 0.0f);
    }

    public void HideActivityInfo()
    {
        activityTypeIcon.Play("Activity Type Icon Initial", 0, 0.0f);
        activityTypeText.Play("Activity Type Text Initial", 0, 0.0f);
        activityName.Play("Activity Name Initial", 0, 0.0f);
        activityPressStart.Play("Activity Press Start Initial", 0, 0.0f);
        blackFadeOverlay.Play("Black Fade Left Overlay Initial", 0, 0.0f);
    }

    public void ActivityTriggerCanvas(int index, string type)
    {
        if (type == "Show") activityTriggerCanvas[index].Play("Activity Trigger Canvas In", 0, 0.0f);
        if (type == "Hide") activityTriggerCanvas[index].Play("Activity Trigger Canvas Out", 0, 0.0f);
    }

    public void ActivityCountdown321(string type)
    {
        if (type == "Play") activityCountdown321.Play("Activity Countdown 321", 0, 0.0f);
        if (type == "Initial") activityCountdown321.Play("Activity Countdown 321 Initial", 0, 0.0f);
    }

    public void ActivityCountdownStart()
    {
        activityCountdownStart.Play("Activity Countdown Start", 0, 0.0f);
    }

    public void ActivityUI(activityType activityType, string type)
    {
        if (activityType == activityType.RaceDestination || activityType == activityType.RaceCircuit)
        {
            if (type == "Show") activityRace.Play("Activity UI Show", 0, 0.0f);
            if (type == "Hide") activityRace.Play("Activity UI Hide", 0, 0.0f);
            if (type == "Initial") activityRace.Play("Activity UI Initial", 0, 0.0f);
        }
    }

    // Info (Activity) UI:
    // Info of this activity that do not need to be continuously updated during the activity.
    public void InfoRaceUI(int totalLap, int totalCheckpoint)
    {
        race_totalLap = totalLap;
        race_totalCheckpoint = totalCheckpoint;
    }

    // Update (Activity) UI:
    // Variables that are continuously updated during the activity.

    public void UpdateRaceDestinationUI(int currentCheckpoint, float raceTime)
    {
        tmpLap.text = currentCheckpoint + " / " + race_totalCheckpoint;
        tmpCheckpoint.text = "Checkpoint";
        tmpTime.text = Methods.TimeFormat(raceTime);
    }

    public void UpdateRaceCircuitUI(int currentLap, int currentCheckpoint, float raceTime)
    {
        tmpLap.text = currentLap + " / " + race_totalLap + " <size=\"40\">Lap</size>";
        tmpCheckpoint.text = "Checkpoint " + currentCheckpoint + " / " + race_totalCheckpoint;
        tmpTime.text = Methods.TimeFormat(raceTime);
    }

    // Result (Activity) UI:
    // Variables that are recorded upon completion of the activity.

    public void ResultRaceUI(int index, float raceTime, float currentTime)
    {
        tmpActivityName.text = master.activityList[index].name;
        tmpPlayerResult.text = Methods.TimeFormat(raceTime) + " | Player";
        returnSessionTime = currentTime;
        tmpReturnSession.enabled = true;
    }

    public void ActivityResultUI(activityType activityType, string type)
    {
        if (activityType == activityType.RaceDestination || activityType == activityType.RaceCircuit)
        {
            if (type == "Show")
            {
                activityResultRace.Play("Activity UI Show", 0, 0.0f);
                blackFadeOverlay.Play("Black Fade Left Overlay In", 0, 0.0f);
            }
            if (type == "Hide")
            {
                activityResultRace.Play("Activity UI Hide", 0, 0.0f);
                blackFadeOverlay.Play("Black Fade Left Overlay Out", 0, 0.0f);
            }
            if (type == "Initial")
            {
                activityResultRace.Play("Activity UI Initial", 0, 0.0f);
                blackFadeOverlay.Play("Black Fade Left Overlay Initial", 0, 0.0f);
            }
        }
    }

    public void FadeBlack(string type)
    {
        if (type == "In") screenAnimator.Play("Fade In Black", 0, 0.0f);
        if (type == "Out") screenAnimator.Play("Fade Out Black", 0, 0.0f);
    }

    string ActivityTypeName(activityType type)
    {
        // To convert activity type to its display name
        return type switch
        {
            activityType.RaceDestination => "Destination Race",
            activityType.RaceCircuit => "Circuit Race",
            activityType.CheckpointRush => "Checkpoint Rush",
            activityType.CarHunt => "Car Hunt",
            _ => ""
        };
    }

    public int AnimNameToIndex(string name)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == name)
                return i; // The index
        }
        return -1; // It is not in the list
    }
}
