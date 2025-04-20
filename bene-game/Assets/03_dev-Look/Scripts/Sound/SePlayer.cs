using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SePlayer : MonoBehaviour
{
    [Header("SE Settings")]
    [SerializeField] private AudioClip[] seClips; // 効果音のAudioClipを格納する配列
    [SerializeField] private AudioSource seSource;

    public void PlaySE(int seIndex)
    {
        if (seClips == null || seClips.Length == 0 || seIndex < 0 || seIndex >= seClips.Length)
        {
            Debug.LogError("指定された効果音が存在しません。");
            return;
        }

        if (seSource != null && seClips[seIndex] != null)
        {
            seSource.PlayOneShot(seClips[seIndex]);
        }
        else
        {
            Debug.LogError("AudioSourceが存在しないか、指定された効果音がnullです。");
        }
    }

    public void PlaySelectSE()
    {
        seSource.PlayOneShot(seClips[0]);
    }

    public void PlaWhistleSE()
    {
        seSource.PlayOneShot(seClips[1]);
    }

    public void PlaKickSE()
    {
        seSource.PlayOneShot(seClips[2]);
    }

    public void PlaGameEndSE()
    {
        seSource.PlayOneShot(seClips[3]);
    }

}
