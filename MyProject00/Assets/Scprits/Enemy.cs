using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Field: This is where you declare the variables you want to use in this class.
    [SerializeField, Header("MoveSpeed")]        //Allows you to change the value of variables in Unity
    private float _move_speed;
    [SerializeField, Header("AttackPower")]        //Allows you to change the value of variables in Unity
    private int _attack_power;

    private Rigidbody2D _rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMove();
    }
    
    private void EnemyMove()
    {
        _rigid.AddForce(new Vector2(Vector2.left.x * _move_speed, _rigid.linearVelocity.y));
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(_attack_power);
    }
}
