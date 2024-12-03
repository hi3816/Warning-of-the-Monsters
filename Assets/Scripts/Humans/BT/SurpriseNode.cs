// using UnityEngine;
//
// public class SurpriseNode : INode
// {
//     private HumanController _humanController;
//     private bool _hasReacted;   // 겁주기에 반응했는지 확인
//
//     public SurpriseNode(HumanController humanController)
//     {
//         _humanController = humanController;
//         _hasReacted = false;
//     }
//
//     public NodeState Evaluate()
//     {
//         if (!_hasReacted)   // 겁주기 반응하지 않았으면
//         {
//             Debug.LogAssertion("Enter SurpriseNode");
//             _humanController.animator.SetBool("IsSurprised", true);
//             _humanController.ReactToScaring(); // 겁주기에 반응하기(두려움 수치 증가)
//             _hasReacted = true;
//             _humanController.animator.SetBool("IsSurprised", false);
//             return NodeState.Success; // 루트 노드로 이동
//         }
//         return NodeState.Failure; // 이전 상태로 전환
//     }
//
//     public void Reset()
//     {
//         _hasReacted = false; // 해당 노드에 들어왔을 때 겁주기 반응 상태 초기화
//     }
// }