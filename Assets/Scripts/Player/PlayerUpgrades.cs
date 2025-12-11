using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public bool canSprint = false;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canWallClimb = false;

    public void unlockDash()
    {
        canDash = true;
    }
    public void unlockDoubleJump()
    {
        canDoubleJump = true;
    }
    public void unlockSprint()
    {
        canSprint = true;
    }
    public void unlockClimb()
    {
        canWallClimb = true;
    }
}
