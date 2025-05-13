using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField, Header("SwayAmount")]        //Allows you to change the value of variables in Unity
    protected float _sway_amount;

    private Vector2 init_pos;               //初期座標
    protected static bool is_shaken;                 //揺らされた？

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform init_transform = GetComponent<Transform>();
        if (init_transform != null)
        {
            //初期座標
            init_pos = init_transform.position;
            Debug.Log("Init Start Pos" + init_pos);
        }

        is_shaken = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("result" + is_shaken);

        if (is_shaken)
        {
            TreeSway();
        }
    }

    //木が揺れる処理
    private void TreeSway()
    {
        const float t = 8.0f;
        float _amplitude = Mathf.Sin(Time.time * t);

        //変動座標を自身の一番最初の座標に加算
        this.transform.position = new Vector2(init_pos.x + _amplitude * _sway_amount, 0);
    }
}
