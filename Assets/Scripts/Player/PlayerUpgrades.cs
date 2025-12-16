using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// keeps track of what abilities and keys the player has
public class PlayerUpgrades : MonoBehaviour
{
    public bool canSprint = false;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canClimb = false;
    public bool allUnlocked = false; // true if we have every power
    public bool puzzleKey = false;
    public bool combatKey = false;
    public bool platformKey = false;
    public bool allKey = false; // true if we found all three keys
    public int arenaKill = 0; // counts how many enemies we beat in the arena
    public bool inFortress = false;
    public playerStatus status;
    public Objectives objectives;

    // checks the name of the upgrade and turns it on
    public void unlock(string unlock)
    {
        if (unlock == "dash")
        {
            canDash = true;
            status.updateDashText(0, 0, true, true);
        }
        if (unlock == "sprint") canSprint = true;
        if (unlock == "climb") canClimb = true;
        if (unlock == "doublejump") canDoubleJump = true;

        // checking for keys
        if (unlock == "puzzle")
        {
            puzzleKey = true;
            objectives.completeObjective(1);
        }
        if (unlock == "combat")
        {
            combatKey = true;
            objectives.completeObjective(2);
        }
        if (unlock == "platform")
        {
            platformKey = true;
            objectives.completeObjective(3);
        }

        // check if we have everything now
        if (canDoubleJump && canClimb && canDash && canSprint) allUnlocked = true;
        if (puzzleKey && combatKey && platformKey) allKey = true;
    }

    // counts kills in the arena to give the combat key
    public void arenaDeath()
    {
        arenaKill += 1;
        if (arenaKill >= 8) unlock("combat");
    }

    // tracking where the player is for sound effects
    public void enterFortress()
    {
        inFortress = true;
    }
    public void exitFortress()
    {
        inFortress = false;
    }
}