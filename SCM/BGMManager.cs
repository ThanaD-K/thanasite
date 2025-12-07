using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // Optional: load saved volume
        float volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }
}

