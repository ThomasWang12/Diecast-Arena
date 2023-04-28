using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public PlayerColor playerColor;
    public int playerColorIndex = -1;

    public void ApplyPlayerColor(int i)
    {
        playerColorIndex = i;
        playerColor.ApplyColor(i);
    }
}
