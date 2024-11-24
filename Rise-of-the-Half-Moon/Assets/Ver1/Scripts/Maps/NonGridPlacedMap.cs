using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonGridPlacedMap : MonoBehaviour
{
    public int nodeCount = 10;        // 노드 개수
    public float nodeSize = 50f;      // 노드 크기
    public float minDistance = 20f;   // 노드간 최소 거리

    private List<Rect> placedNodeRects = new List<Rect>();
    private List<GameObject> nodes = new List<GameObject>();

    private System.Random random;
    private RectTransform boundingBox;
    private GameObject nodePrefab;

    public void CreateMap(int seed, GameObject nodePrefab, RectTransform boundingBox)
    {
        random = new System.Random(seed);
        this.nodePrefab = nodePrefab;
        this.boundingBox = boundingBox;

        GenerateNodes(nodeCount);
        ConnectNodes(nodes);
    }

    void GenerateNodes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 position;
            int attempts = 0;
            do
            {
                position = GetRandomPosition();
                attempts++;
            } while (IsOverlapping(position) && attempts < 100); // 최대 100번 시도

            if (attempts >= 100)
            {
                Debug.LogWarning("더 이상 겹치지 않는 위치를 찾을 수 없습니다.");
                continue;
            }

            GameObject newNode = Instantiate(nodePrefab, boundingBox);
            RectTransform rectTransform = newNode.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(nodeSize, nodeSize); // 노드 크기 설정

            // 배치된 노드의 Rect 저장
            placedNodeRects.Add(new Rect(position.x - nodeSize / 2, position.y - nodeSize / 2, nodeSize, nodeSize));
            newNode.name = $"Node {i}";

            nodes.Add(newNode);
        }
    }

    Vector2 GetRandomPosition()
    {
        float x = (float)(random.NextDouble() * boundingBox.rect.width - boundingBox.rect.width / 2);
        float y = (float)(random.NextDouble() * boundingBox.rect.height - boundingBox.rect.height / 2);
        return new Vector2(x, y);
    }

    bool IsOverlapping(Vector2 position)
    {
        Rect newNodeRect = new Rect(position.x - nodeSize / 2, position.y - nodeSize / 2, nodeSize, nodeSize);
        Vector2 newNodeCenter = new Vector2(newNodeRect.center.x, newNodeRect.center.y);

        foreach (var rect in placedNodeRects)
        {
            Vector2 existingNodeCenter = new Vector2(rect.center.x, rect.center.y);
            if (Vector2.Distance(existingNodeCenter, newNodeCenter) < minDistance)
            {
                return true; // 겹침
            }

            if (rect.Overlaps(newNodeRect))
            {
                return true; // 겹침
            }
        }
        return false; // 겹치지 않음
    }

    public void ConnectNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            int connectionCount = random.Next(1, 4); // 각 노드당 연결 수
            for (int i = 0; i < connectionCount; i++)
            {
                GameObject targetNode = nodes[random.Next(nodes.Count)];
                if (targetNode != node)
                {
                    DrawLine(node.GetComponent<RectTransform>().anchoredPosition,
                             targetNode.GetComponent<RectTransform>().anchoredPosition,
                             boundingBox);
                }
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end, Transform parent)
    {
        // 노드 크기 만큼 오프셋을 적용해 라인의 시작/끝점을 수정
        Vector2 startOffset = start + new Vector2(nodeSize / 2, nodeSize / 2); // 노드의 오른쪽 하단
        Vector2 endOffset = end + new Vector2(nodeSize / 2, nodeSize / 2); // 노드의 오른쪽 하단

        GameObject line = new GameObject("Line");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, startOffset);
        lr.SetPosition(1, endOffset);
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        line.transform.SetParent(parent, false);
    }
}
