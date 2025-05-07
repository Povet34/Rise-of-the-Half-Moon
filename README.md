# 🌙 Rise of the Half Moon - Remake Project

## 🎮 원작 게임
Google Doodle 원작 게임입니다. 아래 링크에서 플레이해볼 수 있습니다:  
🔗 [Rise of the Half Moon - Play](https://doodles.google/doodle/rise-of-the-half-moon/)

## 📺 인게임 플레이 영상
▶️ [Gameplay Video](https://github.com/user-attachments/assets/c0950ac0-29d3-4428-b2b0-e1d72912d09e)

---

## 🛠️ 사용 기술

- 🎨 Sprite Atlas → 다중 Sprite 추출
- 📈 DPS를 이용한 점수 Cycle 탐색
- 📦 `ScriptableObject`로 게임 데이터 관리
- ☁️ Firebase Realtime Database
- 🔐 Firebase Authentication (익명/Google 로그인)
- 🔄 Photon PUN2 (멀티플레이 매치메이킹, 게임 동기화)
- 🔁 DOTween (애니메이션)
- 🎥 PostProcessing (게임 내 상황 강조 효과)

---





# 🚧 구현 항목

## 🧩 Game Contents

### 🔄 Phase Cycle 탐색
카드를 Node에 둘 경우, 연결된 모든 Phase Cycle을 찾는 기능

<details>
<summary><strong>🔍 코드 보기 - FindCycle()</strong></summary>

```csharp
public class NodeCycleHelper
{
    public int row;
    public int col;
    public List<Node> nodes = new List<Node>();
    GameManager gameManager;

    public NodeCycleHelper(GameManager gameManager, List<Node> nodes, int row, int col)
    {
        this.gameManager = gameManager;
        this.row = row;
        this.col = col;
        this.nodes = nodes;
    }

    public Dictionary<string, List<Node>> FindCycle(Node putNode)
    {
        Dictionary<string, List<Node>> resultDic = new Dictionary<string, List<Node>>();

        UpdateNearNodeInfo(putNode);

        List<List<Node>> nextNodeLists = new List<List<Node>>();
        List<List<Node>> prevNodeLists = new List<List<Node>>();
        List<Node> temp = new List<Node>();
        SetNextNodeList(ref nextNodeLists, ref temp, putNode);
        temp = new List<Node>();
        SetPrevNodeList(ref prevNodeLists, ref temp, putNode);

        foreach (List<Node> prevs in prevNodeLists)
        {
            prevs.Reverse();

            foreach (List<Node> next in nextNodeLists)
            {
                List<Node> result = new List<Node>();
                List<Node> tmpNexts = new List<Node>(next);

                if (prevs.Count > 0)
                {
                    for (int i = 0; i < next.Count; i++)
                    {
                        if (next[i] == prevs[0])
                        {
                            tmpNexts = next.GetRange(0, i);
                            break;
                        }
                    }
                }

                string key = "";
                foreach (var pre in prevs)
                {
                    key += pre.index;
                    result.Add(pre);
                }

                key += putNode.index;
                result.Add(putNode);

                foreach (var nxt in tmpNexts)
                {
                    key += nxt.index;
                    result.Add(nxt);
                }

                if (result.Count > 2)
                    resultDic[key] = result;
            }
        }

        return resultDic;
    }

    private void SetNextNodeList(ref List<List<Node>> nextNodeLists, ref List<Node> currentPathNode, Node node)
    {
        bool isFindNext = false;
        foreach (Node nextNode in node.nextNodes)
        {
            if (currentPathNode.Contains(nextNode)) continue;
            isFindNext = true;
            List<Node> nodes = new List<Node>(currentPathNode);
            nodes.Add(nextNode);
            SetNextNodeList(ref nextNodeLists, ref nodes, nextNode);
        }

        if (!isFindNext) nextNodeLists.Add(currentPathNode);
    }

    private void SetPrevNodeList(ref List<List<Node>> nextNodeLists, ref List<Node> currentPathNode, Node node)
    {
        bool isFindNext = false;
        foreach (Node nextNode in node.prevNodes)
        {
            if (currentPathNode.Contains(nextNode)) continue;
            isFindNext = true;
            List<Node> nodes = new List<Node>(currentPathNode);
            nodes.Add(nextNode);
            SetPrevNodeList(ref nextNodeLists, ref nodes, nextNode);
        }

        if (!isFindNext) nextNodeLists.Add(currentPathNode);
    }

    public void UpdateNearNodeInfo(Node node)
    {
        var connectedNodes = node.GetAdjacentNodes();
        foreach (var connectedNode in connectedNodes)
        {
            if (IsNext(connectedNode, node))
            {
                node.prevNodes.Add(connectedNode);
                connectedNode.nextNodes.Add(node);
            }
            if (IsPrev(connectedNode, node))
            {
                node.nextNodes.Add(connectedNode);
                connectedNode.prevNodes.Add(node);
            }
        }
    }

    private bool IsNext(Node node1, Node node2)
    {
        return PhaseData.GetNextPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }

    private bool IsPrev(Node node1, Node node2)
    {
        return PhaseData.GetPreviousPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }
}

```

</details>

---

## ☁️ Firebase 연동

### 🔐 Auth 방식

- ✅ 익명 로그인
    
- ✅ Google 로그인
    

### 📊 Realtime Database 구조

- `Users`: 유저 정보
    
- `Scores`: 유저별 점수 (MMR 등)
-  점수는 저장하지만 아직 구현 x
    

---

## 🔄 Photon PUN2

### 🏠 Room 관리

- 자동 매치메이킹 지원
    

### 🔁 RPC 활용

- 카드 놓기 (`PutCard`)
    
- 아이템 사용 (`UseItem`)
    

---

## 📢 기타 시스템

~~### 📱 AdMob~~

~~- 광고 지원 (보상형 등)~~
    

### 🔊 Sounds

- BGM / 효과음 구성
    

### 💥 Effects

- 카드 배치 / 스킬 사용 시 이펙트 처리
