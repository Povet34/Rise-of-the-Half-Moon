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

    public const int EMPTY_NODE = 0;
    public const int NOT_OCCUPIED_NODE = 1;
    public const int MY_INDEX = 2;
    public const int OTHER_INDEX = 3;

    public const int SAME_PHASE_SCORE = 1;
    public const int PHASE_CYCLE_SCORE = 1;
    public const int COMBINATION_SCORE = 2;

    public const int SETTLEMENT_SCORE = 1;

    public const float Card_Alignment_Y = 0;

    public static readonly Vector2[] OtherTwoCardPositions = {
        new Vector2(-80, Card_Alignment_Y),
        new Vector2(80, Card_Alignment_Y)
    };
    public static readonly Vector2[] OtherThreeCardPositions = {
        new Vector2(-150, Card_Alignment_Y),
        new Vector2(0, Card_Alignment_Y),
        new Vector2(150, Card_Alignment_Y)
    };

    public static readonly Vector2[] MyTwoCardPositions = {
        new Vector2(-80, -Card_Alignment_Y),
        new Vector2(80, -Card_Alignment_Y)
    };
    public static readonly Vector2[] MyThreeCardPositions = {
        new Vector2(-150, -Card_Alignment_Y),
        new Vector2(0, -Card_Alignment_Y),
        new Vector2(150, -Card_Alignment_Y)
    };

    public static readonly Vector2 MyDrawCardSpawnPos = new Vector2(Screen.width, -Card_Alignment_Y);

    public static readonly Vector2 OhterDrawCardSpawnPos = new Vector2(Screen.width, Card_Alignment_Y);
    public static float CardMoveDuration = 1f;

    public static Color My_Occupied_Color = new Color(0.8f, 0.1f, 0.1f, 1f);
    public static Color Other_Occupied_Color = new Color(0.1f, 0.1f, 0.8f, 1f);

    public const float CamRadius = 80f;
}
