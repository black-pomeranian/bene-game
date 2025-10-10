using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject selectUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endUI;
    /*[SerializeField] private GameObject soundUI;*/

    [SerializeField] private TextMeshProUGUI textWin;
    [SerializeField] private TextMeshProUGUI textLose;
    [SerializeField] private TextMeshProUGUI textPlayerScore;
    [SerializeField] private TextMeshProUGUI textCpuScore;

    /*[SerializeField] GameObject panelKick;
    [SerializeField] GameObject panelSave;*/

    [SerializeField] private float idleTimeToPlayVideo = 30f;
    [SerializeField] private GameObject videoPlayerObj;

    [SerializeField] private BgmPlayer bgm_player;

    private float idleTimer = 0f;
    private bool isVideoPlaying = false;
    private Vector3 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        // 初期状態としてStartUIのみアクティブにする例
        EnableStartUI();
        DisableGameUI();
        DisableEndUI();
        /*EnableSoundUI();*/

        var vp = videoPlayerObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (vp != null)
        {
            vp.loopPointReached += OnIdleVideoFinished;
        }
    }

    private void Update()
    {
        if (startUI.activeSelf && !isVideoPlaying)
        {
            if (IsAnyInputDetected())
            {
                idleTimer = 0f;
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTimeToPlayVideo)
                {
                    PlayIdleVideo();
                }
            }
        }

        // 動画再生中に入力があれば中断
        if (isVideoPlaying && IsAnyInputDetected())
        {
            StopIdleVideo();
        }
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

    public void EnableSelectionUI()
    {
        if (selectUI != null)
        {
            selectUI.SetActive(true);
        }
        else
        {
            Debug.LogError("selectUIオブジェクトがアサインされていません。");
        }
    }

    public void DisableSelectionUI()
    {
        if (selectUI != null)
        {
            selectUI.SetActive(false);
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

    /*public void EnableSoundUI()
    {
        if (soundUI != null)
        {
            soundUI.SetActive(true);
        }
        else
        {
            Debug.LogError("SoundUIオブジェクトがアサインされていません。");
        }
    }*/

    /*public void DisableSoundUI()
    {
        if (soundUI != null)
        {
            soundUI.SetActive(false);
        }
    }*/

    public void SetResult(string result, int playerScore, int cpuScore)
    {
        textPlayerScore.text = playerScore.ToString();
        textCpuScore.text = cpuScore.ToString();

        // 一旦非表示にしてから該当するものだけ表示
        textWin.gameObject.SetActive(false);
        textLose.gameObject.SetActive(false);

        if (result == "WIN")
        {
            textWin.gameObject.SetActive(true);
        }
        else if (result == "LOSE")
        {
            textLose.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"不明なresult値: {result}");
        }
    }

    /*public void SetPanelKick()
    {
        panelKick.gameObject.SetActive(true);
        panelSave.gameObject.SetActive(false);
    }*/

    /*public void SetPanelSave()
    {
        panelKick.gameObject.SetActive(false);
        panelSave.gameObject.SetActive(true);
    }*/

    private bool IsAnyInputDetected()
    {
        bool mouseMoved = Input.mousePosition != lastMousePosition;
        lastMousePosition = Input.mousePosition;

        return Input.anyKeyDown || Input.mouseScrollDelta.sqrMagnitude > 0 ||
            Input.GetMouseButtonDown(0) || Input.touchCount > 0 || mouseMoved;
    }

    private void PlayIdleVideo()
    {
        isVideoPlaying = true;
        videoPlayerObj.SetActive(true);
        var vp = videoPlayerObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (vp != null)
        {
            vp.Play();
        }

        bgm_player.StopBGM();
        DisableStartUI();

    }

    private void StopIdleVideo()
    {
        isVideoPlaying = false;
        idleTimer = 0f;

        var vp = videoPlayerObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (vp != null)
        {
            vp.Stop();
        }

        videoPlayerObj.SetActive(false);

        bgm_player.PlayBGM(0);
        EnableStartUI();
    }

    private void OnIdleVideoFinished(UnityEngine.Video.VideoPlayer vp)
    {
        StopIdleVideo();
    }
}
