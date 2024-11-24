using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonGridPlacedMap : MonoBehaviour
{
    public int nodeCount = 10;        // ��� ����
    public float nodeSize = 50f;      // ��� ũ��
    public float minDistance = 20f;   // ��尣 �ּ� �Ÿ�

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
            } while (IsOverlapping(position) && attempts < 100); // �ִ� 100�� �õ�

            if (attempts >= 100)
            {
                Debug.LogWarning("�� �̻� ��ġ�� �ʴ� ��ġ�� ã�� �� �����ϴ�.");
                continue;
            }

            GameObject newNode = Instantiate(nodePrefab, boundingBox);
            RectTransform rectTransform = newNode.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(nodeSize, nodeSize); // ��� ũ�� ����

            // ��ġ�� ����� Rect ����
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
                return true; // ��ħ
            }

            if (rect.Overlaps(newNodeRect))
            {
                return true; // ��ħ
            }
        }
        return false; // ��ġ�� ����
    }

    public void ConnectNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            int connectionCount = random.Next(1, 4); // �� ���� ���� ��
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
        // ��� ũ�� ��ŭ �������� ������ ������ ����/������ ����
        Vector2 startOffset = start + new Vector2(nodeSize / 2, nodeSize / 2); // ����� ������ �ϴ�
        Vector2 endOffset = end + new Vector2(nodeSize / 2, nodeSize / 2); // ����� ������ �ϴ�

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
