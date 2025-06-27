using UnityEngine;

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
    
    private Vector2 basis_pos;                  //初期座標
    private Vector2 attack_start_pos;           //攻撃開始時の座標
    private Vector2 attack_target_pos;          //攻撃対象の座標
    private Vector2 velocity;                   //加速度

    private Vector2 current_offset;             //半円軌道上の攻撃開始位置からのオフセット

    private float _time;                        //時間を計測
    const float _reset_value = 0.0f;            //リセット値

    private SearchPlayer _p_info;        //プレイヤーを見つけた際のプレイヤーの情報取得用
    private float _fall_depth;                  //下降する深さ
    private Quaternion init_rotation;           //初期回転情報

    private Animator _anim;                     //アニメーション用
    private bool is_attack;                     //攻撃する？

    //BirdEnemyのステータス
    private enum _BirdStatus
    {
        Patrol = 0,             //待機、パトロール
        AttackPreparation,      //攻撃準備
        Attacking,              //攻撃中
        AttackRecovery,         //攻撃後処理
    }
    private _BirdStatus _bird_status;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bird_status = _BirdStatus.Patrol;

        if (TryGetComponent<Transform>(out var init_transform))
        {
            //初期座標
            basis_pos = init_transform.position;
        }
        if(_sensor == null)
        {
            Debug.LogError("センサー情報が見つかりませんでした");
        }
        _p_info = _sensor.GetComponent<SearchPlayer>();
        if (_p_info == null)
        {
            Debug.LogError("センサーGameObjectにSearchPlayerスクリプトが見つかりませんでした", this);
        }
        _anim = GetComponent<Animator>();                       //Animation(アニメーション)
        velocity = new(patrol_amplitude, _reset_value);
        _time = _reset_value;
        _attack_duration = 1.0f;
        _fall_depth = _reset_value;
        init_rotation = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 old_pos = transform.position;               //前回の座標を取得
        Vector2 now_pos = transform.position;               //新しい座標を取得
        switch (_bird_status)
        {
            //待機、パトロール処理
            case _BirdStatus.Patrol:
                Patrol();
                transform.position = basis_pos + velocity;  //初期座標に移動量を更新
                now_pos = transform.position;
                break;
            //攻撃準備処理
            case _BirdStatus.AttackPreparation:
                AttackStandBy();
                transform.position = attack_start_pos + velocity;
                float _look_target_direct_x =               //自身とtargetの座標をもとにプレイヤーの方向を取得
                  _p_info.PosInfo().target_pos.x - transform.position.x;
                now_pos = transform.position;
                break;
            //攻撃中処理
            case _BirdStatus.Attacking:
                BirdAttacking();
                transform.position = attack_start_pos + current_offset;
                now_pos = transform.position;
                break;
            //攻撃後処理
            case _BirdStatus.AttackRecovery:
                BirdAttackEnd();
                //攻撃終了時の位置からbasis_posに戻る
                Vector2 recovery_direction = (basis_pos - (Vector2)transform.position).normalized;
                transform.position += (Vector3)recovery_direction * (patrol_ampli_speed * 2f) * Time.deltaTime;
                now_pos = transform.position;
                break;
        }

        _LookMoveDirect(IsMovedForward(old_pos.x, now_pos.x));                   //velocityのXをもとに方向を転換する
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
        if (_p_info.PosInfo().is_find_player)
        {
            horizontal_move_value = _reset_value;
            velocity = new Vector2(move_zero, move_zero);
            //攻撃時の座標を取得
            attack_start_pos = transform.position;

            //status change [Patrol] -> [AttackPrepration]
            _bird_status = _BirdStatus.AttackPreparation;
        }
    }
    private void AttackStandBy()
    {
        //この動作に入ってからの時間を取得
        _time += Time.deltaTime;
        //攻撃準備用のアニメーションに変更
        _anim.SetBool("find_player", _p_info.PosInfo().is_find_player);

        //プレイヤーとの距離の長さを取得
        float dis_to_target = _p_info.PosInfo().distance.magnitude;
        //定数宣言
        const float max_wait_time = 1.0f;
        float attack_wait_time = (max_wait_time - (_time / dis_to_target));
       
        if (_time >= attack_wait_time)       //準備期間を設ける
        {
            _time = _reset_value;            // Attackingに入る前にtimeをリセット
            _fall_depth = Mathf.Abs(_p_info.PosInfo().distance.y);

            //攻撃対象の座標を取得
            attack_target_pos = _p_info.PosInfo().target_pos;
            _bird_status = _BirdStatus.Attacking;
        }
    }
    private void BirdAttacking()
    {
        _time += Time.deltaTime;                //攻撃に入ってからの時間を計測

        is_attack = true;

        _anim.SetBool("is_attack", is_attack);

        //-- 半円の軌道の計算 -- //
        //1.半径の計算(目標が左右のどちらにいるか)
        float target_direct_x = Mathf.Sign(attack_target_pos.x - attack_start_pos.x);
        //軌道の半径の大きさを取得
        float root_radius = Mathf.Abs(attack_target_pos.x - attack_start_pos.x);

        //2.半径の中心を計算
        Vector2 half_circle_center;

        //半円の中心座標
        half_circle_center.x = attack_target_pos.x;
        half_circle_center.y = attack_start_pos.y;

        //3.半円軌道上の角度を計算
        float normalize_time = _time / _attack_duration;        //攻撃開始してからの時間を攻撃にかける時間で除算
        float angle_degree;                                     //攻撃の角度

        const float max_angle = 360.0f;
        const float semicircular_angle = 180.0f;
        const float direct_center = 0.0f;
        if (target_direct_x <= direct_center)      //プレイヤーが右にいる場合
        {
            // 180度（左）から 360度（右）へ（時計回り：下向き）
            angle_degree = Mathf.Lerp(max_angle, semicircular_angle, normalize_time);
        }
        else                            //プレイヤーが左にいる場合
        {
            // 360度（右）から 180度（左）へ（時計回り：下向き）
            angle_degree = Mathf.Lerp(semicircular_angle, max_angle, normalize_time);
        }
        //角度をラジアン値に変更
        float change_angle_radian = angle_degree * Mathf.Deg2Rad;

        //4.半円軌道上の位置を計算(中心からのオフセット)
        const float no_rotate = 0.0f;
        Vector2 offset_center;
        offset_center.x = Mathf.Cos(change_angle_radian) * root_radius;
        offset_center.y = Mathf.Sin(change_angle_radian) * _fall_depth;

        //Z軸の回転(うまくいかない)
        //transform.Rotate(no_rotate, no_rotate, angle_degree);
        transform.eulerAngles = new Vector3(no_rotate, no_rotate, angle_degree);

        //5.始点からのオフセットを計算
        current_offset = (half_circle_center - attack_start_pos) + offset_center;

        // 攻撃終了判定
        if (_time >= _attack_duration)
        {
            _time = _reset_value;                           // 次のステートのためにtimeをリセット
            _bird_status = _BirdStatus.AttackRecovery;      // 攻撃後処理へ移行
            transform.rotation = init_rotation;             // 初期回転状態に移行
            is_attack = false;
            _anim.SetBool("is_attack", is_attack);
            return;
        }
    }
    private void BirdAttackEnd()
    {
        _time += Time.deltaTime;

        //初期座標に戻るための方向計算
        Vector2 direct_to_basis_pos = (basis_pos - (Vector2)transform.position).normalized;
        transform.position += (Vector3)direct_to_basis_pos * (patrol_ampli_speed * 2f) * Time.deltaTime;

        // basis_pos に十分近づいたらIdleに戻る
        const float basis_pos_error_tolerance = 0.05f;
        const float attack_end_time_limit = 1.0f;
        if (Vector2.Distance(transform.position, basis_pos) < basis_pos_error_tolerance && _time >= attack_end_time_limit)
        {
            _time = _reset_value;
            _bird_status = _BirdStatus.Patrol;
        }
    }

    //移動方向を取得
    private bool IsMovedForward(float old_vec,float now_vec)
    {
        bool ret = false;
        if(old_vec < now_vec)
        {
            return true;
        }
        else if(old_vec > now_vec)
        {
            return false;
        }

        return ret;
    }

    //移動方向に沿って左右反転
    private void _LookMoveDirect(bool is_facing_right)
    {
        const float rotate_left = 180.0f;   //左を向くためのY軸回転 (180度)
        const float rotate_right = 0.0f;    //右を向くためのY軸回転 (0度)
        const float no_rorate = 0.0f;       //回転なし

        if (is_facing_right)        //右移動
        {
            // 左を向く回転に設定
            transform.rotation = Quaternion.Euler(no_rorate, rotate_left, no_rorate);
        }
        else if(!is_facing_right)    //左移動
        {
            // 右を向く回転に設定
            transform.rotation = Quaternion.Euler(no_rorate, rotate_right, no_rorate);
        }
    }

    public void PlayerDamage(Player player)
    {
        const int _attack_power = 1;
        player.Damage(_attack_power);
    }

    private bool IsCamera()
    {
        bool ret = false;
        return ret;
    }
}