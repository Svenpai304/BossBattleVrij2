using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Controller : MonoBehaviour
{
    public static SFX_Controller co;
    public SFX spawnSFX;
    private void Awake()
    {
        co = this;
    }
    public void PlaySFX(AudioClip clip, Vector3 pos, float deviatePitch = 0f, float volume = 1f)
    {
        SFX sfx =Instantiate(spawnSFX, pos, Quaternion.identity);
        sfx.PlaySFX(clip, deviatePitch, volume);
    }
    public void PlaySFX(AudioClip[] clips, Vector3 pos, float deviatePitch = 0f, float volume = 1f)
    {
        PlaySFX(clips[Random.Range(0, clips.Length)], pos, deviatePitch, volume);
    }
}
