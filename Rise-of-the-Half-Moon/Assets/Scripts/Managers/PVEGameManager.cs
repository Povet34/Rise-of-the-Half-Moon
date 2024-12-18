using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PVEGameManager : GameManager
{
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public int initBotLevel;
    }

    BotLevelData initBotLevelData;

    [SerializeField] Bot botPrefab;
    Bot bot;

    public void GameInit()
    {
        GameInitData data = ContentsDataManager.Instance.GetPVEGameInitData();
        if (data == null)
            return;

        contentType = data.contentType;
        Random.InitState(data.initBotLevel);

        phaseDatas = ContentsDataManager.Instance.GetPhaseDatas(contentType, ref rule);

        StartPlay();
    }


    private void StartPlay()
    {
        foreach (var card in otherCards)
        {
            card.Destroy();
        }

        foreach (var card in myCards)
        {
            card.Destroy();
        }

        myCards.Clear();
        otherCards.Clear();

        nodeGenerator.Create();
        rule.Init();

        cardDrawer.Init(phaseDatas, ref myCards, ref otherCards, NextTurn);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        bot = Instantiate(botPrefab);
        bot.Init(initBotLevelData, otherCards);

        isMyTurn = true; // �÷��̾ ���� ����
        cardDrawer.DrawCard(isMyTurn);
    }

    private void InitCards(int cardCount, List<ICard> cards, bool isPlayer1)
    {
        for (int i = 0; i < cardCount; i++)
        {
            cardDrawer.DrawCard(isPlayer1, false);
        }
    }

    private void NextTurn(ICard removedCard)
    {
        if (nodeGenerator.IsEndGame())
        {
            SettlementPlay();
            return;
        }

        //����Ʈ���� �����ϰ�
        if (isMyTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        //���� �ٲ��ְ�
        isMyTurn = !isMyTurn;

        //��ο��Ѵ�
        cardDrawer.DrawCard(isMyTurn);

        //���� ai ���̸�, bot�� �� �� �ֵ��� �Ѵ�.
        if (!isMyTurn)
        {
            bot.StartPlaceCard(Random.Range(1f, 4f));
        }
    }

    private void SettlementPlay()
    {
        Rule.SettlementOccupiedNodes(
            () => 
            {
                if (myScore > otherScore)
                {
                    gameUI.ShowWin();
                }
                else if (myScore < otherScore)
                {
                    gameUI.ShowLose();
                }
                else
                {
                    gameUI.ShowDraw();
                }
            });
    }

    #region Update Score

    public override void UpdateMyScore(int score)
    {
        myScore += score;
        gameUI.UpdateMyScore(myScore);
    }

    public override void UpdateOtherScore(int score)
    {
        otherScore += score;
        gameUI.UpdateOtherScore(otherScore);
    }

    #endregion
}