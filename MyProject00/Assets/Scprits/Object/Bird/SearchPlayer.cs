using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    private bool is_find_player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        is_find_player = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //プレイヤーと接触した場合、親クラスのBirdに通知を送信
        if(collision.gameObject.CompareTag("Player"))
        {
            is_find_player = true;
        }
    }

    public bool FindPlayer()
    {
        return is_find_player;
    }
}
