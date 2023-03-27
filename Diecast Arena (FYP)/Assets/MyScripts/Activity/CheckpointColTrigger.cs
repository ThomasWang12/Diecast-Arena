using System.Collections;
using System.Drawing;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class CheckpointColTrigger : MonoBehaviour
{
    GameMaster master;
    CheckpointCircuit race;

    GameObject checkpoint;
    Material checkpointVisualMat;
    GameObject arrow;
    Material arrowMat;
    Material arrowMatInitial;
    GameObject flag;
    Material flagMat;
    Material flagMatInitial;

    int index;
    float colliderRadius;
    [Tooltip("Manually adjust the spawn height of arrow/flag for this checkpoint.")]
    [Range(-10f, 10f)] [SerializeField] float heightAdjust = 0;
    [SerializeField] bool isCollided = false;
    float collideTime = 0;

    /* Tunables */
    float fadeDist = 30f;
    float alphaMax = 0.8f;
    float alphaMin = 0.25f;
    float fadeStartAlpha = 0.5f;
    float fadeDuration = 0.3f;

    void Awake()
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
        race = transform.parent.parent.parent.GetComponent<CheckpointCircuit>();
        checkpoint = transform.parent.gameObject;
        checkpointVisualMat = Methods.GetChildContainsName(checkpoint, "[Visual]").GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        colliderRadius = transform.parent.localScale.x / 2;
    }

    void Update()
    {
        if (race.started)
        {
            if (isCollided == false)
            {
                // When player is getting closer to the checkpoint, alpha fades to less (for visibility)
                float dist = Vector3.Distance(master.playerPos, transform.position) - colliderRadius;
                float distClamped = Mathf.Clamp(dist, 0, fadeDist);
                float alpha = Methods.Map(distClamped, 0, fadeDist, alphaMin, alphaMax);
                checkpointVisualMat.SetFloat("_AlphaA", alpha);
            }
            else
            {
                // Checkpoint reached, mesh turns white and fades out
                checkpointVisualMat.SetColor("_Color", Color.white);
                float k = Mathf.Clamp((collideTime + fadeDuration) - Time.time, 0, fadeDuration);
                float fade = Easing.Type.SineEaseOut(k, 0, fadeStartAlpha, fadeDuration);
                checkpointVisualMat.SetFloat("_AlphaA", fade);

                FadeCollided(arrow, arrowMat, fade);
                FadeCollided(flag, flagMat, fade);
            }
        }
    }

    void FadeCollided(GameObject user, Material userMat, float fade)
    {
        if (user == null) return;
        userMat.SetColor("_EmissionColor", Color.white);
        Color color = new Color(Color.white.r, Color.white.g, Color.white.b, fade);
        userMat.SetColor("_BaseColor", color);
    }

    public void GetCheckpointIndex()
    {
        index = race.checkpoints.IndexOf(checkpoint);
    }

    public void InitializeCheckpoint()
    {
        checkpointVisualMat.SetColor("_Color", checkpointVisualMat.GetColor("_Color_Checkpoint"));
    }

    public void InitializeFinalCheckpoint()
    {
        checkpointVisualMat.SetColor("_Color", checkpointVisualMat.GetColor("_Color_Finish"));
        race.flagScript.UpdateActive();
    }

    public void InitializeArrow()
    {
        arrow = race.arrowsScript.SpawnArrow(index, heightAdjust);
        MeshRenderer arrowMeshRenderer = arrow.GetComponent<MeshRenderer>();
        arrowMatInitial = arrowMeshRenderer.material;
        arrowMeshRenderer.material = Instantiate(arrowMatInitial);
        arrowMat = arrowMeshRenderer.material;
    }

    public void InitializeFlag()
    {
        flag = race.flagScript.SpawnFlag(index, heightAdjust);
        MeshRenderer flagMeshRenderer = flag.GetComponent<MeshRenderer>();
        flagMatInitial = flagMeshRenderer.material;
        flagMeshRenderer.material = Instantiate(flagMatInitial);
        flagMat = flagMeshRenderer.material;
    }

    void OnTriggerEnter(Collider other)
    {
        // Avoid repeated detection
        if (isCollided) return;
        isCollided = true;
        collideTime = Time.time;
        StartCoroutine("WaitForInactive");

        if (race.currentCheckpoint < race.checkpoints.Count - 1)
            race.arrowsScript.StartCoroutine("WaitForInactive", index);
        else
        {
            if (race.activityType == GameMaster.activityType.RaceDestination)
            {
                if (race.currentCheckpoint == race.checkpoints.Count - 1)
                    race.flagScript.StartCoroutine("WaitForInactive");
            }
            if (race.activityType == GameMaster.activityType.RaceCircuit)
            {
                if (race.currentLap < race.totalLap)
                    race.arrowsScript.StartCoroutine("WaitForInactive", index);
                else race.flagScript.StartCoroutine("WaitForInactive");
            }
        }

        // Send this checkpoint index to the race script
        race.CheckpointReached(index);
    }

    IEnumerator WaitForInactive()
    {
        yield return new WaitForSeconds(race.inactiveAfter);
        Reset();
        checkpoint.SetActive(false);
    }

    public void Reset()
    {
        isCollided = false;
        collideTime = 0;

        checkpointVisualMat.SetColor("_Color", checkpointVisualMat.GetColor("_Color_Checkpoint"));

        if (arrow != null)
        {
            arrow.GetComponent<MeshRenderer>().material = arrowMatInitial;
            arrowMat = arrow.GetComponent<MeshRenderer>().material;
        }
        if (flag != null)
        {
            flag.GetComponent<MeshRenderer>().material = flagMatInitial;
            flagMat = flag.GetComponent<MeshRenderer>().material;
        }
    }
}
