using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    [SerializeField, Header("パトロール時の移動幅")]        //Allows you to change the value of variables in Unity
    private float patrol_amplitude;
    [SerializeField, Header("パトロール時の往復速度")]      //Allows you to change the value of variables in Unity
    private float patrol_ampli_speed;
    [SerializeField, Header("プレイヤー発見用センサー")]    //Allows you to change the value of variables in Unity
    private GameObject _sensor;
    [SerializeField, Header("攻撃にかかる時間")]            //Allows you to change the value of variables in Unity
    private float _attack_duration;                         // 攻撃（半円移動）にかかる時間
    
    private Vector2 basis_pos;                //初期座標
    private Vector2 attack_start_pos;         //攻撃開始時の座標
    private Vector2 attack_target_pos;        //攻撃対象の座標
    private Vector2 attack_end_pos;           //攻撃終了時の座標
    private Vector2 velocity;                 //加速度

    private Vector2 current_offset;           //半円軌道上の攻撃開始位置からのオフセット

    private float _time;                        //時間を計測
    private SearchPlayer _search_player_script; //

    private Vector2 _distance_player;           //

    private const int _attack_power = 1;

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
        _search_player_script = _sensor.GetComponent<SearchPlayer>();
        if (_search_player_script == null)
        {
            Debug.LogError("センサーGameObjectにSearchPlayerスクリプトが見つかりませんでした", this);
        }

        velocity = new(patrol_amplitude, 0.0f);
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
                this.transform.position = attack_start_pos + current_offset;
                break;
            //攻撃後処理
            case _BirdStatus.AttackRecovery:
                BirdAttackEnd();
                //// 攻撃終了時の位置からbasis_posに戻る
                Vector2 recovery_direction = (basis_pos - (Vector2)this.transform.position).normalized;
                this.transform.position += (Vector3)recovery_direction * (patrol_ampli_speed * 2f) * Time.deltaTime;
                break;
            //ダメージを受けた際の処理
            case _BirdStatus.Damage:
                break;
            //死亡時の処理
            case _BirdStatus.Death:
                break;
        }

        Debug.Log("basis_pos " + basis_pos);
        Debug.Log("bird_location " + this.transform.position);
        Debug.Log("bird_vec " + velocity);
        Debug.Log("attack_start_pos " + attack_start_pos);
        Debug.Log("player_dis " + _distance_player.x);
    }
    private void Patrol()
    {
        //この動作に入ってからの時間を取得
        _time += Time.deltaTime;

        //水平移動の移動値
        float horizontal_move_value = Mathf.Sin(_time * patrol_ampli_speed) * patrol_amplitude;
        const float move_zero = 0.0f;

        //左右移動の値
        velocity = new (horizontal_move_value, move_zero);

        //センサーの範囲内にプレイヤーが存在する場合
        if (_search_player_script.PlayerInfo().is_find_player)
        {
            horizontal_move_value = 0.0f;
            velocity = new Vector2(0.0f, 0.0f);
            //攻撃時の座標を取得
            attack_start_pos = this.transform.position;

            //status change [Patrol] -> [AttackPrepration]
            _bird_status = _BirdStatus.AttackPreparation;
        }
    }
    private void AttackStandBy()
    {
        Debug.Log("Bird is AttackPreparation");

        //この動作に入ってからの時間を取得
        _time += Time.deltaTime;

        float dis_to_target = _search_player_script.PlayerInfo().distance_length;

        //定数宣言
        float attack_wait_time = (1.0f - (_time/ dis_to_target));
        const float time_reset = 0.0f;

        if (_time >= attack_wait_time)       //準備期間を設ける
        {
            _time = time_reset;              // Attackingに入る前にtimeをリセット
            _bird_status = _BirdStatus.Attacking;

            //攻撃対象の座標を取得
            attack_target_pos = _search_player_script.PlayerInfo().target_pos;
        }
    }
    private void BirdAttacking()
    {
        Debug.Log("Bird is Attacking");
        _time += Time.deltaTime;

        // 攻撃終了判定
        if (_time >= _attack_duration)
        {
            Debug.Log("Attack End");
            _time = 0.0f; // 次のステートのためにtimeをリセット
            attack_end_pos = this.transform.position;
            _bird_status = _BirdStatus.AttackRecovery; // 攻撃後処理へ移行
            return;
        }

        //-- 半円の軌道の計算 -- //
        //1.半径の計算(目標が左右のどちらにいるか)
        float target_direct_x = Mathf.Sign(attack_target_pos.x - attack_start_pos.x);
        //軌道の半径の大きさを取得
        float root_radius = Mathf.Abs(attack_target_pos.x - attack_start_pos.x);

        //2.半径の中心を計算
        Vector2 half_circle_center;

        //半円の中心は、目標点の座標と同じにする
        half_circle_center.x = attack_target_pos.x;
        half_circle_center.y = attack_start_pos.y;

        //3.半円軌道上の角度を計算
        float normalize_time = _time / _attack_duration;
        float angle_degree;

        const float max_angle = 360.0f;
        const float semicircular_angle = 180.0f;
        if (target_direct_x >= 0)        //プレイヤーが右にいる場合
        {
            // 0度（右）から -180度（左）へ（時計回り：下向き）
            angle_degree = Mathf.Lerp(semicircular_angle, max_angle ,normalize_time);
        }
        else                            //プレイヤーが左にいる場合
        {
            // 180度（左）から 0度（右）へ（時計回り：下向き）
            angle_degree = Mathf.Lerp(max_angle, semicircular_angle, normalize_time);
        }
        //角度をラジアン値に変更
        float change_angle_radian = angle_degree * Mathf.Deg2Rad;

        //4.半円軌道上の位置を計算(中心からのオフセット)
        Vector2 offset_center;
        float fall_depth = 5.0f;
        offset_center.x = Mathf.Cos(change_angle_radian) * root_radius;
        offset_center.y = Mathf.Sin(change_angle_radian) * fall_depth;

        //5.始点からのオフセットを計算
        current_offset = (half_circle_center - attack_start_pos) + offset_center;
    }
    private void BirdAttackEnd()
    {
        _time += Time.deltaTime;

        //初期座標に戻るための方向計算
        Vector2 direct_to_basis_pos = (basis_pos - (Vector2)this.transform.position).normalized;
        this.transform.position += (Vector3)direct_to_basis_pos * (patrol_ampli_speed * 2f) * Time.deltaTime;

        // basis_pos に十分近づいたらIdleに戻る
        if (Vector2.Distance(this.transform.position, basis_pos) < 0.05f && _time >= 1.0f)
        {
            _time = 0.0f;
            _bird_status = _BirdStatus.Idle;
        }
    }
    public void PlayerDamage(Player player)
    {
        player.Damage(_attack_power);
    }
}