using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerConnector : MonoBehaviour
{
    PlayerInputManager im;


    private void Start()
    {
        im = PlayerInputManager.instance;
        im.onPlayerJoined += OnPlayerJoined;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        //Debug.Log(input.ToString());
        //if (input.devices[0] == Keyboard.current || input.devices[0] == Mouse.current)
        //{
        //    Debug.Log("Setting m&k controls");
        //    Debug.Log(input.SwitchCurrentControlScheme(Keyboard.current, Mouse.current));
        //}
    }
}
