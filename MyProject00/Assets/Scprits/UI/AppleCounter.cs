using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class AppleCounter : MonoBehaviour
{
    private TextMeshProUGUI _total_apple_text;
    
    private int current_total_apple = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _total_apple_text = GetComponent<TextMeshProUGUI>();
        //if (_total_apple_text == null)
        //{
        //    Debug.LogError("TextMeshProUGUI component not found on this GameObject");
        //    Destroy(this);
        //    return;             //コンポーネントが見つからなければこれ以上処理しない
        //}

        //if (GManager.instance != null)
        //{
        //    _total_apple_text.text = "SCORE : " + GManager.instance.score;
        //}
        //else
        //{
        //    Debug.Log("Game Manager is not! Maybe forgrt to put GameManager in this Scene");
        //    Destroy(this);
        //}
        UpdateAppleCountText();
    }

    public void ControlAppleCount(bool flg)
    {
        //リンゴを取得していれば増加
        if (flg)
        {
            current_total_apple++;
        }
        //リンゴを失っていれば減少
        else
        {
            current_total_apple--;
        }

        //合計数が0を下回る場合、0に固定する
        const int count_min = 0;
        if(current_total_apple < count_min)
        {
            current_total_apple = count_min;
        }

        //UIテキストを表示
        UpdateAppleCountText();
    }

    //UIテキストを更新するPrivate Method
    private void UpdateAppleCountText()
    {
        if(_total_apple_text != null)
        {
            //テキスト表示の例
            _total_apple_text.text = "x " + current_total_apple.ToString();
        }
        else
        {
            Debug.LogWarning("AppleCount not apply.");
        }
    }
}