using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class FieldMessageBase : MonoBehaviour
{
    [Header("Canvas")]
    public Canvas serif_window;         //セリフ用の枠
    [Header("Text")]
    public TextMeshProUGUI target;

    private bool is_contacted;
    
    private IEnumerator coroutine;      //コルーチン

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //window停止
        serif_window.gameObject.SetActive(false);

        is_contacted = false;
    }

    // colliderをもつオブジェクトの領域に入ったとき
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            is_contacted = true;
            // プレイヤーが入ってきたときに、まだコルーチンが始まっていない場合のみ開始
            if (coroutine == null)
            {
                coroutine = CreateCoroutine();
                StartCoroutine(coroutine);
            }
        }
    }

    // colliderをもつオブジェクトの領域外にでたとき
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            is_contacted = false;

            // プレイヤーが範囲外に出たら、コルーチンを停止し、リソースをクリアする
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                CloseMessageWindow(); // ウィンドウを閉じる処理をまとめる
            }
        }
    }

    //コルーチンを生成
    private IEnumerator CreateCoroutine()
    {
        //セリフ枠を起動
        serif_window.gameObject.SetActive(true);

        //セリフ表示開始(中身は子クラスにて)
        yield return OnAction();

        if (!is_contacted)
        {
            //完全にメッセージ表示が終わったらウィンドウを閉じる
            yield return new WaitForSeconds(1.0f); // 例えば1秒待つ
            CloseMessageWindow();
        }
    }

    //テキストウィンドウを閉じる
    private void CloseMessageWindow()
    {
        //Window終了
        target.text = "";                          //表示テキストを無にする
        serif_window.gameObject.SetActive(false);  //Window表示
    }

    //テキスト表示を起動
    protected abstract IEnumerator OnAction();

    //メッセージを表示
    protected void ShowMessage(string message)
    {
        this.target.text = message;
    }
}
