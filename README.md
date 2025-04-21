# Rise of the Half Moon

### 구글 두들 게임
플레이해보기
https://doodles.google/doodle/rise-of-the-half-moon/


### Ingame Video
https://github.com/user-attachments/assets/c0950ac0-29d3-4428-b2b0-e1d72912d09e


## 요약
### 사용기술
- 아틀라스 sprite 를 다중 Sprite로 추출
- 점수 Cycle 탐색을 위해 DPS 사용
- Scriptable Object를 이용하여 데이터 관리
- Google Firebase realtimeDB
- Google Firebase Auth
- PUN2을 이용하여 매치메이킹 및 게임 진행
- DOTween
- PostProcessing을 이용하여, Game의 유불리함 표현

--------------------------------------------------------------------------------------------------------------------------------------------
# Implement

## Game Contents
### PhaseCycle 요약
- Card를 Node에 두었을 때, 해당 Node와 관련 있는 모든 Phase Cycle을 찾는 함수
    <details>
    <summary>Code</summary>
    <div markdown="1">
     
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
      
                      if(result.Count > 2)
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
                  if (currentPathNode.Contains(nextNode))
                      continue;
                  isFindNext = true;
                  List<Node> nodes = new List<Node>(currentPathNode);
                  nodes.Add(nextNode);
                  SetNextNodeList(ref nextNodeLists, ref nodes, nextNode);
              }
              if (!isFindNext)
              {
                  nextNodeLists.Add(currentPathNode);
              }
          }
      
          private void SetPrevNodeList(ref List<List<Node>> nextNodeLists, ref List<Node> currentPathNode, Node node)
          {
              bool isFindNext = false;
              foreach (Node nextNode in node.prevNodes)
              {
                  if (currentPathNode.Contains(nextNode))
                      continue;
                  isFindNext = true;
                  List<Node> nodes = new List<Node>(currentPathNode);
                  nodes.Add(nextNode);
                  SetPrevNodeList(ref nextNodeLists, ref nodes, nextNode);
              }
              if (!isFindNext)
              {
                  nextNodeLists.Add(currentPathNode);
              }
          }
      
          public void UpdateNearNodeInfo(Node node)
          {
              var connectedNodes = node.GetAdjacentNodes();
              foreach(var connectedNode in connectedNodes)
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
    
    </div>
    </details>


## Firebase
### Auth
- Anonymous
- Google
## Realtime database
 - User
 - Score(MMR)

## Photon Pun2
### Room
- Matchmaking
### RPC
- Put Cards
- Use Item

## Admob
## Sounds
## Effect
