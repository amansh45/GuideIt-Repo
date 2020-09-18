using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustSettings : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider, sfxVolumeSlider;
    PlayerStatistics playerStats;
    MusicPlayer musicPlayer;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        musicVolumeSlider.value = playerStats.musicVolume;
        sfxVolumeSlider.value = playerStats.sfxVolume;
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    void OnMusicVolumeChange()
    {
        playerStats.musicVolume = musicVolumeSlider.value;
        musicPlayer.gameObject.GetComponent<AudioSource>().volume = playerStats.musicVolume;
    }

    void OnSFXVolumeChange()
    {
        playerStats.sfxVolume = sfxVolumeSlider.value;
    }
}
