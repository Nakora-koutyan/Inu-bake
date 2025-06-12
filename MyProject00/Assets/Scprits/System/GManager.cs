using UnityEngine;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;
    public int score = 0;
    public int total_get_apple = 0;

    public AppleCounter apple_counter_ui;

    private void Awake()
    {
        //ゲームマネージャーをシングルトン化
        //instanceがなければ
        if(instance == null)
        {
            //instanceを生成
            instance = this;
            //シーンが切り替わっても破棄されなくなる
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
