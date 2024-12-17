using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchUserProfile : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] Text profileName;
    [SerializeField] Text score;

    public void SetProfile(PhotonPlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData is null");
            return;
        }

        if (profileImage != null)
        {
            profileImage.sprite = playerData.PlayerProfileSprite;
        }

        if (profileName != null)
        {
            profileName.text = playerData.PlayerName;
        }

        if (score != null)
        {
            score.text = playerData.Score.ToString();
        }
    }
}