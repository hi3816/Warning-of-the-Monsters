// using UnityEngine;
//
// public class SetFormationNode : INode
// {
//     private HumanController _humanController;
//     
//     public SetFormationNode(HumanController humanController)
//     {
//         _humanController = humanController;
//     }
//
//     public NodeState Evaluate()
//     {
//         _humanController.nodeTxt.text = "SetFormation";
//         // Debug.Log($"Evaluating {this.GetType().Name}");
//         _humanController.animator.SetBool("IsBattle", true);
//         // 타켓 몬스터의 위치로 계속해서 이동
//         Vector3 formationPosition = _humanController.human.targetMonster.transform.position;
//         _humanController.MoveToFormationPosition(formationPosition);
//         return NodeState.Success; // TestCode 바로 상태 전환
//     }
// }