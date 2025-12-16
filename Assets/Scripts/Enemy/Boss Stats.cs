using UnityEngine;

// holds all the important numbers for the boss
public class BossStats : MonoBehaviour
{
    public float att; // how much damage the boss does
    public float attspeed; // how fast he attacks
    public float movespeed; // how fast he chases you
    public float hp; // starting health
    public float flyHeight; // how high up he flies
    public float AttackDistance; // how close he needs to be to hit you
    public float DetectDistance; // how far away he can see the player

}