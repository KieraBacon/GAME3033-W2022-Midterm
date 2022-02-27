using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    private static void Init()
    {

    }

    private static int soundChannels = 8;
    private static Queue<AudioSource> audioSources = new Queue<AudioSource>();

    private void Awake()
    {
        for (int i = 0; i < soundChannels; i++)
        {
            audioSources.Enqueue(new GameObject().AddComponent<AudioSource>());
        }
    }

    private AudioSource FetchAudioSource()
    {
        AudioSource source = audioSources.Dequeue();
        audioSources.Enqueue(source);
        return source;
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource source = FetchAudioSource();
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void PlayClipAtPosition(AudioClip clip, Vector3 position)
    {

    }
}
