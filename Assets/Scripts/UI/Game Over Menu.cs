using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.Events;

// handles what happens when you die
public class gameOver : MonoBehaviour
{
    public GameObject gameOverMenu;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject player;
    public UnityEvent respawn;

    // make sure time is running when we start
    void Start()
    {
        Time.timeScale = 1f;
    }

    // check constantly if the player is dead
    void Update()
    {
        if (stats.dead == true)
        {
            Stop();
        }
    }

    // freeze everything and show the death screen
    public void Stop()
    {
        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // respawn button pressed
    public void Play()
    {
        Time.timeScale = 1f; // unfreeze
        CharacterController cc = player.GetComponent<CharacterController>();

        // turn off movement so we can teleport
        if (cc != null) cc.enabled = false;

        player.transform.position = stats.spawnPoint;

        // turn movement back on
        if (cc != null) cc.enabled = true;
        player.transform.root.position = stats.spawnPoint;
        gameOverMenu.SetActive(false);
        stats.dead = false;

        // lock the mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // reset inputs and animation
        respawn.Invoke();
        player.GetComponent<PlayerCombat>().RespawnInput();
        Animator anim = player.GetComponentInChildren<Animator>();
        anim.SetTrigger("Respawn");
    }

    // go back to title screen
    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // exit the game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("The Player has Quit the game");
    }
}