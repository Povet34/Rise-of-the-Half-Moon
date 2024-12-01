using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TextMeshProUGUI myScore;
    [SerializeField] TextMeshProUGUI otherScore;

    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button retryButton;

    private void Start()
    {
        RegistEvents();
    }

    private void RegistEvents()
    {
        retryButton.onClick.AddListener(GameManager.Instance.StartPlay);
        retryButton.onClick.AddListener(() => { ActiveResult(false); });
    }

    public void UpdateMyScore(int score)
    {
        myScore.text = score.ToString();
    }

    public void UpdateOtherScore(int score)
    {
        otherScore.text = score.ToString();
    }

    public void ShowWin()
    {
        ActiveResult(true);
        resultText.text = "Win";
    }

    public void ShowLose()
    {
        ActiveResult(true); 
        resultText.text = "Lose";
    }

    public void ShowDraw()
    {
        ActiveResult(true);
        resultText.text = "Draw";
    }

    private void ActiveResult(bool isShow)
    {
        resultPanel.gameObject.SetActive(isShow);
    }
}
