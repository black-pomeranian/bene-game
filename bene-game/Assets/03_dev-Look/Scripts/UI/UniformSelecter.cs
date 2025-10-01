using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UniformSelector : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    [SerializeField] private Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private Vector3 normalScale = Vector3.one;

    private int selectedIndex = -1;

    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // キャプチャ用ローカル変数
            buttons[i].onClick.AddListener(() => OnButtonSelected(index));
            buttons[i].transform.localScale = normalScale;
        }
    }

    private void OnButtonSelected(int index)
    {
        // すでに選択中なら何もしない
        if (selectedIndex == index) return;

        // 前の選択を元に戻す
        if (selectedIndex >= 0 && selectedIndex < buttons.Count)
        {
            buttons[selectedIndex].transform.localScale = normalScale;
        }

        // 新しい選択を反映
        selectedIndex = index;
        buttons[selectedIndex].transform.localScale = selectedScale;

        Debug.Log("Selected Index: " + selectedIndex);
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
}
