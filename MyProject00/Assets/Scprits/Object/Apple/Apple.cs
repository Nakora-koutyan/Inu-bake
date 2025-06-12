using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Apple : MonoBehaviour
{
    private const int score = 5;

    //プレイヤーに触れた場合
    public void PlayerHit()
    {
        //UIのAppleCounterへの参照がGManagerに設定されているか確認
        if (GManager.instance.apple_counter_ui != null)
        {
            //UIのAppleCounterのControlAppleCountMethodを呼び出してリンゴの数を増やす
            GManager.instance.apple_counter_ui.ControlAppleCount(true);
        }
        else
        {
            //GManagerのapple_counter_ui変数に、UIのapple_counterGameObjectが設定さてていなければ発生する警告
            Debug.LogError("GManagerの'apple_counter_ui'参照が設定されていません");
        }

        //GManagerのスコアを加算
        GManager.instance.score += score;
    }
}
