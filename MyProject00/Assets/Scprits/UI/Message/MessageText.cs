using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MessageText : FieldMessageBase
{
    [SerializeField, Header("Serif")]
    private List<string> _messages;

    [SerializeField, Header("表示速度")]
    [Range(0.01f, 0.5f)] // 0.01秒から0.5秒の範囲で調整可能
    private float _textSpeed = 0.05f; // 一文字あたりの表示速度 (例: 0.05秒)

    private bool _is_input_received = false;

    protected override IEnumerator OnAction()
    {
        for (int i = 0; i < _messages.Count; i++)
        {
            // 会話をWindowのTextフィールドに一文字ずつ表示
            // ShowMessageがコルーチンを返すようになったので、yield returnで待機
            yield return ShowMessageCoroutine(_messages[i]);

            // ここでInput Systemからの入力待ちフラグをリセット
            _is_input_received = false;

            //指定されたキーが押された場合、次のメッセージを表示
            yield return new WaitUntil(()=> _is_input_received);
        }

        yield break;
    }

    //文字を一文字ずつ表示するコルーチン
    private IEnumerator ShowMessageCoroutine(string message)
    {
        if (target == null)
        {
            Debug.LogError("TextMeshProUGUI target is not assigned!");
            yield break;
        }

        target.text = ""; // 表示テキストをクリア

        for (int i = 0; i < message.Length; i++)
        {
            target.text += message[i]; // 一文字追加
            yield return new WaitForSeconds(_textSpeed); // 指定された速度で待機
        }

        // 全ての文字が表示された後、念のため再度、完全に表示された状態にする（タイプミス防止）
        target.text = message;
    }

    public void DisplayNextMessage(InputAction.CallbackContext context)
    {
        Debug.Log("OnNextMessageInput called!"); // まずはここが呼ばれるか確認
        if (context.performed)
        {
            Debug.Log("Action Performed!"); // その後ここが呼ばれるか確認
            //ボタンが押された瞬間だけ反応させる
            _is_input_received = true;
        }
    }
}
