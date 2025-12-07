using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeController : MonoBehaviour
{
    public Slider volumeSlider;
    public BGMManager bgmManager;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        volumeSlider.value = savedVolume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float volume)
    {
        bgmManager.SetVolume(volume);
    }
}
