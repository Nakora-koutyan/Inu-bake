using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    //Field: This is where you declare the variables you want to use in this class.
    [SerializeField, Header("MoveSpeed")]        //Allows you to change the value of variables in Unity
    private float _move_speed;
    [SerializeField, Header("JumpSpeed")]        //Allows you to change the value of variables in Unity
    private float _jump_speed;
    [SerializeField, Header("HitPoint")]        //Allows you to change the value of variables in Unity
    private int _hp;
    [SerializeField, Header("InvincibleTime")]        //Allows you to change the value of variables in Unity
    private float _damage_time;
    [SerializeField, Header("BlinkingTime")]        //Allows you to change the value of variables in Unity
    private float _flash_time;

    private Vector2 _input_direct;          //
    private Rigidbody2D _rigid;             //物理挙動に関するクラス
    private Animator _anim;                  //アニメーションに関するクラス型変数
    private SpriteRenderer _sprite_renderer;

    private bool is_jump;                   //ジャンプした？
    private bool is_player_shake_tree;      //揺らす処理

    private readonly float zero_f = 0.0f;
    private float _max_speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //<>で指定したコンポーネントを接続したSpriteから取ってくる処理
        _rigid = GetComponent<Rigidbody2D>();                   //RigidBody2D(物理演算)
        _anim = GetComponent<Animator>();                       //Animation(アニメーション)
        _sprite_renderer = GetComponent<SpriteRenderer>();      //SpriteRenderer(スプライト関連)

        is_jump = false;

        _max_speed = _move_speed * 2.0f;
        is_player_shake_tree = false;
    }

    // Update is called once per frame
    void Update()
    {
        //デバッグ表示処理
        Debug.Log(_hp);
    }

    void FixedUpdate() // Rigidbodyの操作は FixedUpdate で行うのが推奨
    {
        PlayerMove();
    }

    //左右移動に関する処理
    private void PlayerMove()
    {
        //目標の速度ベクトルを計算
        Vector2 target_vec = new Vector2(_input_direct.x * _move_speed, _rigid.linearVelocityY);
        //最大速度でクランプ
        target_vec = Vector2.ClampMagnitude(target_vec, _max_speed);
        // 目標速度に近づける力を加える (ForceMode.VelocityChange を使用)
        _rigid.linearVelocity = target_vec;

        //animationの切り替え(左右方向に対する加速度があればwalkに切り替える)
        _anim.SetBool("walk", _input_direct.x != 0.0f);
    }

    //左右移動の処理
    public void PlayerOnMove(InputAction.CallbackContext context)
    {
        //context内のVector2の情報をinput_directに代入して方向を取得できるようにする
        _input_direct = context.ReadValue<Vector2>();
    }

    //ジャンプ処理
    public void PlayerOnJump(InputAction.CallbackContext context)
    {
        //Jumpキーが押されていないorジャンプ状態であれば処理を終了
        if (!context.performed || is_jump)
        {
            return;
        }
        else
        {
            //ジャンプ力を乗算し、ジャンプ状態にする
            _rigid.AddForce(Vector2.up * _jump_speed, ForceMode2D.Impulse);
            is_jump = true;
            _anim.SetBool("jump", is_jump);
        }
    }

    //オブジェクトと接触した場合
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //HitしたObjectが地面ならジャンプflgを解除する
        if (collision.gameObject.CompareTag("Floor"))
        {
            is_jump = false;
            _anim.SetBool("jump", is_jump);
        }
        //if hit object's tag is Enemy -> script HitApple(object)
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Bird"))
        {
            HitEnemy(collision.gameObject);
            //プレイヤーのレイヤー情報を「ダメージを受けた状態」へと更新する
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            StartCoroutine(_Damage());
        }
        //if hit object's tag is Apple -> script HitApple(object)
        else if (collision.gameObject.CompareTag("Apple"))
        {
            HitApple(collision.gameObject);
        }
    }

    //特定のオブジェクトと接触している間発生する処理
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Object[Tree]の[Branch]の部分と接触している場合
        if(collision.gameObject.CompareTag("Branch"))
        {
            //木を揺らす
            HitBranchOfTree(collision.gameObject);
            Debug.Log("is_shaken_tree" + is_player_shake_tree);
        }
    }

    //敵と接触した際の処理
    private void HitEnemy(GameObject enemy)
    {
        //敵情報が取得できなければエラーを出力
        if (enemy == null)
        {
            throw new ArgumentNullException(nameof(enemy));
        }

        const float half = 0.5f;
        //衝突したオブジェクトと自身の大きさを変数に格納
        Vector2 player_half_scale = transform.lossyScale * half;
        Vector2 enemy_half_scale = enemy.transform.lossyScale * half;

        const float anti_penetration_offset = 0.1f;
        float player_bottom = transform.position.y - player_half_scale.y + anti_penetration_offset;
        float enemy_top = enemy.transform.position.y + enemy_half_scale.y - anti_penetration_offset;

        if(enemy_top < player_bottom && is_jump)
        {
            Destroy(enemy);
            _rigid.AddForce(Vector2.up * _jump_speed, ForceMode2D.Impulse);
            int score_num = 0;
            int rand_min_score = 12;
            int rand_max_score = 16;
            score_num = UnityEngine.Random.Range(rand_min_score, rand_max_score);
            GManager.instance.score += score_num;
        }
        else
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().PlayerDamage(this);
            }
            else if(enemy.CompareTag("Bird"))
            {
                enemy.GetComponent<Bird>().PlayerDamage(this);
            }
        }
    }

    IEnumerator _Damage()
    {
        Color color = _sprite_renderer.color;       //スプライトの色情報を取得

        //ダメージ時間の間繰り返すfor文
        for (int i = 0; i < (int)_damage_time; i++)
        {
            yield return new WaitForSeconds(_flash_time);
            const float alpha_min = 0.0f;
            _sprite_renderer.color = new Color(color.r, color.g, color.b, alpha_min);

            yield return new WaitForSeconds(_flash_time);
            const float alpha_max = 1.0f;
            _sprite_renderer.color = new Color(color.r, color.g, color.b, alpha_max);
        }
        _sprite_renderer.color = color;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void HitApple(GameObject apple)
    {
        //Hitしたオブジェクトから情報が得られなければ
        if(apple == null)
        {
            throw new ArgumentNullException(nameof(apple));
        }

        apple.GetComponent<Apple>().PlayerHit();
        Destroy(apple);
    }

    public void HitBranchOfTree(GameObject tree)
    {
        //木の情報が取得できなければエラーを出力
        if (tree == null)
        {
            throw new ArgumentNullException(nameof(tree));
        }

        //念のためLog.messageを出力
        Debug.Log("Hit Branch");

        if (is_player_shake_tree)
        {
            tree.GetComponent<BranchHit>().HandleTreeShaken(true);

            StartCoroutine(WaitForSpecifiedTime(2.0f));
        }

    }

    //数秒間後にbool型引数にfalseを返す処理
    IEnumerator WaitForSpecifiedTime(float second)
    {
        //Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(second);

        //boolをfalseに変更
        is_player_shake_tree = false;

        //Debug.Log("End Coroutine");
    }

    public void PlayerOnShakeMotion(InputAction.CallbackContext context)
    {
        //V(仮)キーが押されていないorすでに揺れている場合
        //(二回もtrueにする必要はないため)
        if (!context.performed || is_player_shake_tree)
        {
            return;
        }
        else
        {
            is_player_shake_tree = true;
        }
    }

    //ダメージを受けた際の処理
    public void Damage(int damage)
    {
        //HP減少処理(0の方が大きければ0が返ってくる)
        _hp = Mathf.Max(_hp - damage, (int)zero_f);
    }

    public int GetHp()
    {
        return _hp;
    }
}