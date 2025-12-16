using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// a teleport door to leave the boss room
public class DoorCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject tpTarget; // where we end up
    [SerializeField] private GameObject BossUI;
    public GameObject doorCanvas;
    public AudioManager audioManager;

    public string GetDescription()
    {
        return "Leave";
    }

    // ask player if they want to leave
    public void Interact()
    {
        Time.timeScale = 0f;
        doorCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // cancel
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f;
        doorCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // actually leave
    public void openDoor()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 1f;

        // temporarily turn off controller so we can teleport safely
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = tpTarget.transform.position;
        if (cc != null) cc.enabled = true;
        player.transform.root.position = tpTarget.transform.position;

        // clean up UI and music
        doorCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<PlayerCombat>().RespawnInput();
        BossUI.SetActive(false);
        audioManager.PlaySFX(audioManager.door);
        audioManager.PlayMusic(audioManager.Music);
        player.GetComponent<PlayerUpgrades>().exitFortress();

    }
}