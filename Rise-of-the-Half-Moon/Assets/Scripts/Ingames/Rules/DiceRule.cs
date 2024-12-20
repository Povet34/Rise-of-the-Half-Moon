public class DiceRule : ContentRule
{
    protected override bool IsCombination(int index1, int index2)
    {
        // index1과 index2가 특정 조합을 이루는지 확인하여 true를 반환
        return (index1 == 0 && index2 == 5) ||
               (index1 == 1 && index2 == 4) ||
               (index1 == 2 && index2 == 3) ||
               (index1 == 3 && index2 == 2) ||
               (index1 == 4 && index2 == 1) ||
               (index1 == 5 && index2 == 0);  
    }
}