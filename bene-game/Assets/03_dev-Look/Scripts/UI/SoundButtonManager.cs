using UnityEngine;
using UnityEngine.UI;

public class SoundButtonManager : MonoBehaviour
{
    Image sourceImage;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;
    [SerializeField] GameObject sound;

    private bool isSoundOn = false;
    private Button button;

    void Start()
    {
        // ボタンコンポーネントを取得
        button = GetComponent<Button>();
        sourceImage = this.GetComponent<Image>();
        if (button == null)
        {
            Debug.LogError("Buttonコンポーネントがアタッチされていません。");
            enabled = false; // スクリプトを無効にする
            return;
        }

        // 初期状態を設定
        UpdateImage();

        // ボタンのクリックイベントにリスナーを追加
        button.onClick.AddListener(ToggleButtonSound);
    }

    void ToggleButtonSound()
    {
        isSoundOn = !isSoundOn;
        UpdateImage();
        UpdateSoundManager();
    }

    void UpdateImage()
    {
        if (sourceImage != null)
        {
            sourceImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
        else
        {
            Debug.LogError("SourceImageがアサインされていません。");
        }
    }

    void UpdateSoundManager()
    {
        sound.SetActive(isSoundOn);
    }
}