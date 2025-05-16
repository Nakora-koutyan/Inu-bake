using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField, Header("PlayerIcon")]        //Allows you to change the value of variables in Unity
    private GameObject _player_icon;

    private Player _player;
    private int _before_hp;

    void Start()
    {
        //プレイヤーの情報を取得
        _player = FindFirstObjectByType<Player>();
        _before_hp = _player.GetHp();
        CreateHPICon();
    }

    private void CreateHPICon()
    {
        for (int i = 0; i < _player.GetHp(); i++)
        {
            GameObject _player_hp_obj = Instantiate(_player_icon);
            _player_hp_obj.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowIcon();
    }

    private void ShowIcon()
    {
        //PlayerのHPが1フレーム前と同じであれば処理を行う必要はないためスルー
        if(_before_hp == _player.GetHp())
        {
            return;
        }

        Image[] icons = transform.GetComponentsInChildren<Image>();
        for(int i=0;i<icons.Length;i++)
        {
            bool ret;
            ret = (i < _player.GetHp());

            icons[i].gameObject.SetActive(ret);
        }

        _before_hp = _player.GetHp();
    }
}