using DG.Tweening;
public class NumberRule : ContentRule
{
    protected override bool IsCombination(int index1, int index2)
    {
        return (index1 + index2) == 10;
    }
}
