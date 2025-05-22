using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField, Header("SwayAmount")]          //Allows you to change the value of variables in Unity
    protected float _sway_amount;
    [SerializeField, Header("LeafObject")]        //Allows you to change the value of variables in Unity
    private GameObject _leaf;

    private Vector2 init_pos;                       //初期座標
    protected bool is_shaken;                //揺らされた
    private Coroutine _shake_coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //このオブジェクトの座標や大きさに関する情報を取得
        if (TryGetComponent<Transform>(out var init_transform))
        {
            //初期座標
            init_pos = init_transform.position;
        }
        if(_leaf == null)
        {
            Debug.LogError("not Apple", this);
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
            OnTreeShaken();
        }
    }

    //木が揺れる処理
    private void OnTreeShaken()
    {
        //1freamにつき動かしたい値
        const float t = 8.0f;
        //振れ幅の値
        float _amplitude = Mathf.Sin(Time.time * t);

        //変動座標を自身の一番最初の座標に加算
        this.transform.position = new Vector2(init_pos.x + _amplitude * _sway_amount, init_pos.y);
    }

    public void StartTreeShaken()
    {
        //連続した二度の実行を阻止
        if(is_shaken)
        {
            return;
        }

        is_shaken = true;

        if(_leaf != null)
        {
            //リンゴの生成処理
            _leaf.GetComponent<LeafHit>().SpawnApple();
            //リンゴの生成処理を開始した通知
            _leaf.GetComponent<LeafHit>().NotifySpawnedApple(true);
        }

        if(_shake_coroutine != null)
        {
            StopCoroutine(_shake_coroutine);
        }
        //数秒後に揺れる処理が停止するように
        _shake_coroutine = StartCoroutine(WaitForSpecifiedTime(2.0f));
    }

    IEnumerator WaitForSpecifiedTime(float second)
    {
        //Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(second);

        //揺れのリセットとして初期座標に戻す
        this.transform.position = init_pos;
        //boolをfalseに変更
        is_shaken = false;

        if (_leaf != null)
        {
            //生成通知を受け取っていない状態にする
            _leaf.GetComponent<LeafHit>().NotifySpawnedApple(false);
        }
    }
}
