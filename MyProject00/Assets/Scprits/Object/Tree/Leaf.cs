using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class LeafHit : Tree
{
    //Field: This is where you declare the variables you want to use in this class.
    [SerializeField, Header("RandomRadius")]        //Allows you to change the value of variables in Unity
    private float _random_radius;
    [SerializeField, Header("AppleObject")]        //Allows you to change the value of variables in Unity
    private GameObject _apple_prefab;

    private int _min_num = 1;
    private int _max_num = 3;
    private bool _is_spawn_apple;                   //Apple is Spawn Already?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ここで _apple_prefab が設定されているか確認するのも良いでしょう
        if (_apple_prefab == null)
        {
            Debug.LogError("リンゴのプレハブが設定されていません！", this);
        }
        _is_spawn_apple = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnApple()
    {
        if (!_is_spawn_apple)
        {
            int spawn_num = Random.Range(_min_num, _max_num);

            for (int i = 0; i < spawn_num; i++)
            {
                Vector2 random_offset = Random.insideUnitCircle * _random_radius;
                Vector3 spawn_pos = this.transform.position + new Vector3(random_offset.x, -0.2f, 0.0f);
                GameObject apple_instance = Instantiate(_apple_prefab, spawn_pos, Quaternion.identity);
            }
        }
    }

    public void NotifySpawnedApple(bool ret)
    {
        _is_spawn_apple = ret;
    }
}
