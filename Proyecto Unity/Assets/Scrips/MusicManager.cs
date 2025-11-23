using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    [Header("Rango del loop en segundos")]
    public float loopStart = 0f;
    public float loopEnd = 32f;

    // Start is called before the first frame update
    void Start()
    {
        if(audioSource == null)
        audioSource = GetComponent<AudioSource>();

        audioSource.time = loopStart;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying) return;

        if (audioSource.time >= loopEnd)
        {
            audioSource.time = loopStart;
            audioSource.Play();
        }
    }
}
