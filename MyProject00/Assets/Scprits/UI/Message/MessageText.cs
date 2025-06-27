using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;

public class MessageText : FieldMessageBase
{
    [SerializeField, Header("MainSerif")]       //最初に表示されるメッセージ
    private List<string> _default_messages;
    [SerializeField, Header("MinGoalSerif")]    //条件分岐メッセージ1
    private List<string> _level1_messages;
    [SerializeField, Header("MiddleGoalSerif")] //条件分岐メッセージ2
    private List<string> _level2_messages;
    [SerializeField, Header("MaxGoalSerif")]    //条件分岐メッセージ3
    private List<string> _level3_messages;

    [SerializeField] PlayerInput _player_input;   // インスペクタで player をドラッグ

    [SerializeField, Header("表示速度")]
    [Range(0.01f, 0.5f)]                        // 0.01秒から0.5秒の範囲で調整可能
    private float _text_speed = 0.0f;           // 一文字あたりの表示速度 (例: 0.05秒)

    [SerializeField, Header("TotalAppleCount")]
    private AppleCounter _apple;                // 獲得したリンゴの数
    [SerializeField, Header("GoalCount")]
    private int threshold_count;                // 目標となるリンゴの数
    [SerializeField, Header("BaseLineGoal")]
    private int baseline_count;                 // 最低目標数

    private List<string> _current_messages;     // 現在表示するメッセージ

    private enum CollectRank
    {
        Beginner = 0,
        Intermediate,
        Advanced,
    }
    private CollectRank collect_rank;

    private void DesideRank()
    {
        int total_apples = _apple.GetTotalAppleCount();
        if (total_apples >= threshold_count)
        {
            collect_rank = CollectRank.Advanced;
        }
        else if (total_apples >= baseline_count) // threshold_count 未満で baseline_count 以上
        {
            collect_rank = CollectRank.Intermediate;
        }
        else // baseline_count 未満
        {
            collect_rank = CollectRank.Beginner;
        }
    }

    //表示するメッセージを設定
    protected override void DesideMessage()
    {
        DesideRank();

        List<string> branch_messages = null;
        switch (collect_rank)
        {
            case CollectRank.Beginner:
                branch_messages = _level1_messages;
                Debug.Log("Rank1");
                break;

            case CollectRank.Intermediate:
                branch_messages = _level2_messages;
                Debug.Log("Rank2");
                break;

            case CollectRank.Advanced:
                branch_messages = _level3_messages;
                Debug.Log("Rank3");
                break;

            default:
                Debug.Log("別の値が入っています");
                break;
        }

        // デフォルトメッセージと分岐メッセージを結合
        _current_messages = new List<string>();
        if (_default_messages != null)
        {
            _current_messages.AddRange(_default_messages);
        }
        //ブランチのメッセージを取得
        if (branch_messages != null)
        {
            _current_messages.AddRange(branch_messages);
        }
    }

    protected override IEnumerator OnAction()
    {
        //Player入力操作Lock
        var input_handler = _player_input.GetComponent<PlayerInputHandler>();
        if(input_handler != null)
        {
            input_handler.PlayerInputLock(true);
        }
        // UI へ
        _player_input.SwitchCurrentActionMap("UI");

        DesideMessage();

        for (int i = 0; i < _current_messages.Count; i++)
        {
            Debug.Log("Now Input Mode : " + _player_input.currentActionMap.name);

            // 会話をWindowのTextフィールドに一文字ずつ表示
            // ShowMessageがコルーチンを返すようになったので、yield returnで待機
            yield return ShowMessageCoroutine(_current_messages[i]);

            //指定されたキーが押された場合、次のメッセージを表示
            yield return new WaitUntil(() => input_handler.ClickLicence());
        }

        // 元へ戻す
        _player_input.SwitchCurrentActionMap("Player");
        input_handler.PlayerInputLock(false);

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
            yield return new WaitForSecondsRealtime(_text_speed); // 指定された速度で待機
            //WaitForSecondsはTimeScaleの影響を受けるためここではWaitForSecondsRealTimeを使用
        }

        // 全ての文字が表示された後、念のため再度、完全に表示された状態にする（タイプミス防止）
        target.text = message;
    }
}