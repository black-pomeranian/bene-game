using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{

    [Header("BGM Settings")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioSource bgmSource;
    private int currentBgmIndex = 0;

    public void PlayBGM(int bgmIndex)
    {

        if (bgmClips == null || bgmClips.Length == 0 || bgmIndex < 0 || bgmIndex >= bgmClips.Length)
        {
            Debug.LogError("éwíËÇ≥ÇÍÇΩBGMÇ™ë∂ç›ÇµÇ‹ÇπÇÒÅB");
            return;
        }

        if (currentBgmIndex == bgmIndex && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = bgmClips[bgmIndex];
        if (gameObject.activeInHierarchy)
        {
            bgmSource.Play();
        }
        currentBgmIndex = bgmIndex;
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            currentBgmIndex = -1;
        }
    }

}
