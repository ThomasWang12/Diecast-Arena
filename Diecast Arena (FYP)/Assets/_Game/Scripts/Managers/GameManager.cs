using RVP;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    [SerializeField] private PlayerController _playerPrefab;
    public GameObject vehicleNormOrient; // #%
    [SerializeField] private GameObject gameMasterPrefab; // #%
    [SerializeField] private GameObject spawnPointsPrefab; // #%

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId) {

        /*var gm = Instantiate(gameMasterPrefab);
        gm.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);

        var sp = Instantiate(spawnPointsPrefab);
        sp.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);*/

        //SpawnPointsServerRpc();

        GameObject spawnPoint = GameObject.Find("Spawn Points").transform.Find("Player " + playerId).gameObject; // #%
        var spawn = Instantiate(_playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation); // #%
        spawn.NetworkObject.SpawnWithOwnership(playerId);

        Debug.Log("Spawned player through SpawnPlayerServerRpc", this);

        //SpawnNormOrientServerRpc();

        // #% Vehicle Norm Orient
        /*var spawnNormOrient = Instantiate(vehicleNormOrient);
        spawnNormOrient.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);
        spawn.GetComponent<VehicleParent>().GetNormOrient(spawnNormOrient);*/
    }

    [ServerRpc]
    void SpawnPointsServerRpc()
    {
        Transform spawnedPointsTransform = Instantiate(spawnPointsPrefab).transform;
        spawnedPointsTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    void SpawnNormOrientServerRpc()
    {
        Transform spawnedNormOrientTransform = Instantiate(vehicleNormOrient).transform;
        spawnedNormOrientTransform.GetComponent<NetworkObject>().Spawn(true);
        Methods.FindOwnedPlayerVehicle().GetComponent<VehicleParent>().norm = spawnedNormOrientTransform;
    }

    public override async void OnDestroy() {
        base.OnDestroy();
        await MatchmakingService.LeaveLobby();
        if(NetworkManager.Singleton != null )NetworkManager.Singleton.Shutdown();
    }
}