using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadHint : MonoBehaviour
{
    GameMaster master;
    InputManager input;

    [SerializeField] GameObject target;
    [SerializeField] GameObject ok;

    void Awake()
    {
        master = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        input = master.input;
    }

    void Update()
    {
        if (master.ready)
        {
            target.SetActive(input.showGamepadHint);
            ok.SetActive(input.showGamepadHint);
        }
        else
        {
            target.SetActive(false);
            ok.SetActive(false);
        }
    }

    public void Ok()
    {
        input.showGamepadHint = false;
        target.SetActive(false);
        ok.SetActive(false);
    }
}
