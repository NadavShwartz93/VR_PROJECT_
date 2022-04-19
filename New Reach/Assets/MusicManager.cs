using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource gameSongAudioSource;
    [SerializeField] private AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        gameSongAudioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        gameSongAudioSource.Play();
    }
}
