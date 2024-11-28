using UnityEngine;

public static class Definitions
{
    public static int MY_INDEX = 1;
    public static int OTHER_INDEX = 2;

    public static int SAME_PHASE_SCORE = 1;
    public static int FULL_MOON_SCORE = 2;
    public static int PHASE_CYCLE_SCORE = 1;

    public static float Card_Alignment_Y = 400f;

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
}
