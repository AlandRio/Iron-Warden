using UnityEngine;

// just holds all the numbers for the enemy stats
public class AiStats : MonoBehaviour
{
    public float att; // how much damage they do
    public float attspeed; // how fast they attack
    public float movespeed; // how fast they move
    public float hp; // starting health
    public float flyHeight; // how high up they fly
    public float AttackDistance; // how close they need to be to hit you
    public float DetectDistance; // how far away they can see the player
}