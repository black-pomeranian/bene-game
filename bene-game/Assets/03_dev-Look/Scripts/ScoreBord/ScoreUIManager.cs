using UnityEngine;
using UnityEngine.UI;

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private Image[] scoreImages; // UIのImage要素の配列
    [SerializeField] private Sprite goal_icon;   // ゴール時のアイコン
    [SerializeField] private Sprite miss_icon;   // ミス時のアイコン


    private void Start()
    {


    }
    // 指定した番号のImageにゴールアイコンを設定
    public void SetScore_Goal(int num)
    {
        if (IsValidIndex(num))
        {
            scoreImages[num].sprite = goal_icon;
        }
    }

    // 指定した番号のImageにミスアイコンを設定

    public void SetScore_Miss(int num)
    {
        if (IsValidIndex(num))
        {
            scoreImages[num].sprite = miss_icon;
        }
    }

    // インデックスが有効か確認
    private bool IsValidIndex(int num)
    {
        if (num < 0 || num >= scoreImages.Length)
        {
            Debug.LogWarning($"Invalid index: {num}");
            return false;
        }
        return true;
    }
}

