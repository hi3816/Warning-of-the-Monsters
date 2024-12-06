using System.Collections.Generic;
using UnityEngine;

public class HumanBT : MonoBehaviour
{
    private BehaviorTree _behaviorTree;
    private HumanController _humanController;

    public Vector3 spawnPoint;
    public Vector3 targetPoint;

    private void Awake()
    {
        _humanController = TryGetComponent<HumanController>(out _humanController)
            ? _humanController : gameObject.AddComponent<HumanController>();
        if (GameObject.FindWithTag("HumanSpawnPoint") != null)
        {
            spawnPoint = GameObject.FindWithTag("HumanSpawnPoint").transform.position;
        }
        if (spawnPoint == null)
        {
            Debug.LogAssertion("Could not find spawn point");
        }

        if (GameObject.FindWithTag("DestinationPoint") != null)
        {
            targetPoint = GameObject.FindWithTag("DestinationPoint").transform.position;

        }
        if (targetPoint == null)
        {
            Debug.LogAssertion("Could not find destination point");
        }
    }

    private void Start()
    {
        InitializeBehaviorTree();
    }

    private void Update()
    {
        _behaviorTree.Update();
    }
    /* 참고 사항 */
    // BT 구조 이미지는 팀노션 페이지에 수정하는 대로 업데이트 예정(현재는 스크립트로만 작성됨)
    // TODO: unity editor experimental graph view 활용 or 외부 패키지 사용하여 Edtor에서 확인 가능하게 수정
    private void InitializeBehaviorTree()   // 인간 행동 트리 초기화
    {
        // SelectorNode root = new SelectorNode();
        //
        // IdleNode idleNode = new IdleNode(_humanController);
        // RunNode runNode = new RunNode(_humanController, spawnPoint);
        //
        //
        //  // Ready To Battle        
        //  SelectorNode readySelector = new SelectorNode();
        //  WalkNode walkNode = new WalkNode(_humanController, targetPoint);
        //  readySelector.AddChild(walkNode);
        // // Battle
        // SequenceNode battleSequence = new SequenceNode();
        // battleSequence.AddChild(new SetFormationNode(_humanController));
        // battleSequence.AddChild(new AttackNode(_humanController));
        //
        // readySelector.AddChild(battleSequence);
        //
        // root.AddChild(idleNode);
        // root.AddChild(runNode);
        // root.AddChild(readySelector);
        //
        // _behaviorTree = new BehaviorTree(root);
        SelectorNode root = new SelectorNode
        (
            new List<INode>
            {
                new IdleNode(_humanController), 

                new RunNode(_humanController, spawnPoint), 

                // 전투 분기 노드
                new SelectorNode
                (
                    new List<INode>
                    {
                        new WalkNode(_humanController, targetPoint),

                        // 전투 시퀀스
                        new SequenceNode
                        (
                            new List<INode>
                            {
                                new SetFormationNode(_humanController), // 공격 위치 조정
                                new AttackNode(_humanController)      // 공격 수행
                            }
                        )
                    }
                )
            }
        );
        _behaviorTree = new BehaviorTree(root);

    }
}