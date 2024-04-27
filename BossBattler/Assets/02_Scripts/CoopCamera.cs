using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopCamera : MonoBehaviour
{
    private List<CharacterStatus> characters = new List<CharacterStatus>();

    private void Start()
    {
        characters = PlayerConnector.instance.players;
        PlayerConnector.instance.im.onPlayerJoined += OnPlayerJoin;
    }

    private void Update()
    {
        if(characters.Count < 1) { return; }
        Vector2 newPos = new();
        foreach (CharacterStatus character in characters)
        {
            newPos += (Vector2)character.transform.position;
        }
        newPos /= characters.Count;



        transform.position = new Vector3(newPos.x, newPos.y, -10);


    }

    private void OnPlayerJoin(PlayerInput input)
    {
        characters.Add(input.GetComponent<CharacterStatus>());
    }
}
