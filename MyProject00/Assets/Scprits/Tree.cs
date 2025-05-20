using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField, Header("SwayAmount")]          //Allows you to change the value of variables in Unity
    protected float _sway_amount;

    private Vector2 init_pos;                       //初期座標
    protected static bool is_shaken;                //揺らされた？

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //このオブジェクトの座標や大きさに関する情報を取得
        if (TryGetComponent<Transform>(out var init_transform))
        {
            //初期座標
            init_pos = init_transform.position;
        }

        is_shaken = false;
    }

    // Update is called once per frame
    void Update()
    {
        //揺らされた場合
        if (is_shaken)
        {
            //揺れる処理を実行
            TreeSway();

            //数秒後に揺れる処理が停止するように
            StartCoroutine(WaitForSpecifiedTime(2.0f));
        }
    }

    //木が揺れる処理
    private void TreeSway()
    {
        //1freamにつき動かしたい値
        const float t = 8.0f;
        //振れ幅の値
        float _amplitude = Mathf.Sin(Time.time * t);

        //変動座標を自身の一番最初の座標に加算
        this.transform.position = new Vector2(init_pos.x + _amplitude * _sway_amount, init_pos.y);
    }
    IEnumerator WaitForSpecifiedTime(float second)
    {
        //Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(second);

        //boolをfalseに変更
        is_shaken = false;

        this.transform.position = new Vector2(init_pos.x, init_pos.y);
    }
}
