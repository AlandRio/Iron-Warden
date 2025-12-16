using TMPro;
using UnityEngine;

// holds all the numbers for the player, like health and damage
public class PlayerStats : MonoBehaviour
{
    public TMP_Text deathText;
    public float attack;
    public float attackSpeed;
    public float health;
    public float moveSpeed = 4f;
    public float sprintSpeed = 4f;
    public float jumpForce = 2f;
    public float dashCD = 2f; // how long to wait before dashing again
    public bool dead = false;
    public int deaths = 1;
    public Vector3 spawnPoint = new Vector3(0, 5, 0); // where we respawn

    // count the death and update the text on robot
    public void die()
    {
        dead = true;
        deaths += 1;

        if (deaths < 10)
        {
            deathText.text = "000" + deaths.ToString();
        }
        else if (deaths < 100)
        {
            deathText.text = "00" + deaths.ToString();

        }
        else if (deaths < 1000)
        {
            deathText.text = "0" + deaths.ToString();

        }
        else
        {
            deathText.text = deaths.ToString();
        }
    }

    // reset the dead flag
    public void respawn()
    {
        dead = false;
    }

}