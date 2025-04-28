using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject soundUI;

    // Start is called before the first frame update
    void Start()
    {
        // 初期状態としてStartUIのみアクティブにする例
        EnableStartUI();
        DisableGameUI();
        DisableEndUI();
        EnableSoundUI();
    }

    public void EnableStartUI()
    {
        if (startUI != null)
        {
            startUI.SetActive(true);
        }
        else
        {
            Debug.LogError("StartUIオブジェクトがアサインされていません。");
        }
    }

    public void DisableStartUI()
    {
        if (startUI != null)
        {
            startUI.SetActive(false);
        }
    }

    public void EnableGameUI()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(true);
        }
        else
        {
            Debug.LogError("GameUIオブジェクトがアサインされていません。");
        }
    }

    public void DisableGameUI()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }
    }

    public void EnableEndUI()
    {
        if (endUI != null)
        {
            endUI.SetActive(true);
        }
        else
        {
            Debug.LogError("EndUIオブジェクトがアサインされていません。");
        }
    }

    public void DisableEndUI()
    {
        if (endUI != null)
        {
            endUI.SetActive(false);
        }
    }

    public void EnableSoundUI()
    {
        if (soundUI != null)
        {
            soundUI.SetActive(true);
        }
        else
        {
            Debug.LogError("SoundUIオブジェクトがアサインされていません。");
        }
    }

    public void DisableSoundUI()
    {
        if (soundUI != null)
        {
            soundUI.SetActive(false);
        }
    }
}