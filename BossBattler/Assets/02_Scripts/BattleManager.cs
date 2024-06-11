using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    [SerializeField] ElectroGolem boss;
    [SerializeField] Transform[] playerSpawns;
    [SerializeField] CharacterUI[] playerUI;
    [SerializeField] MessageRevealer text;
    [SerializeField] float playerSpawnInterval;
    [SerializeField, TextArea] string startText;
    [SerializeField, TextArea] string winText;
    [SerializeField, TextArea] string loseText;

    private int defeatCounter;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputManager.instance.DisableJoining();
        instance = this;
        StartCoroutine(StartFight());
    }

    public void OnPlayerDefeated()
    {
        defeatCounter--;
        if(defeatCounter == 0)
        {
            StartCoroutine(OnLose());
        }
    }

    public void OnBossDefeated()
    {
        StartCoroutine(OnWin());
    }

    private IEnumerator StartFight()
    {
        yield return new WaitForSeconds(1);

        List<CharacterStatus> players = PlayerConnector.instance.players;
        defeatCounter = players.Count;
        for(int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = playerSpawns[i].position;
            players[i].gameObject.SetActive(true);
            players[i].Setup(playerUI[i]);
            players[i].sprite.enabled = true;
            ParticleManager.SpawnParticles(2, playerSpawns[i].position, Vector3.one, Quaternion.identity);
            yield return new WaitForSeconds(playerSpawnInterval);
        }
        text.ChangeText(startText);
        text.ActivateText();
        yield return new WaitForSeconds(4);
        text.ClearText(); 
        for (int i = 0; i < players.Count; i++)
        {
            players[i].EnableCharacter();
        }
        boss.StartFight();
    }

    private IEnumerator OnWin()
    {
        yield return new WaitForSeconds(1);
        text.ChangeText(winText);
        text.ActivateText();
        yield return new WaitForSeconds(4);
        End();
    }
    private IEnumerator OnLose()
    {
        boss.Stop();
        yield return new WaitForSeconds(1);
        text.ChangeText(loseText); 
        text.ActivateText();
        yield return new WaitForSeconds(3);
        End();
    }

    private void End()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        PlayerInputManager.instance.EnableJoining();
    }
}
