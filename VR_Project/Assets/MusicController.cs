using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    [SerializeField] private AudioClip menuClip, endClip;
    // Start is called before the first frame update
    private string lastScene, currentScene;

    private static MusicController instance = null;

    public static MusicController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (MusicController)FindObjectOfType(typeof(MusicController));
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            audioSource = GetComponent<AudioSource>();
            lastScene = SceneManager.GetActiveScene().name;
            if (lastScene == "StartScene")
            {
                audioSource.clip = menuClip;
                audioSource.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != lastScene)
        {
            lastScene = currentScene;
            audioSource.Stop();
            ChangeSong();
        }
    }

    void ChangeSong()
    {
        if (!audioSource.isPlaying)
        {
            switch (lastScene)
            {
                case "StartScene":
                    {
                        audioSource.clip = menuClip;
                        audioSource.Play();
                        break;
                    }
                case "GameScene":
                    {
                        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                        audioSource.Play();
                        break;
                    }
                case "EndScene":
                    {
                        audioSource.clip = endClip;
                        audioSource.Play();
                        break;
                    }
                default:
                    break;
            }
        }
    }


}
