using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private static AudioManager instance
    {
        get
        {
            if (isQuitting) return null;

            if (!_instance)
                _instance = FindObjectOfType<AudioManager>();
            if (!_instance)
                _instance = new GameObject("Sound Manager").AddComponent<AudioManager>();
            Init();
            return _instance;
        }
    }

    private static readonly int soundChannels = 8;
    private static AudioSource musicSource = null;
    private static Queue<AudioSource> audioSources = new Queue<AudioSource>();
    private static bool isQuitting = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private static void Init()
    {
        musicSource = new GameObject("Music Source").AddComponent<AudioSource>();
        musicSource.transform.SetParent(_instance.transform);
        musicSource.playOnAwake = false;

        if (audioSources.Count < soundChannels)
        {
            for (int i = audioSources.Count; i < soundChannels; i++)
            {
                AudioSource audioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
                audioSource.transform.SetParent(_instance.transform);
                audioSource.playOnAwake = false;
                audioSources.Enqueue(audioSource);
            }
        }
    }

    private AudioSource FetchAudioSource()
    {
        AudioSource source = audioSources.Dequeue();
        audioSources.Enqueue(source);
        return source;
    }

    public static void PlayMusic(AudioClip clip)
    {
        AudioManager audioManager = instance;
        if (!musicSource || !musicSource.gameObject.activeInHierarchy) return;

        musicSource.Stop();
        musicSource.transform.position = Vector3.zero;
        musicSource.spatialBlend = 0;
        musicSource.loop = true;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public static void PlayClip(AudioClip clip)
    {
        AudioSource source = instance.FetchAudioSource();
        if (!source || !source.gameObject.activeInHierarchy) return;

        Debug.Log("<Color=Red>Playing " + clip.name + "!</Color>");

        source.Stop();
        source.transform.position = Vector3.zero;
        source.spatialBlend = 0;
        source.loop = false;
        source.clip = clip;
        source.Play();
    }

    public static void PlayClipAtPosition(AudioClip clip, Vector3 position)
    {
        AudioSource source = instance.FetchAudioSource();
        if (!source || !source.gameObject.activeInHierarchy) return;

        source.Stop();
        source.transform.position = position;
        source.spatialBlend = 1;
        source.loop = false;
        source.clip = clip;
        source.Play();
    }
}
