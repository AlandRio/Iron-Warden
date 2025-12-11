using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.Events;

public class gameOver : MonoBehaviour
{
    public GameObject gameOverMenu;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject player;
    public UnityEvent respawn;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }


    // Update is called once per frame
    void Update()
    {
        if (stats.dead == true)
        {
            Stop();
        }
    }

    public void Stop()
    {
        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play()
    {
        Time.timeScale = 1f;
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        player.transform.position = stats.spawnPoint;

        if (cc != null) cc.enabled = true;
        player.transform.root.position = stats.spawnPoint;
        gameOverMenu.SetActive(false);
        stats.dead = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        respawn.Invoke(); 
        player.GetComponent<PlayerCombat>().RespawnInput();
        Animator anim = player.GetComponentInChildren<Animator>();
        anim.SetTrigger("Respawn");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("The Player has Quit the game");
    }
}