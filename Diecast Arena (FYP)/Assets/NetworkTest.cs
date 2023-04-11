using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class NetworkTest : NetworkBehaviour
{
    GameMaster master;
    InputManager input;

    [SerializeField] private int id = -1;

    void Awake()
    {
        Debug.Log("In " + SceneManager.GetActiveScene().name + ", OwnerClientId = " + OwnerClientId + ", " + "void Awake()");

        GetGameMaster("Awake()");
    }

    public override void OnNetworkSpawn()
    {
        id = (int) OwnerClientId;

        Debug.Log("In " + SceneManager.GetActiveScene().name + ", OwnerClientId = " + OwnerClientId + ", " + "void OnNetworkSpawn()");

        GetGameMaster("OnNetworkSpawn()");

        transform.position = GameObject.Find("Spawn Points").transform.Find("Player " + OwnerClientId).position;
    }

    void Start()
    {
        Debug.Log("In " + SceneManager.GetActiveScene().name + ", OwnerClientId = " + OwnerClientId + ", " + "void Start()");

        GetGameMaster("Start()");

        enabled = true;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.A)) transform.Translate(0, 0, 0.2f);
        if (Input.GetKeyDown(KeyCode.D)) transform.Translate(0, 0, -0.2f);
    }

    void GetGameMaster(string function)
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
        input = master.ManagerObject(Manager.type.input).GetComponent<InputManager>();

        if (master != null) Debug.Log("[OwnerClientId: " + OwnerClientId + "] " + master.gameObject.name + " in " + function);
        else Debug.Log("[OwnerClientId: " + OwnerClientId + "] " + "No Master!! in " + function);

        if (input != null) Debug.Log("[OwnerClientId: " + OwnerClientId + "] " + input.gameObject.name + " in " + function);
        else Debug.Log("[OwnerClientId: " + OwnerClientId + "] " + "No Input!! in " + function);
    }
}
