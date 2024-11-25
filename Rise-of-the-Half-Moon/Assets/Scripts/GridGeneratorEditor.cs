using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGeneratorWithEightDirections))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector UI 렌더링
        DrawDefaultInspector();

        // 대상 스크립트 가져오기
        GridGeneratorWithEightDirections generator = (GridGeneratorWithEightDirections)target;

        GUILayout.Space(10); // 간격 추가

        // "Generate Grid" 버튼
        if (GUILayout.Button("Generate Grid"))
        {
            // 현재 씬에 있는 기존 노드와 엣지 제거
            ClearGeneratedObjects(generator);

            // 새로운 그리드 생성
            generator.Create();
        }

        // "Randomize Seed & Generate" 버튼
        if (GUILayout.Button("Randomize Seed & Generate"))
        {
            // 새로운 시드 생성 (랜덤값)
            generator.seed = Random.Range(0, 10000);

            // 현재 씬에 있는 기존 노드와 엣지 제거
            ClearGeneratedObjects(generator);

            // 새로운 그리드 생성
            generator.Create();
        }
    }

    // 기존 노드와 엣지를 삭제하는 함수
    private void ClearGeneratedObjects(GridGeneratorWithEightDirections generator)
    {
        // 생성된 노드와 엣지 GameObject 리스트를 순회하며 제거
        foreach (GameObject node in generator.nodeObjects)
        {
            DestroyImmediate(node);
        }
        foreach (GameObject edge in generator.edgeObjects)
        {
            DestroyImmediate(edge);
        }

        // 리스트 초기화
        generator.nodeObjects.Clear();
        generator.edgeObjects.Clear();
    }
}