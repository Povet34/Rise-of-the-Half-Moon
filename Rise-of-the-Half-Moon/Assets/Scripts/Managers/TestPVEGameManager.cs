using Random = UnityEngine.Random;

public class TestPVEGameManager : PVEGameManager
{
    protected override void Start()
    {
        base.Start();

        data = ContentsDataManager.Instance.GetTestData();
        cardDrawer = Instantiate(cardDrawerPrefab);
        Random.InitState(data.seed);

        contentType = data.contentType;
        initBotLevelData = ContentsDataManager.Instance.GetBotLevelData(data.initBotLevel);

        StartPlay();
    }

    protected override void NextTurn(ICard removedCard)
    {
        if (nodeGenerator.IsEndGame())
        {
            SettlementPlay();
            return;
        }

        //리스트에서 제거하고
        if (isMyTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        isMyTurn = true;
        cardDrawer.DrawCard(isMyTurn);
    }
}
