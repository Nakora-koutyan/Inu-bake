using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [SerializeField, Header("EnemyObject")]
    private GameObject _bird;

    private Player _player;
    private GameObject _bird_obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindFirstObjectByType<Player>();
        _bird_obj = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            return;
        }

        Vector3 p_pos = _player.transform.position;
        Vector3 c_max_pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        Vector3 scale = _bird.transform.lossyScale;

        float distance = Vector2.Distance(transform.position, new Vector2(p_pos.x, transform.position.y));
        float spawn_dis = Vector2.Distance(p_pos, new Vector2((c_max_pos.x + (scale.x / 2.0f)), p_pos.y));
        if (distance <= spawn_dis && _bird_obj == null)
        {
            _bird_obj = Instantiate(_bird);
            _bird_obj.transform.position = transform.position;
            transform.parent = _bird_obj.transform;
        }
    }
}
