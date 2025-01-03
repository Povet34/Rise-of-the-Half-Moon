using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

public class ScoreStar : MonoBehaviour
{
    [SerializeField] SpriteRenderer bigStarRenderer;

    public void DoEffect(bool isMine, Vector3 targetPos)
    {
        SetColor(isMine);
        SetMoveSequene(targetPos);
    }

    private void SetMoveSequene(Vector3 targetPos) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * 3.0f;
        randomDirection.z = 0;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(transform.position + randomDirection, 0.3f).SetEase(Ease.OutBounce));
        sequence.Append(transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => { Destroy(gameObject); });
        sequence.Play();
    }

    private void SetColor(bool isMine)
    {
        bigStarRenderer.color = isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color;
    }
}
