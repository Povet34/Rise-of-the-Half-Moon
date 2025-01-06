using UnityEngine;

public static class Definitions
{

    public static string GoogleWebAPI = "280826413127-uotcphrkd8rictnid8hadd0p0gn87ihl.apps.googleusercontent.com";

    #region Scene Names

    public const string LOBBY_SCENE = "Lobby";
    public const string AUTH_SCENE = "Auth";
    public const string INGAME_SCENE = "InGame";

    #endregion

    #region Contants Data Def

    public enum ContentType
    {
        Dice,
        MoonPhase,
        Number,
    }

    public const string MoonRule = "MoonRule";
    public const string DiceRule = "DiceRule";
    public const string NumberRule = "NumberRule";

    public const int DiceDataCount = 6;
    public const int MoonPhaseDataCount = 8;
    public const int NumberDataCount = 9;

    #endregion

    #region PUN Name
    
    public const string PUNCard = "PUNCard";
    public const string PUNCardDrawer = "PUNCardDrawer";
    public const string PVPGameManager = "PVPGameManager";

    #endregion

    #region Prefab Names

    public const string PREFAB_SCORE_STAR = "ScoreStar";

    #endregion

    #region Result Comments

    public const string Win = "Win";
    public const string Lose = "Lose";
    public const string Draw = "Draw";

    #endregion

    #region Sounds
    public const string SOUND_BGM1 = "BGM1";
    public const string SOUND_UI_BUTTON_CLICK = "UIButtonClick";
    public const string SOUND_OCCUR_SCORE_STAR = "OccurScoreStar";    //DM-CGS-03    
    public const string SOUND_ARRIVE_SCORE_STAR = "ArriveScoreStar";  //DM-CGS-28

    #endregion

    public const int EMPTY_NODE = 0;
    public const int NOT_OCCUPIED_NODE = 1;
    public const int MY_INDEX = 2;
    public const int OTHER_INDEX = 3;

    public const int SAME_PHASE_SCORE = 1;
    public const int PHASE_CYCLE_SCORE = 1;
    public const int COMBINATION_SCORE = 2;

    public const int SETTLEMENT_SCORE = 1;

    public const float Card_Alignment_Y = 0;

    public static readonly Vector2[] TwoCardPositions = {
        new Vector2(-80, Card_Alignment_Y),
        new Vector2(80, Card_Alignment_Y)
    };
    public static readonly Vector2[] ThreeCardPositions = {
        new Vector2(-150, Card_Alignment_Y),
        new Vector2(0, Card_Alignment_Y),
        new Vector2(150, Card_Alignment_Y)
    };
    public static readonly Vector2[] FourCardPositions = {
        new Vector2(-210, Card_Alignment_Y),
        new Vector2(-70, Card_Alignment_Y),
        new Vector2(70, Card_Alignment_Y),
        new Vector2(210, Card_Alignment_Y),
    };

    public static readonly Vector2 MyDrawCardSpawnPos = new Vector2(Screen.width, -Card_Alignment_Y);

    public static readonly Vector2 OhterDrawCardSpawnPos = new Vector2(Screen.width, Card_Alignment_Y);
    public static float CardMoveDuration = 1f;

    public static Color Non_Occupied_Color = Color.black;
    public static Color My_Occupied_Color = new Color(0.8f, 0.1f, 0.1f, 1f);
    public static Color Other_Occupied_Color = new Color(0.1f, 0.1f, 0.8f, 1f);

    public const float CamRadius = 80f;
}
