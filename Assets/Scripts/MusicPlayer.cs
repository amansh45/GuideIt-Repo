using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> musicList;
    
    private void Awake()
    {
        int playerStatsCount = FindObjectsOfType<MusicPlayer>().Length;
        if (playerStatsCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicList[Random.Range(0, musicList.Count + 1)];
        audioSource.Play();
    }
}
