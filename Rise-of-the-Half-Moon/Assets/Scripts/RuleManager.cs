using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    public int OnCardPlaced(Node node)
    {
        int totalScore = 0;

        totalScore += CheckAdjacentNodes(node);
        totalScore += CheckFullMoon(node);
        totalScore += CheckMoonCycle(node);

        return totalScore;
    }

    private int CheckAdjacentNodes(Node node)
    {
        int score = 0;
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                Debug.Log("Adjacent node has the same type.");
                score += Definitions.SAME_PHASE_SCORE;
            }
        }

        return score;
    }

    private int CheckFullMoon(Node node)
    {
        int score = 0;
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && IsFullMoonCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                Debug.Log("Adjacent node can form a full moon.");
                score += Definitions.FULL_MOON_SCORE;
            }
        }

        return score;
    }

    private bool IsFullMoonCombination(MoonPhaseData.PhaseType type1, MoonPhaseData.PhaseType type2)
    {
        // Implement the logic to check if the combination of type1 and type2 can form a full moon
        return (type1 == MoonPhaseData.PhaseType.NewMoon && type2 == MoonPhaseData.PhaseType.FullMoon) ||
               (type1 == MoonPhaseData.PhaseType.FullMoon && type2 == MoonPhaseData.PhaseType.NewMoon) ||
               (type1 == MoonPhaseData.PhaseType.WaningCrescent && type2 == MoonPhaseData.PhaseType.WaxingGibbous) ||
               (type1 == MoonPhaseData.PhaseType.WaxingGibbous && type2 == MoonPhaseData.PhaseType.WaningCrescent) ||
               (type1 == MoonPhaseData.PhaseType.ThirdQuarter && type2 == MoonPhaseData.PhaseType.FirstQuarter) ||
               (type1 == MoonPhaseData.PhaseType.FirstQuarter && type2 == MoonPhaseData.PhaseType.ThirdQuarter) ||
               (type1 == MoonPhaseData.PhaseType.WaningGibbous && type2 == MoonPhaseData.PhaseType.WaxingCrescent) ||
               (type1 == MoonPhaseData.PhaseType.WaxingCrescent && type2 == MoonPhaseData.PhaseType.WaningGibbous);
    }

    private int CheckMoonCycle(Node node)
    {
        int score = 0;

        ColorNodesByPhase(node);

        List <Node> cycleNodes = new List<Node>();
        if (IsMoonCycle(node, cycleNodes))
        {
            Debug.Log("The placed node forms a moon cycle.");
        }
        return score;
    }

    private bool IsMoonCycle(Node node, List<Node> cycleNodes)
    {
        return false;
    }

    public void ColorNodesByPhase(Node startNode)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>(); // To track visited nodes and prevent revisiting

        queue.Enqueue(startNode);
        visited.Add(startNode); // Mark the start node as visited

        bool isDecreasing = false;
        bool isIncreasing = false;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            MoonPhaseData.PhaseType currentPhase = currentNode.GetPhaseType();

            foreach (Node neighbor in currentNode.GetAdjacentNodes())
            {
                if (!visited.Contains(neighbor))
                {
                    MoonPhaseData.PhaseType neighborPhase = neighbor.GetPhaseType();

                    if (!isDecreasing && !isIncreasing)
                    {
                        if (MoonPhaseData.GetPreviousPhaseType(currentPhase) == neighborPhase)
                        {
                            isDecreasing = true;
                        }
                        else if (MoonPhaseData.GetNextPhaseType(currentPhase) == neighborPhase)
                        {
                            isIncreasing = true;
                        }
                    }

                    if (isDecreasing && MoonPhaseData.GetPreviousPhaseType(currentPhase) == neighborPhase)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor); // Mark as visited
                        neighbor.ChangeColor(Color.blue); // Change color to indicate phase
                    }
                    else if (isIncreasing && MoonPhaseData.GetNextPhaseType(currentPhase) == neighborPhase)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor); // Mark as visited
                        neighbor.ChangeColor(Color.green); // Change color to indicate phase
                    }
                }
            }
        }
    }
}
