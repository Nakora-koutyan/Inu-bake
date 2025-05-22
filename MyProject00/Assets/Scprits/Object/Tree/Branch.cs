using UnityEngine;

public class BranchHit : MonoBehaviour
{
    private Tree _parent_tree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _parent_tree = GetComponentInParent<Tree>();

        if (_parent_tree == null)
        {
            // Tree コンポーネントが見つからなかった場合はエラーを出す
            Debug.LogError("BranchHit requires a Tree component on its parent or an ancestor.", this);
        }
    }

    //プレイヤーによって揺らされているかを受け取る処理
    public void HandleTreeShaken(bool notify) // notify 引数は今回不要になるが、Player側の互換性のため残しても良い
    {
        // notify が true の場合のみ、親の Tree に揺らす開始を指示
        if (notify && _parent_tree != null)
        {
            // ここで親の Tree コンポーネントの StartTreeShaken() メソッドを呼び出す
            _parent_tree.StartTreeShaken();
        }
    }
}