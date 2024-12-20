public class MoonRule : ContentRule
{
    protected override bool IsCombination(int index1, int index2)
    {
        // index1과 index2가 특정 조합을 이루는지 확인하여 true를 반환
        return (index1 == 0 && index2 == 4) || // NewMoon (0) - FullMoon (4)
               (index1 == 4 && index2 == 0) || // FullMoon (4) - NewMoon (0)
               (index1 == 1 && index2 == 5) || // WaningCrescent (1) - WaxingGibbous (5)
               (index1 == 5 && index2 == 1) || // WaxingGibbous (5) - WaningCrescent (1)
               (index1 == 2 && index2 == 6) || // ThirdQuarter (2) - FirstQuarter (6)
               (index1 == 6 && index2 == 2) || // FirstQuarter (6) - ThirdQuarter (2)
               (index1 == 3 && index2 == 7) || // WaningGibbous (3) - WaxingCrescent (7)
               (index1 == 7 && index2 == 3);   // WaxingCrescent (7) - WaningGibbous (3)
    }
}
