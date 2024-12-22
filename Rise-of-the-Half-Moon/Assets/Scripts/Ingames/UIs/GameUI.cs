using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myScore;
    [SerializeField] TextMeshProUGUI otherScore;

    [SerializeField] EndGamePanel endGamePanel;

    [SerializeField] Button settingButton;
    [SerializeField] IngameSettings ingameSettings;

    private void Start()
    {
        RegistEvents();
    }

    private void RegistEvents()
    {
        settingButton.onClick.AddListener(ShowSettingPanel);
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
        endGamePanel.ShowResult("Win");
    }

    public void ShowLose()
    {
        endGamePanel.ShowResult("Lose");
    }

    public void ShowDraw()
    {
        endGamePanel.ShowResult("Draw");
    }

    private void ShowSettingPanel()
    {
        ingameSettings.gameObject.SetActive(true);
    }
}
