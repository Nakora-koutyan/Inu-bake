using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MessageText : FieldMessageBase
{
    [SerializeField, Header("Serif")]
    private List<string> _messages;

    protected override IEnumerator OnAction()
    {
        for (int i = 0; i < _messages.Count; i++)
        {
            //1フレーム待機
            yield return null;

            //会話をWindowのTextフィールドに表示
            ShowMessage(_messages[i]);

            const float pause_mode = 0.0f;
            Time.timeScale = pause_mode;

            //キー入力を待機
            yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame);
        }

        yield break;
    }
}
