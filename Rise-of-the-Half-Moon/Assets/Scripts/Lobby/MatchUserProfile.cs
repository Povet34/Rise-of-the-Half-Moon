using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchUserProfile : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] Text profileName;
    [SerializeField] TextMeshProUGUI score;

    public void SetProfile(Sprite image, string name, int score)
    {
        profileImage.sprite = image;
        profileName.text = name;
        this.score.text = score.ToString();
    }

    public void SetProfile(PhotonPlayerData playerData)
    {
        profileImage.sprite = playerData.GetPlayerProfileSprite();
        profileName.text = playerData.GetPlayerName();
        score.text = playerData.GetScore().ToString();
    }
}