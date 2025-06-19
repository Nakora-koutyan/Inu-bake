using Unity.VisualScripting;
using UnityEngine;

public class CameraChecker : MonoBehaviour
{
    private enum Mode
    {
        None,
        Render,
        RenderOut
    }
    private Mode _mode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mode = Mode.None;
    }

    // オブジェクトのレンダラーが任意のカメラに表示されたときに呼び出される
    void OnBecameVisible()
    {
        // 画面に入ってきたことを検知
        if (_mode != Mode.Render) // 既にRender状態でない場合のみ更新
        {
            _mode = Mode.Render;
        }
    }

    // オブジェクトのレンダラーが任意のカメラに表示されなくなったときに呼び出される
    void OnBecameInvisible()
    {
        // 画面外に出たことを検知
        if (_mode == Mode.Render) // Render状態から外に出た場合のみ更新
        {
            _mode = Mode.RenderOut;
            
            // 画面外に出て、かつ一度でも画面内に表示されたことがあれば削除
            // ここでオブジェクトを削除する
            Destroy(gameObject);
        }
    }
}
