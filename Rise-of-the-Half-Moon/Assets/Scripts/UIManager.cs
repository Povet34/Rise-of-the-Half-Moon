using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TextMeshProUGUI myScore;
    [SerializeField] TextMeshProUGUI otherScore;

    public void UpdateMyScore(int score)
    {
        myScore.text = score.ToString();
    }

    public void UpdateOtherScore(int score)
    {
        otherScore.text = score.ToString();
    }
}
