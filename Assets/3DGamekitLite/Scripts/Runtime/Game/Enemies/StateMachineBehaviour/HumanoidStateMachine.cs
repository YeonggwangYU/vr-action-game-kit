using UnityEngine;

public class HumanoidStateMachine : StateMachineBehaviour
{
    private static readonly int AnimationBattleRandomHash = Animator.StringToHash("BattleRandom");

    //状態が変わった時に実行
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Idle1"))
        {
            // 攻撃パターンをリセット
            animator.SetInteger(AnimationBattleRandomHash, 0);
            Debug.Log($"Idle1:攻撃パターンをリセット");
        }
    }
}