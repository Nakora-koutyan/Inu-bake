using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    public struct SpecialRelation
    {
        public Vector2 distance;
        public bool is_find_player;
        public float collider_radius;
        public Vector2 target_pos;
        public float distance_length;

        public void Initialize()
        {
            distance = new (0.0f,0.0f);
            is_find_player = false;
            collider_radius = 0.0f;
            target_pos = new (0.0f,0.0f);
            distance_length = 0.0f;
        }
    }
    SpecialRelation collision_info;
    private CircleCollider2D _my_collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //衝突情報の初期化
        collision_info.Initialize();
        //自身のcolliderの情報を取得
        _my_collider = GetComponent<CircleCollider2D>();
        if (_my_collider == null)
        {
            //circle_colliderの情報が取得できない場合はエラーを出力
            Debug.LogError("collider情報がありませんでした");
        }
        else
        {
            //取得できた場合は半径を取得する
            collision_info.collider_radius = _my_collider.radius;
        }
    }

    //プレイヤーがこのセンサーに触れている場合
    private void OnTriggerStay2D(Collider2D collision)
    {
        //接触した時の座標などの情報を更新
        if(collision.gameObject.CompareTag("Player"))
        {
            collision_info.is_find_player = true;
            collision_info.distance.x = collision.transform.position.x - this.transform.position.x;
            collision_info.distance.y = collision.transform.position.y - this.transform.position.y;
            collision_info.target_pos = collision.transform.position;
            collision_info.distance_length = Vector2.Distance(collision_info.distance,collision.transform.position);
        }
    }

    //プレイヤーがセンサーの範囲内にいなかった場合
    private void OnTriggerExit2D(Collider2D collision)
    {
        //接触した時の座標などの情報を更新
        if (collision.gameObject.CompareTag("Player"))
        {
            collision_info.is_find_player = false;
            collision_info.distance.x = 0.0f;
            collision_info.target_pos = new Vector2(0.0f, 0.0f);
        }
    }

    public SpecialRelation PlayerInfo()
    {
        return collision_info;
    }
}
