using System;
using UnityEngine;

public class HumanBT : MonoBehaviour
{
    private BehaviorTree _behaviorTree;
    private HumanController _humanController;

    public Transform spawnPoint;
    public Transform targetPoint;

    private void Awake()
    {
        _humanController = TryGetComponent<HumanController>(out _humanController)
            ? _humanController : gameObject.AddComponent<HumanController>();
        spawnPoint = GameObject.FindWithTag("HumanSpawnPoint").transform;
        if (spawnPoint == null)
        {
            Debug.LogAssertion("Could not find spawn point");
        }
        targetPoint = GameObject.FindWithTag("DestinationPoint").transform;
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
        SelectorNode root = new SelectorNode();

        // Idle Node
        SequenceNode idleSequence = new SequenceNode();
        idleSequence.AddChild(new IdleNode(_humanController));
        //IdleNode idleNode = new IdleNode(_humanController);

        // Walk Node
        SequenceNode walkSequence = new SequenceNode();
        walkSequence.AddChild(new WalkNode(_humanController, targetPoint.position));
        //WalkNode walkNode = new WalkNode(_humanController, targetPoint.position);

        // Battle Subtree
        SelectorNode battleSelector = new SelectorNode();

        SequenceNode attackSequence = new SequenceNode();
        var attackNode = new AttackNode(_humanController);
        attackSequence.AddChild(attackNode);
        attackSequence.AddChild(new WaitNode(_humanController));

        SequenceNode formationSequence = new SequenceNode();
        formationSequence.AddChild(new SetFormationNode(_humanController));
        //SetFormationNode setFormationNode = new SetFormationNode(_humanController);

        battleSelector.AddChild(formationSequence);
        //battleSelector.AddChild(setFormationNode);
        battleSelector.AddChild(attackSequence);

        // Run Node
        SequenceNode runSequence = new SequenceNode();
        runSequence.AddChild(new RunNode(_humanController, spawnPoint.position));
        //RunNode runNode = new RunNode(_humanController, spawnPoint.position);
        
        // Surprise Node
        SurpriseNode surpriseNode = new SurpriseNode(_humanController);

        root.AddChild(surpriseNode); // Any State
        root.AddChild(idleSequence);
        //root.AddChild(idleNode);
        root.AddChild(walkSequence);
        //root.AddChild(walkNode);
        root.AddChild(battleSelector);
        root.AddChild(runSequence);
        //root.AddChild(runNode);

        _behaviorTree = new BehaviorTree(root);
    }
}