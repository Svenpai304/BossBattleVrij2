using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource src;

    public void PlaySFX(AudioClip clip, float deviatePitch = 0f, float volume = 1f)
    {
        src.clip = clip;
        src.pitch = 1f + Random.Range(-deviatePitch, deviatePitch);
        src.volume = volume;
        src.Play();
    }
    void Update()
    {
        if (!src.isPlaying) Destroy(gameObject);
    }
}
