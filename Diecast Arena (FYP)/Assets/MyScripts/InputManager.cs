using RVP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    GameMaster master;

    public bool allowInput = true;
    public bool allowDrive = true;
    public bool forceBrake = false;

    bool padAxis6Pressed = false; // Gamepad Left/Right Buttons (left = -1, right = 1)
    bool padAxis7Pressed = false; // Gamepad Up/Down Buttons (up = 1, down = -1)

    /* Tunables */
    //float deadzone = 0.19f;
    float deadzoneButton = 0.001f;

    void Awake()
    {
        master = GameObject.FindWithTag("GameManager").GetComponent<GameMaster>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void LateUpdate()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Gamepad Left/Right Buttons")) < deadzoneButton)
            padAxis6Pressed = false;

        if (Mathf.Abs(Input.GetAxisRaw("Gamepad Up/Down Buttons")) < deadzoneButton)
            padAxis7Pressed = false;
    }

    public void ForceBrake()
    {
        allowDrive = false;
        forceBrake = true;
    }

    public void EnableDrive()
    {
        allowDrive = true;
        forceBrake = false;
    }

    public bool ToggleHUD()
    {
        if (!allowInput) return false;

        if (Input.GetKeyDown(KeyCode.U)) return true;
        return false;
    }

    public bool StartActivity()
    {
        if (!allowInput) return false;

        if (Input.GetKeyDown(KeyCode.E) || GamepadRightButton()) return true;
        return false;
    }

    #region Gamepad Buttons

    public bool GamepadLeftButton()
    {
        if (Input.GetAxisRaw("Gamepad Left/Right Buttons") < 0)
        {
            if (!padAxis6Pressed)
            {
                padAxis6Pressed = true;
                return true;
            }
        }
        return false;
    }

    public bool GamepadRightButton()
    {
        if (Input.GetAxisRaw("Gamepad Left/Right Buttons") > 0)
        {
            if (!padAxis6Pressed)
            {
                padAxis6Pressed = true;
                return true;
            }
        }
        return false;
    }

    public bool GamepadUpButton()
    {
        if (Input.GetAxisRaw("Gamepad Up/Down Buttons") > 0)
        {
            if (!padAxis7Pressed)
            {
                padAxis7Pressed = true;
                return true;
            }
        }
        return false;
    }

    public bool GamepadDownButton()
    {
        if (Input.GetAxisRaw("Gamepad Up/Down Buttons") < 0)
        {
            if (!padAxis7Pressed)
            {
                padAxis7Pressed = true;
                return true;
            }
        }
        return false;
    }

    #endregion
}
