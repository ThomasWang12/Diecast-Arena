using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sound
{
    public enum name
    {
<<<<<<< HEAD:Diecast Arena (SIG)/Assets/MyScripts/SoundManager.cs
        Select, PaintSpray, ButtonUp, ButtonDown,
        Countdown321, Countdown5,
        Checkpoint, CheckpointBold, CameraShutter,
        GameLose
=======
        Countdown321, Countdown5, Checkpoint, CheckpointBold, CameraShutter, GameLose
>>>>>>> parent of 6c7c732 (Dev):Diecast Arena (FYP)/Assets/MyScripts/SoundManager.cs
    }

    public static string AudioEnumToName(name name)
    {
        return name switch
        {
<<<<<<< HEAD:Diecast Arena (SIG)/Assets/MyScripts/SoundManager.cs
            name.Select => "Select",
            name.PaintSpray => "Paint Spray",
            name.ButtonUp => "Button Up",
            name.ButtonDown => "Button Down",
=======
>>>>>>> parent of 6c7c732 (Dev):Diecast Arena (FYP)/Assets/MyScripts/SoundManager.cs
            name.Countdown321 => "Countdown 321",
            name.Countdown5 => "Countdown 5",
            name.Checkpoint => "Checkpoint",
            name.CheckpointBold => "Checkpoint Bold",
            name.CameraShutter => "Camera Shutter",
            name.GameLose => "Game Lose",
            _ => ""
        };
    }
}

public class SoundManager : MonoBehaviour
{
    [Tooltip("Find the AudioSource component in its children and store them in this list.")]
    public List<AudioSource> audioList;

    void Start()
    {
        List<GameObject> audioObjects = new List<GameObject>();
        Methods.GetChildRecursive(gameObject, audioObjects, "");

        for (int i = 0; i < audioObjects.Count; i++)
        {
            AudioSource audioSource = audioObjects[i].GetComponent<AudioSource>();
            if (audioSource != null) audioList.Add(audioSource);
        }
    }

    public void Play(Sound.name name)
    {
        string audioName = Sound.AudioEnumToName(name);
        int index = AudioNameToIndex(audioName);
        audioList[index].PlayOneShot(audioList[index].clip);
    }

    public void Stop(Sound.name name)
    {
        string audioName = Sound.AudioEnumToName(name);
        int index = AudioNameToIndex(audioName);
        audioList[index].Stop();
    }

    int AudioNameToIndex(string name)
    {
        for (int i = 0; i < audioList.Count; i++)
        {
            if (audioList[i].gameObject.name == name)
                return i; // The index
        }
        return -1; // It is not in the list
    }
}
