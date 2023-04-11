using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkList<FixedString64Bytes> playerName;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        playerName = new NetworkList<FixedString64Bytes>();
        for (int i = 0; i < Constants.MaxPlayers; i++) playerName.Add("Player " + i);
    }

    public void ChangePlayerName(int index, string input)
    {
        playerName[index] = (Methods.IsEmptyOrWhiteSpace(input)) ? playerName[index] : input;
        Debug.Log("Player " + index + " Name: " + playerName[index]);
    }
}
