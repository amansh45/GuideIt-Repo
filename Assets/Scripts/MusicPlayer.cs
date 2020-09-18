using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> musicList;
    AudioSource audioSource;
    PlayerStatistics playerStats;

    private void Awake()
    {
        int musicPlayerCount = FindObjectsOfType<MusicPlayer>().Length;
        if (musicPlayerCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicList[Random.Range(0, musicList.Count + 1)];
        audioSource.volume = playerStats.musicVolume;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
