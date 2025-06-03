using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    [SerializeField, Header("移動幅")]        //Allows you to change the value of variables in Unity
    private float _amplitude;
    [SerializeField, Header("往復速度")]        //Allows you to change the value of variables in Unity
    private float _ampli_speed;
    [SerializeField, Header("プレイヤー発見用センサー")]        //Allows you to change the value of variables in Unity
    private GameObject _sensor;
    [SerializeField, Header("攻撃にかかる時間")]        //Allows you to change the value of variables in Unity
    private float _attack_duration;           // 攻撃（半円移動）にかかる時間

    private Vector2 basis_pos;                //初期座標
    private Vector2 attack_start_pos;         //攻撃開始時の座標
    private Vector2 velocity;                 //加速度

    private float _time;                       //時間を計測

    private Vector2 _distance_player;
    private float _censor_radius;

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

        if (TryGetComponent<Transform>(out var init_transform))
        {
            //初期座標
            basis_pos = init_transform.position;
        }
        if(_sensor == null)
        {
            Debug.LogError("センサー情報が見つかりませんでした");
        }

        velocity = new(_amplitude, 0.0f);
        _time = 0.0f;
        _attack_duration = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_bird_status)
        {
            //待機、パトロール処理
            case _BirdStatus.Idle:
                Patrol();
                this.transform.position = basis_pos + velocity;
                break;
            //攻撃準備処理
            case _BirdStatus.AttackPreparation:
                AttackStandBy();
                this.transform.position = attack_start_pos + velocity;
                break;
            //攻撃中処理
            case _BirdStatus.Attacking:
                BirdAttacking();
                this.transform.position = attack_start_pos + velocity;
                break;
            //攻撃後処理
            case _BirdStatus.AttackRecovery:
                // 回復中は、攻撃終了時の位置からbasis_posに戻る
                this.transform.position = basis_pos + velocity;
                break;
            //ダメージを受けた際の処理
            case _BirdStatus.Damage:
                break;
            //死亡時の処理
            case _BirdStatus.Death:
                break;
        }

        this.transform.position = basis_pos + velocity;
        Debug.Log("basis_pos " + basis_pos);
        Debug.Log("bird_location " + this.transform.position);
        Debug.Log("bird_vec " + velocity);
        Debug.Log("attack_start_pos " + attack_start_pos);
    }

    private void Patrol()
    {
        float horizontal_move_value = Mathf.Sin(Time.time * _ampli_speed) * _amplitude;
        const float move_zero = 0.0f;

        //左右移動の値
        velocity = new (horizontal_move_value, move_zero);

        //センサーの範囲内にプレイヤーが存在する場合
        if (_sensor.GetComponent<SearchPlayer>().FindPlayer().is_find_player)
        {
            //攻撃時の座標を取得
            attack_start_pos = this.transform.position;

            //status change [Patrol] -> [AttackPrepration]
            _bird_status = _BirdStatus.AttackPreparation;
            _distance_player = _sensor.GetComponent<SearchPlayer>().FindPlayer().distance;
            _censor_radius = _sensor.GetComponent<SearchPlayer>().FindPlayer().collider_radius;
        }
    }
    private void AttackStandBy()
    {
        Debug.Log("Bird is AttackPreparation");

        //この動作に入ってからの時間を取得
        _time += Time.deltaTime;

        //定数宣言
        const float attack_wait_time = 0.5f;
        const float time_reset = 0.0f;

        if (_time >= attack_wait_time)       //準備期間を設ける
        {
            _time = time_reset;              // Attackingに入る前にtimeをリセット
            _bird_status = _BirdStatus.Attacking;
        }
    }

    private void BirdAttacking()
    {
        Debug.Log("Bird is Attacking");

        _time += Time.deltaTime;

        //急降下処理
        const float one_half = 0.5f;
        const float max_angle = 360.0f;

        //値の正規化
        //経過時間を0.0～1.0の値で正規化
        float normalize_time = (_time / _attack_duration);

        //時間経過と共に角度を180～360の間で滑らかに変化させる
        float angle_degrees = Mathf.Lerp((max_angle), (max_angle * one_half),normalize_time);

        //ラジアン値に変換
        float angle_change_radian = angle_degrees * Mathf.Deg2Rad;

        velocity.x = Mathf.Cos(angle_change_radian) * _distance_player.x;
        velocity.y = Mathf.Sin(angle_change_radian) * _censor_radius;

        // 攻撃終了判定
        if (_time >= _attack_duration)
        {
            Debug.Log("Attack End");
            _time = 0.0f; // 次のステートのためにtimeをリセット
            _bird_status = _BirdStatus.AttackRecovery; // 攻撃後処理へ移行
            return;
        }
    }
}