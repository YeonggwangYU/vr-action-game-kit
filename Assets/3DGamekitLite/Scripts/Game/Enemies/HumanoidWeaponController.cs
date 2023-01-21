using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidWeaponController : MonoBehaviour
{

    /// <Summary>
    /// アニメーションのパラメータの打ち間違いを防ぐため、変数に格納してSetTriggerに渡します
    /// </Summary>
    private static readonly int AnimationRepelledHash = Animator.StringToHash("Repelled");

    /// <Summary>
    /// この変数の中の値を変更することでこの武器を持つ敵の対応したアニメーションが再生されます
    /// </Summary>
    [SerializeField] private Animator _animator;

    /// <Summary>
    /// この変数の中の値を変更することで武器の当たり判定の有無を操作します
    /// </Summary>
    [SerializeField] private BoxCollider _boxCollider;

    private void Start()
    {
        //攻撃モーションが始まるまでは当たり判定を無効化します
        DisableAttack();
    }

    /// <Summary>
    /// 武器のColliderを有効にします。
    /// 色々なシチュエーションで使えるように他のスクリプトから呼び出せるようにpublicにします。
    /// </Summary>
    public void EnableAttack()
    {
        _boxCollider.enabled = true;
    }

    /// <Summary>
    /// 武器のColliderを無効にします。
    /// 色々なシチュエーションで使えるように他のスクリプトから呼び出せるようにpublicにします。
    /// </Summary>
    public void DisableAttack()
    {
        _boxCollider.enabled = false;
    }

    /// <Summary>
    /// プレイヤーの武器が敵の武器に設定したColliderに触れると敵がのけぞるアニメーションをオンにします
    /// </Summary>
    private void OnTriggerEnter(Collider other)
    {
        //当たったのがプレイヤーの武器かどうかを判定します
        if (other.gameObject.TryGetComponent<PlayerWeapon>(out PlayerWeapon _playerWeapon))
        {
            //敵の攻撃が当たったことを示すパラメーターをオンにします
            //Triggerの場合は自動でオフになるため、Boolのようにfalseにする処理は必要ありません
            _animator.SetTrigger(AnimationRepelledHash);

            //敵の攻撃が当たり終わったので、武器の当たり判定をオフにします
            DisableAttack();
        }
    }
}
