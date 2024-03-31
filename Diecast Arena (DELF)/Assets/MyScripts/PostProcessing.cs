using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessing : MonoBehaviour
{
    GameMaster master;
    [SerializeField] VolumeProfile profile;
    DepthOfField depthOfField;
    ColorAdjustments colorAdjustments;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
    }

    public void ToggleDOV(bool state)
    {
        if (profile.TryGet(out depthOfField))
        {
            depthOfField.active = state;
        }
    }

    public void ToggleSaturation(bool state)
    {
        if (profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.saturation.overrideState = state;
        }
    }
}
