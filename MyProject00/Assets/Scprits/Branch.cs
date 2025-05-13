using UnityEngine;

public class BranchHit : Tree
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //プレイヤーによって揺らされているかを受け取る処理
    public void HandleTreeShaken(bool notify)
    {
        is_shaken = notify;
    }
}