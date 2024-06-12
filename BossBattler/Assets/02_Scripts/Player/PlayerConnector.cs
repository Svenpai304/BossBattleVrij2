using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerConnector : MonoBehaviour
{
    public static PlayerConnector instance;
    [SerializeField] private List<Color> characterColors = new();
    public List<CharacterStatus> players = new();
    private PlayerJoinedUI joinUI;
    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            foreach(CharacterStatus status in players)
            {
                Destroy(status.gameObject);
            }
            joinUI = FindObjectOfType<PlayerJoinedUI>();
        }
    }

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        joinUI = FindObjectOfType<PlayerJoinedUI>();
        DontDestroyOnLoad(gameObject);
        instance = this;
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        if (input.TryGetComponent<CharacterStatus>(out var cs))
        {
            players.Add(cs);
            DontDestroyOnLoad(cs.gameObject);
            cs.sprite.color = characterColors[players.Count - 1];
            if (joinUI != null)
            {
                joinUI.SetEntryActive(players.Count - 1);
            }
        }
    }
}
