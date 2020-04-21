using System;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;
    [Range(1, 25)]
    public int MaxConcurrentSFX;
    [Range(0,2)]
    public int AdditionalAudioDelayInSeconds;
    [Range(25, 75)]
    public int MaxAudioDistance;
    public Camera AudioListenerObject;
    private volatile int currentlyPlayingSFXCount;

    public static AudioManager Instance { get { return _instance; } }

    public Sound[] sounds;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Play(AudioSource audioSource, AudioClip clip)
    {
        if (AudioListenerObject == null)
        {
            AudioListenerObject = FindObjectOfType<Camera>();
        }

        if(!audioSource.isPlaying && currentlyPlayingSFXCount < MaxConcurrentSFX)
        {
            currentlyPlayingSFXCount++;
            audioSource.PlayOneShot(clip);
            StartCoroutine(DelayedDecrement(Mathf.CeilToInt(clip.length)));
        }
    }

    private IEnumerator DelayedDecrement(int seconds)
    {
        yield return new WaitForSeconds(seconds + AdditionalAudioDelayInSeconds);
        currentlyPlayingSFXCount--;
    }

    internal void PlayRandomSFX(AudioSource audioSource)
    {
        int rnd = UnityEngine.Random.Range(0, sounds.Length);
        if(sounds[rnd] != null)
        {
            audioSource.maxDistance = MaxAudioDistance;
            Play(audioSource, sounds[rnd].clip);
        }
    }

    internal void PlayDeathSFX(AudioSource audioSource)
    {
        audioSource.Stop();
        AudioClip clip = Array.Find(sounds, sounds => sounds.name == "death").clip;
        audioSource.PlayOneShot(clip);
    }
}
