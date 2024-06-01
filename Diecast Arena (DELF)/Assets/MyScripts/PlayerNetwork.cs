using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    GameMaster master;
    VehicleManager vehicle;
    SoundManager sound;
    UIManager UI;
    ActivityOption activityOption;

    [SerializeField] TMP_InputField playerNameInput;

    [Tooltip("Toggle spawning player vehicle locally or over the network.")]
    public bool localPlay = false;

    [Space(10)]

    bool initialized = false;
    public int playerCount = 1;
    public int ownerPlayerId = 0;

    public NetworkList<FixedString64Bytes> playerName;
    NetworkList<int> playerColorIndex;
    //NetworkList<int> playerCollectScore;
    NetworkList<bool> playerAutoReset;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        vehicle = master.vehicle;
        sound = master.sound;
        UI = master.UI;
        activityOption = master.activityOption;

        playerName = new NetworkList<FixedString64Bytes>();
        playerColorIndex = new NetworkList<int>();
        //playerCollectScore = new NetworkList<int>();
        playerAutoReset = new NetworkList<bool>();
    }

    void NetworkVarsInit()
    {
        if (IsServer)
        {
            playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        }

        for (int i = 0; i < Constants.MaxPlayers; i++)
        {
            playerName.Add("Player " + i);
            playerColorIndex.Add(-1);
            //playerCollectScore.Add(0);
            playerAutoReset.Add(true);
        }
    }

    public override void OnNetworkSpawn()
    {
        localPlay = false;
        NetworkVarsInit();
    }

    void Start()
    {
        if (localPlay) NetworkVarsInit();
    }

    void Initialize()
    {
        int defaultColorIndex;

        if (localPlay)
        {
            ownerPlayerId = 0;
            defaultColorIndex = 0;
            playerColorIndex[ownerPlayerId] = defaultColorIndex;
            ChangePlayerColorCodes(0, defaultColorIndex);
        }
        else
        {
            ownerPlayerId = (int)Methods.FindOwnedPlayer().GetComponent<NetworkObject>().OwnerClientId;
            defaultColorIndex = ownerPlayerId % vehicle.playerColorTotal;
            UI.SelectColorOption(-1, defaultColorIndex);
            ChangePlayerColorServerRpc(ownerPlayerId, defaultColorIndex);
        }

        initialized = true;
    }

    void Update()
    {
        if (!initialized) Initialize();
    }

    public void EnterSession()
    {
        for (int i = 0; i < Constants.MaxPlayers; i++)
            vehicle.ApplyPlayerColor(i, playerColorIndex[i]);
    }

    #region Player Names

    public void ChangePlayerName()
    {
        if (Methods.IsEmptyOrWhiteSpace(playerNameInput.text)) return;

        if (localPlay)
        {
            if (playerName[0] != playerNameInput.text)
            {
                playerName[0] = playerNameInput.text;
                ActivityLog.WriteChangePlayerName(playerNameInput.text);
            }
        }
        else
        {
            ChangePlayerNameServerRpc(ownerPlayerId, playerNameInput.text);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerNameServerRpc(int id, string customName)
    {
        if (playerName[id] != customName)
        {
            playerName[id] = customName;
            Methods.FindPlayerById(id).transform.Find("network").GetComponent<PlayerNameDisplay>().UpdateName(customName);
            ActivityLog.WriteChangePlayerName(customName);
        }
        UpdatePlayerNameClientRpc(id, customName);
    }

    [ClientRpc]
    void UpdatePlayerNameClientRpc(int id, string customName)
    {
        if (playerName[id] != customName)
        {
            playerName[id] = customName;
            Methods.FindPlayerById(id).transform.Find("network").GetComponent<PlayerNameDisplay>().UpdateName(customName);
            ActivityLog.WriteChangePlayerName(customName);
        }
    }

    #endregion

    #region Player Colors

    public void ChangePlayerColor(int colorIndex)
    {
        if (localPlay)
        {
            playerColorIndex[0] = colorIndex;
            ChangePlayerColorCodes(0, colorIndex);
        }
        else ChangePlayerColorServerRpc(ownerPlayerId, colorIndex);

        sound.Play(Sound.name.PaintSpray);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerColorServerRpc(int id, int colorIndex)
    {
        playerColorIndex[id] = colorIndex;
        ChangePlayerColorClientRpc(id, colorIndex);
    }

    [ClientRpc]
    void ChangePlayerColorClientRpc(int id, int colorIndex)
    {
        ChangePlayerColorCodes(id, colorIndex);
    }

    void ChangePlayerColorCodes(int id, int colorIndex)
    {
        if (ownerPlayerId == id)
        {
            UI.SelectColorOption(vehicle.playerColorIndex, colorIndex);
            vehicle.playerColorIndex = colorIndex;
        }
        if (master.ready) vehicle.ApplyPlayerColor(id, colorIndex);
    }

    #endregion

    #region Enter Activity

    [ServerRpc(RequireOwnership = false)]
    public void EnterActivityServerRpc(int activityIndex, int launcherOption)
    {
        EnterActivityClientRpc(activityIndex, launcherOption);
    }

    [ClientRpc]
    void EnterActivityClientRpc(int activityIndex, int launcherOption)
    {
        activityOption.ApplyOptions(activityIndex, launcherOption);
        master.EnterActivity(activityIndex);
    }

    #endregion

    #region Exit Activity

    [ServerRpc(RequireOwnership = false)]
    public void ExitActivityServerRpc(int activityIndex)
    {
        ExitActivityClientRpc(activityIndex);
    }

    [ClientRpc]
    void ExitActivityClientRpc(int activityIndex)
    {
        master.ExitActivity(activityIndex);
    }

    #endregion

    #region Collect Checkpoint
    /*
    [ServerRpc(RequireOwnership = false)]
    public void CollectCheckpointServerRpc(int id, int score)
    {
        playerCollectScore[id] = score;
        CollectCheckpointClientRpc(playerCollectScore[id], score);
    }

    [ClientRpc]
    void CollectCheckpointClientRpc(int id, int score)
    {
        // ...
    }
    */
    #endregion

    #region Relaunch

    [ServerRpc(RequireOwnership = false)]
    public void AutoResetStateServerRpc(int id, bool state)
    {
        playerAutoReset[id] = state;

        // If all players are currently still connected
        int currentlyConnected = 0;
        if (IsServer) currentlyConnected = NetworkManager.Singleton.ConnectedClients.Count;
        if (currentlyConnected == playerCount)
        {
            // If all players are available for relaunch
            if (!playerAutoReset.Contains(false)) // i.e. all true
                RelaunchServerRpc();
        }
        else RelaunchServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RelaunchServerRpc()
    {
        // Relaunch the game for all players
        RelaunchClientRpc();
    }

    [ClientRpc]
    void RelaunchClientRpc()
    {
        AutoReset.Relaunch(Dev.logType.PassiveRelaunch);
    }

    #endregion
}
