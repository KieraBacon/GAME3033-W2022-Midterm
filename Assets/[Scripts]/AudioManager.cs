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
            if (!_instance)
                _instance = FindObjectOfType<AudioManager>();
            if (!_instance)
                _instance = new GameObject("Sound Manager").AddComponent<AudioManager>();
            Init();
            return _instance;
        }
    }

    private static readonly int soundChannels = 8;
    private static Queue<AudioSource> audioSources = new Queue<AudioSource>();

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

    private static void Init()
    {
        if (audioSources.Count < soundChannels)
        {
            for (int i = audioSources.Count; i < soundChannels; i++)
            {
                AudioSource audioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.transform.SetParent(_instance.transform);
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
        AudioSource source = instance.FetchAudioSource();
        if (!source || !source.gameObject.activeInHierarchy) return;

        source.Stop();
        source.transform.position = Vector3.zero;
        source.spatialBlend = 0;
        source.loop = true;
        source.clip = clip;
        source.Play();
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
