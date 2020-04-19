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
        // if the audiosource is outside of max hearing range from the camera, and the clip is relatively short then don't play at all.
        // the clip length of 1 is an arbitrary value, if it is a longer soundtrack/sfx then the
        // player might pan into hearing range of the sound, and therefore making it relevant to play anyhow.
        // Otherwise we avoid sounds playing too far away to hear, and thereby occupying some of the slots for more relevant sfx.

        Vector3 distanceToListener = audioSource.transform.position - AudioListenerObject.transform.position;
        if (distanceToListener.magnitude > audioSource.maxDistance && clip.length < 1)
        {
            return;
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
