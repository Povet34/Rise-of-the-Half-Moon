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
    [SerializeField] MakePatternNotifier patternNotifier;

    [Header("3D Helper")]
    [SerializeField] Transform myScoreWorldTr;
    [SerializeField] Transform otherScoreWorldTr;

    public RectTransform GetMyScoreUI => myScore.rectTransform;
    public RectTransform GetOtherScoreUI => otherScore.rectTransform;

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
        endGamePanel.ShowResult(Definitions.Win);
    }

    public void ShowLose()
    {
        endGamePanel.ShowResult(Definitions.Lose);
    }

    public void ShowDraw()
    {
        endGamePanel.ShowResult(Definitions.Draw);
    }

    private void ShowSettingPanel()
    {
        ingameSettings.gameObject.SetActive(true);
    }


    public Transform GetMyProfileWorldTr()
    {
        return myScoreWorldTr;
    }

    public Transform GetOtherProfileWorldTr()
    {
        return otherScoreWorldTr;
    }

    public void ShowMakePatternNotifier(string text)
    {
        patternNotifier.ShowNotifier(text);
    }
}
