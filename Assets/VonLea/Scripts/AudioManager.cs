using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> _currentRunnindAudioSources = new List<AudioSource>();
    // dictionary audiosource als key, value is float as timer
    // wrapper class for audio source and float as timer, in update the timer runs out and notifies if clip ended
    // list with current running 

    public AudioManager Instance;

    private void AddAudioSource()
    {
        var newSource = transform.AddComponent<AudioSource>();
        _audioSourcePool.Enqueue(newSource);
    }

    private AudioSource GetAudioSource()
    {
        if(_audioSourcePool.Count == 0)
        {
            AddAudioSource();
        }

        var currentSource = _audioSourcePool.Dequeue();
        currentSource.gameObject.SetActive(true);
        return currentSource;
    }

    public void PlayAudioClip(AudioClip clip)
    {
        var audioSource = GetAudioSource();
        audioSource.clip = clip;
        audioSource.Play(); // add coroutine for playing the audio clip so that I can call REturnSourceToPool as soon as it finished playing?

    }

    private void ReturnSourceToPool(AudioSource source)
    {
        _audioSourcePool.Enqueue(source);
        source.clip = null;
        source.gameObject.SetActive(false);
    }

    //private IEnumerator WaitForEndOfClip(AudioSource source)
    //{
    //    return new WaitForSeconds(source.clip.
    //        kk);
    //}
}
