using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGeneratorWithEightDirections))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ Inspector UI ������
        DrawDefaultInspector();

        // ��� ��ũ��Ʈ ��������
        GridGeneratorWithEightDirections generator = (GridGeneratorWithEightDirections)target;

        GUILayout.Space(10); // ���� �߰�

        // "Generate Grid" ��ư
        if (GUILayout.Button("Generate Grid"))
        {
            // ���� ���� �ִ� ���� ���� ���� ����
            ClearGeneratedObjects(generator);

            // ���ο� �׸��� ����
            generator.Create();
        }

        // "Randomize Seed & Generate" ��ư
        if (GUILayout.Button("Randomize Seed & Generate"))
        {
            // ���ο� �õ� ���� (������)
            generator.seed = Random.Range(0, 10000);

            // ���� ���� �ִ� ���� ���� ���� ����
            ClearGeneratedObjects(generator);

            // ���ο� �׸��� ����
            generator.Create();
        }
    }

    // ���� ���� ������ �����ϴ� �Լ�
    private void ClearGeneratedObjects(GridGeneratorWithEightDirections generator)
    {
        // ������ ���� ���� GameObject ����Ʈ�� ��ȸ�ϸ� ����
        foreach (GameObject node in generator.nodeObjects)
        {
            DestroyImmediate(node);
        }
        foreach (GameObject edge in generator.edgeObjects)
        {
            DestroyImmediate(edge);
        }

        // ����Ʈ �ʱ�ȭ
        generator.nodeObjects.Clear();
        generator.edgeObjects.Clear();
    }
}