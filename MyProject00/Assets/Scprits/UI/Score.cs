using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI _score_text = null;
    private int _old_score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _score_text = GetComponent<TextMeshProUGUI>();
        if(_score_text == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this GameObject");
            Destroy(this);
            return;             //コンポーネントが見つからなければこれ以上処理しない
        }

        if(GManager.instance != null)
        {
            _score_text.text = "SCORE : " + GManager.instance.score;
        }
        else
        {
            Debug.Log("Game Manager is not! Maybe forgrt to put GameManager in this Scene");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_old_score != GManager.instance.score)
        {
            _score_text.text = "SCORE : " + GManager.instance.score;
            _old_score = GManager.instance.score;
        }
    }
}
