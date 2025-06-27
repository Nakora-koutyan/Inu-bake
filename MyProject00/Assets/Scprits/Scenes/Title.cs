using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private bool start_clicked = false;
    private bool exit_clicked = false;

    //ゲーム開始関数
    public void GameStart()
    {
        Debug.Log("Game Start!");
        if(!start_clicked)
        {
            Debug.Log("GO! Next Scene");

            //シーンをGameMainに遷移
            SceneManager.LoadScene("GameMainScene");

            //連続してシーン遷移できない様にする
            start_clicked = true;
        }
    }

    public void GameExit()
    {
        Debug.Log("Game End!");
        if (!start_clicked)
        {
            Debug.Log("Quit Game!");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
            //連続してシーン遷移できない様にする
            start_clicked = true;
        }
    }
}
