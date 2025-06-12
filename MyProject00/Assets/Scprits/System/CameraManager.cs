using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //スクロールに関する変数
    [SerializeField, Header("スクロールの速度")]        //Allows you to change the value of variables in Unity
    private float _scroll_speed;

    //ダメージエフェクト用の変数
    [SerializeField, Header("振動時間")]        //Allows you to change the value of variables in Unity
    private float _camera_shake_time;
    [SerializeField, Header("振動の大きさ")]        //Allows you to change the value of variables in Unity
    private float _shake_scale;

    private Player _player;                     //プレイヤーの情報
    private Camera main_camera;                 //メインカメラの情報を取得
    private Vector3 _init_pos;

    //ダメージエフェクトに使用する変数
    private float _shake_count;                 //現在の振動時間
    private int _current_player_hp;             //現在のPlayerのHP

    public struct CameraPos
    {
        public Vector3 _min;
        public Vector3 _max;
    }
    public CameraPos camera_pos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindFirstObjectByType<Player>();
        _current_player_hp = _player.GetHp();

        _init_pos = transform.position;

        // main_camera の初期化を追加
        // Camera.main はゲーム内でタグが "MainCamera" のカメラを探します
        main_camera = Camera.main;
        if (main_camera == null)
        {
            Debug.LogError("Main Camera not found! Please ensure your camera has the 'MainCamera' tag.");
            enabled = false; // カメラが見つからなければスクリプトを無効にする
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShakeCheck();
        ForcedScroll();
    }

    private void ForcedScroll()
    {
        float normalize_scroll_speed = 0.005f;                      //スクロール速度を矯正する値
        float scroll_x = _scroll_speed * normalize_scroll_speed;    //スクロール速度の矯正
        float update_pos = transform.position.x + scroll_x;         //カメラ座標にスクロール速度を加算
        transform.position = new Vector3(update_pos, _init_pos.y, _init_pos.z);         //座標更新
    }

    private void ShakeCheck()
    {
        if(_current_player_hp != _player.GetHp())
        {
            _current_player_hp = _player.GetHp();
            _shake_count = 0.0f;
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        Vector3 init_pos = transform.position;

        while (_shake_count < _camera_shake_time)
        {
            float x = init_pos.x + Random.Range(-_shake_scale, _shake_scale);
            float y = init_pos.y + Random.Range(-_shake_scale, _shake_scale);
            transform.position = new Vector3(x, y, init_pos.z);

            _shake_count += Time.deltaTime;

            yield return null;
        }

        transform.position = init_pos;
    }

    public CameraPos GetCameraPos()
    {
        //カメラの端の座標を取得
        Vector2 min_camera_dire = new Vector2(0, 0);
        Vector2 max_camera_dire = new Vector2(1, 1);

        //カメラの左下の座標を取得
        camera_pos._min = main_camera.ViewportToWorldPoint(new Vector3(min_camera_dire.x, min_camera_dire.y, _init_pos.z));
        //カメラの右上の座標を取得
        camera_pos._max = main_camera.ViewportToWorldPoint(new Vector3(max_camera_dire.x, max_camera_dire.y, _init_pos.z));

        return camera_pos;
    }
}
