using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    public struct SpecialRelation
    {
        public Vector2 distance;
        public bool is_find_player;

        public void Initialize()
        {
            distance = new (0.0f,0.0f);
            is_find_player = false;
        }
    }
    SpecialRelation distance_info;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        distance_info.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //プレイヤーと接触した場合、親クラスのBirdに通知を送信
        if(collision.gameObject.CompareTag("Player"))
        {
            distance_info.is_find_player = true;
            distance_info.distance.x = this.transform.position.x - collision.transform.position.x;

        }
    }

    public SpecialRelation FindPlayer()
    {
        return distance_info;
    }
}
