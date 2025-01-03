using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Camera uiCam;
    [SerializeField] TextMeshProUGUI myScore;
    [SerializeField] TextMeshProUGUI otherScore;

    [SerializeField] EndGamePanel endGamePanel;

    [SerializeField] Button settingButton;
    [SerializeField] IngameSettings ingameSettings;

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


    public Vector3 GetMyProfileWorldPosition()
    {
        return GetWorldPositionFromRectTransform(myScore.rectTransform);
    }

    public Vector3 GetOtherProfileWorldPosition()
    {
        return GetWorldPositionFromRectTransform(otherScore.rectTransform);
    }

    private Vector3 GetWorldPositionFromRectTransform(RectTransform rectTransform)
    {
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, rectTransform.position, uiCam, out Vector3 worldPosition))
        {
            worldPosition.z = 0; // Z축을 0으로 설정
            return worldPosition;
        }

        return Vector3.zero;
    }
}
