using UnityEngine;

// controls all the music and sound effects in the game
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // --- sound clips ---
    public AudioClip Music;
    public AudioClip secondMusic;
    public AudioClip thirdMusic;
    public AudioClip footstepGrass1;
    public AudioClip footstepGrass2;
    public AudioClip footstepGrass3;
    public AudioClip footstepGrass4;
    public AudioClip footstepFort1;
    public AudioClip footstepFort2;
    public AudioClip footstepFort3;
    public AudioClip footstepFort4;
    public AudioClip click;
    public AudioClip close;
    public AudioClip death;
    public AudioClip punch;
    public AudioClip select;
    public AudioClip door;
    public AudioClip explode;

    // play the main music when the game starts
    private void Start()
    {
        PlayMusic(Music);
    }

    // play a sound effect just once (like a jump or hit)
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // switch the background music loop
    public void PlayMusic(AudioClip clip)
    {
        musicSource.volume = 0.4f;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // change volume for settings menu
    public void setAudiolevel(string type, float value)
    {
        if (type == "music") { musicSource.volume = value; }
        if (type == "sfx") { sfxSource.volume = value; }
    }

    // get current volume for the sliders
    public float getAudiolevel(string type)
    {
        if (type == "music") return musicSource.volume;
        if (type == "sfx") return sfxSource.volume;
        else return 1f;
    }
}