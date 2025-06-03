using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    public struct SpecialRelation
    {
        public Vector2 distance;
        public bool is_find_player;
        public float collider_radius;

        public void Initialize()
        {
            distance = new (0.0f,0.0f);
            is_find_player = false;
            collider_radius = 0.0f;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //プレイヤーがこのセンサーに触れている場合
    private void OnTriggerStay2D(Collider2D collision)
    {
        //接触した時の座標などの情報を更新
        if(collision.gameObject.CompareTag("Player"))
        {
            collision_info.is_find_player = true;
            collision_info.distance.x = this.transform.position.x - collision.transform.position.x;
        }
    }

    public SpecialRelation FindPlayer()
    {
        return collision_info;
    }
}
