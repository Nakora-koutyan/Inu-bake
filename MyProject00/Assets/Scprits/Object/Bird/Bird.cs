using System.Diagnostics;
using UnityEngine;

public class Bird : MonoBehaviour
{
    //BirdEnemyのステータス
    private enum _BirdStatus
    {
        Idle = 0,               //待機、パトロール
        AttackPreparation,      //攻撃準備
        Attacking,              //攻撃中
        AttackRecovery,         //攻撃後処理
        Damage,                 //ダメージを受けた際の処理
        Death,                  //死亡時処理
    }

    private _BirdStatus _bird_status;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bird_status = _BirdStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_bird_status)
        {
            //待機、パトロール処理
            case _BirdStatus.Idle:
                Patrol();
                break;
            //攻撃準備処理
            case _BirdStatus.AttackPreparation:
                break;
            //攻撃中処理
            case _BirdStatus.Attacking:
                break;
            //攻撃後処理
            case _BirdStatus.AttackRecovery:
                break;
            //ダメージを受けた際の処理
            case _BirdStatus.Damage:
                break;
            //死亡時の処理
            case _BirdStatus.Death:
                break;
        }
    }

    private void Patrol()
    {

    }
}
