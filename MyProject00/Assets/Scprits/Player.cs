using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    //Field: This is where you declare the variables you want to use in this class.
    [SerializeField, Header("MoveSpeed")]        //Allows you to change the value of variables in Unity
    private float _move_speed;
    [SerializeField, Header("JumpSpeed")]        //Allows you to change the value of variables in Unity
    private float _jump_speed;
    [SerializeField, Header("HitPoint")]        //Allows you to change the value of variables in Unity
    private int _hp;

    private Vector2 _input_direct;          //
    private Rigidbody2D _rigid;             //物理挙動に関するクラス
    private bool is_jump;                   //ジャンプした？
    private bool is_start_shaking;             //揺らす処理

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();           //<>で指定したコンポーネントを接続したSpriteから取ってくる処理
        is_jump = false;
    }

    // Update is called once per frame
    void Update()
    {
        //移動処理
        PlayerMove();

        //デバッグ表示処理
        Debug.Log(_hp);
    }

    //左右移動に関する処理
    private void PlayerMove()
    {
        //PlayerSpriteに加えられる方向の力
        _rigid.AddForce(new Vector2(_input_direct.x * _move_speed, 0.0f));
    }

    //オブジェクトと接触した場合
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //HitしたObjectが地面ならジャンプflgを解除する
        if(collision.gameObject.CompareTag("Floor"))
        {
            is_jump = false;
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            HitEnemy(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Branch"))
        {
            Debug.Log("Hit Branch");
            TreeSway(collision.gameObject);
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

        if(enemy_top < player_bottom)
        {
            Destroy(enemy);
            _rigid.AddForce(Vector2.up * _jump_speed, ForceMode2D.Impulse);
        }
        else
        {
            enemy.GetComponent<Enemy>().PlayerDamage(this);
        }
    }

    public void TreeSway(GameObject tree)
    {
        //木の情報が取得できなければエラーを出力
        if (tree == null)
        {
            throw new ArgumentNullException(nameof(tree));
        }

        const float half = 0.5f;
        //衝突したオブジェクトと自身の大きさを変数に格納
        Vector2 player_half_scale = transform.lossyScale * half;
        Vector2 tree_half_scale = tree.transform.lossyScale * half;

        const float anti_penetration_offset = 2.5f;
        float player_left = (transform.position.x - player_half_scale.x) - anti_penetration_offset;
        float player_right = (transform.position.x + player_half_scale.x) + anti_penetration_offset;
        float tree_left = (tree.transform.position.x - tree_half_scale.x) - anti_penetration_offset;
        float tree_right = (tree.transform.position.x + tree_half_scale.x) + anti_penetration_offset;

        if (player_right > tree_left ||
            player_left < tree_right)
        {
            if (is_start_shaking)
            {
                tree.GetComponent<BranchHit>().HandleTreeShaken(true);
            }
        }
        else
        {
            Debug.Log("Failed");
        }
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
        }
    }

    public void PlayerOnShakeMotion(InputAction.CallbackContext context)
    {
        //Jumpキーが押されていないorジャンプ状態であれば処理を終了
        if (!context.performed || is_start_shaking)
        {
            return;
        }
        else
        {
            is_start_shaking = true;
        }
    }

    //ダメージを受けた際の処理
    public void Damage(int damage)
    {
        //HP減少処理(0の方が大きければ0が返ってくる)
        _hp = Mathf.Max(_hp - damage, 0);
    }
}