using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerConnector : MonoBehaviour
{
    public static PlayerConnector instance;
    PlayerInputManager im;
    [SerializeField] private List<CharacterUI> characterUIs = new List<CharacterUI>();

    public List<CharacterStatus> players = new();

    private void Start()
    {
        instance = this;
        im = PlayerInputManager.instance;
        im.onPlayerJoined += OnPlayerJoined;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        CharacterStatus cs = input.GetComponent<CharacterStatus>();
        if (cs != null)
        {
            players.Add(cs);
            cs.Setup(characterUIs[players.Count - 1]);
        }
    }
}
