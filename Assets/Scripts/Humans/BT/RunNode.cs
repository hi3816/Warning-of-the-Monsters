// using UnityEngine;
//
// public class RunNode : INode
// {
//     private HumanController _humanController;
//     private Vector3 _spawnPosition;
//     
//     public RunNode(HumanController humanController, Vector3 spawnPosition)
//     {
//         _humanController = humanController;
//         _spawnPosition = spawnPosition;
//     }
//
//     public NodeState Evaluate()
//     {
//         _humanController.nodeTxt.text = "Run";
//         // Debug.Log($"Evaluating {this.GetType().Name}");
//         if (_humanController.IsFearMaxed())
//         {
//             _humanController.animator.SetTrigger("Run");
//             _humanController.ArriveToDestination(_spawnPosition);
//             return NodeState.Running;
//         }
//         return NodeState.Failure;
//     }
// }