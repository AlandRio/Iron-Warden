using UnityEngine;

// keeps track of which ending the player got
public class Ending : MonoBehaviour
{
    // makes sure there is only one of these in the game
    public static Ending Instance;

    // true if we saved everyone, false if we didn't
    public bool goodEnding = false;

    // setup the singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // don't delete this when changing scenes
        }
        else
        {
            Destroy(gameObject); // destroy any extras
        }
    }

    // update the ending variable
    public void setGoodEnding(bool end)
    {
        goodEnding = end;
    }
}