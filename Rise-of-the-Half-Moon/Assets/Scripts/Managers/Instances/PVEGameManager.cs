using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PVEGameManager : GameManager
{
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public int initBotLevel;
        public int seed;
    }

     protected BotLevelData initBotLevelData;

    [SerializeField] protected Bot botPrefab;
    [SerializeField] protected CardDrawer cardDrawerPrefab;

    protected Bot bot;
    protected GameInitData data;

    protected override void Start()
    {
        base.Start();

        data = ContentsDataManager.Instance.GetPVEGameInitData();
        cardDrawer = Instantiate(cardDrawerPrefab);
        Random.InitState(data.seed);

        contentType = data.contentType;
        initBotLevelData = ContentsDataManager.Instance.GetBotLevelData(data.initBotLevel);

        StartPlay();
    }

    protected void StartPlay()
    {
        InitCardList();
        InitNodeGenerator();
        InitRule((int)contentType);
        InitCam();
        
        InitCardDrawer();

        bot = Instantiate(botPrefab);
        bot.Init(initBotLevelData, otherCards);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        isMyTurn = true; // 플레이어가 먼저 시작
        cardDrawer.DrawCard(isMyTurn);

        ContentsDataManager.Instance.ClearDatas();
    }

    protected override void NextTurn(ICard removedCard)
    {
        base.NextTurn(removedCard);

        if (!isMyTurn)
            bot.StartPlaceCard();
    }
}