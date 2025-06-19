using UnityEngine;
using TMPro;
using System.Collections;

public abstract class FieldMessageBase : MonoBehaviour
{
    [Header("Canvas")]
    public Canvas serif_window; //セリフ用の枠
    [Header("Text")]
    public TextMeshProUGUI target;

    private bool is_contacted;

    private IEnumerator coroutine; //コルーチン

    void Start()
    {
        // Nullチェックを追加: Inspectorで設定されていない可能性も考慮
        if (serif_window != null)
        {
            serif_window.gameObject.SetActive(false);
        }

        is_contacted = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            is_contacted = true;
            if (coroutine == null)
            {
                // コルーチン生成前にnullチェックを追加
                if (serif_window != null) // serif_windowが破棄されているならコルーチンを開始しない
                {
                    coroutine = CreateCoroutine();
                    StartCoroutine(coroutine);
                }
                else
                {
                    Debug.LogWarning("serif_window is null when OnTriggerEnter2D is called. Cannot start message coroutine.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            is_contacted = false;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null; // コルーチンが停止したらnullに戻す

                // ここでnullチェックを追加
                if (serif_window != null)
                {
                    CloseMessageWindow();
                }
                else
                {
                    Debug.LogWarning("serif_window is null when OnTriggerExit2D is called. Cannot close message window.");
                }
            }
        }
    }

    private IEnumerator CreateCoroutine()
    {
        // コルーチン内で最初にnullチェック
        if (serif_window == null)
        {
            Debug.LogError("serif_window is null. Cannot display message window.");
            yield break; // コルーチンをここで終了
        }

        //セリフ枠を起動
        serif_window.gameObject.SetActive(true);

        //セリフ表示開始(中身は子クラスにて)
        yield return OnAction();

        if (!is_contacted)
        {
            //完全にメッセージ表示が終わったらウィンドウを閉じる
            yield return new WaitForSeconds(1.0f); // 例えば1秒待つ

            // ここでもnullチェックを追加
            if (serif_window != null)
            {
                CloseMessageWindow();
            }
        }
    }

    private void CloseMessageWindow()
    {
        // 最も重要なnullチェック
        if (serif_window == null)
        {
            // 既に破棄されている場合は何もしない
            Debug.LogWarning("Attempted to close message window, but serif_window is already null.");
            return;
        }

        //targetも念のためnullチェック
        if (target != null)
        {
            target.text = ""; //表示テキストを無にする
        }
        serif_window.gameObject.SetActive(false); //Window表示
    }

    protected abstract IEnumerator OnAction();
}