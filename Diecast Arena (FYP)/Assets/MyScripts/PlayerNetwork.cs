using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    GameMaster master;
    VehicleManager vehicle;
    UIManager UI;
    ActivityOption activityOption;

    [SerializeField] TMP_InputField playerNameInput;

    [Tooltip("Toggle spawning player vehicle locally or over the network.")]
    public bool localPlay = false;

    [Space(10)]

    public int ownerPlayerId;
    public int activeActivityIndex = -1;
    NetworkList<FixedString64Bytes> playerName;
    [HideInInspector] public NetworkVariable<int> playerColorIndex = new NetworkVariable<int>(-1);

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        vehicle = master.vehicle;
        UI = master.UI;
        activityOption = GameObject.Find("[Activity Triggers]").GetComponent<ActivityOption>();

        playerName = new NetworkList<FixedString64Bytes>();
    }

    public override void OnNetworkSpawn()
    {
        localPlay = false;

        for (int i = 0; i < Constants.MaxPlayers; i++)
            playerName.Add("Player " + i);
    }

    public void Initialize()
    {
        if (localPlay)
        {
            if (playerColorIndex.Value == -1)
                playerColorIndex.Value = 0;
        }
        else
        {
            ownerPlayerId = (int)Methods.FindOwnedPlayer().GetComponent<NetworkObject>().OwnerClientId;
        }
    }

    public void ChangePlayerName()
    {
        if (Methods.IsEmptyOrWhiteSpace(playerNameInput.text)) return;
        ChangePlayerNameServerRpc(ownerPlayerId, playerNameInput.text);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerNameServerRpc(int id, string customName)
    {
        playerName[id] = customName;
        UpdatePlayerNameClientRpc(id, customName);
    }

    [ClientRpc]
    void UpdatePlayerNameClientRpc(int id, string customName)
    {
        playerName[id] = customName;
        Methods.FindPlayerById(id).transform.Find("network").GetComponent<PlayerNameDisplay>().UpdateName(customName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnterActivityServerRpc(int activityIndex, int launcherOption)
    {
        activeActivityIndex = activityIndex;
        EnterActivityClientRpc(activityIndex, launcherOption);
    }

    [ClientRpc]
    void EnterActivityClientRpc(int activityIndex, int launcherOption)
    {
        activityOption.ApplyOptions(activityIndex, launcherOption);
        master.EnterActivity(activityIndex);
    }

    public void ChangePlayerColor(int i)
    {
        if (localPlay)
        {
            UI.SelectColorOption(playerColorIndex.Value, i);
            playerColorIndex.Value = i;
            if (master.ready) vehicle.ApplyPlayerColor(playerColorIndex.Value);
        }
        else ChangePlayerColorServerRpc(i);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerColorServerRpc(int i) // Dev
    {
        UI.SelectColorOption(playerColorIndex.Value, i);
        playerColorIndex.Value = i;
    }
}
