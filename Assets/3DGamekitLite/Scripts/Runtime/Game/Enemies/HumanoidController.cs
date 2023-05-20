using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using AnnulusGames.LucidTools.RandomKit;
using UnityEngine.Serialization;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies
{
    public class HumanoidController : MonoBehaviour
    {

        /// <Summary>
        /// どの音を再生するかを設定します
        /// </Summary>
        [SerializeField] private AudioClip _se_death;

        //音を再生するためのコンポーネントの情報を格納する変数です
        [SerializeField] private AudioSource _audioSource;

        /// <Summary>
        /// この変数に対してUnityの画面上でプレイヤーを設定することで、敵がプレイヤーに向かいます
        /// </Summary>
        [SerializeField] private Transform _target;

        /// <Summary>
        /// この変数に対してターゲットとしてプレイヤーを指定することで敵がプレイヤーに向かいます
        /// </Summary>
        [SerializeField] private NavMeshAgent _navMeshAgent;
        
        /// <Summary>
        /// 敵がこちらに気づいて近づいてくる距離を設定します
        /// </Summary>
        [SerializeField] private float noticeDistance = 10.0f;

        /// <Summary>
        /// Distance start to battle.
        /// </Summary>
        [SerializeField] private float battleDistance;

        /// <Summary>
        /// この変数の中の値を変更することで対応したアニメーションが再生されます
        /// </Summary>
        [SerializeField] public Animator animator;

        /// <Summary>
        /// set moving speed.
        /// </Summary>
        [SerializeField] private int MoveSpeed;
        
        /// <Summary>
        /// set stepping speed.
        /// </Summary>
        [SerializeField] private float StepSpeed;
        
        /// <Summary>
        /// set moving time.
        /// </Summary>
        [SerializeField] private float MoveTime;
        
        /// <Summary>
        /// set stepping time.
        /// </Summary>
        [SerializeField] private float StepTime;
        
        /// <Summary>
        /// set idling time.
        /// </Summary>
        [SerializeField] private float IdleTime;
        
        /// <Summary>
        /// 物理演算を行うオブジェクトです
        /// </Summary>
        [SerializeField] private Rigidbody _rigidbody;

        private const string MeleeAttackPattern = "MeleeAttackPattern";
        private const string GuardWhileMovingPattern = "GuardWhileMovingPattern";
        private const string StepAvoidancePattern = "StepAvoidancePattern";
        private const string IdlePattern = "IdlePattern";
        
        private const string MoveRightPattern = "MoveRightPattern";
        private const string MoveBackPattern = "MoveBackPattern";
        private const string MoveLeftPattern = "MoveLeftPattern";
        
        private const string StepRightPattern = "StepRightPattern";
        private const string StepBackPattern = "StepBackPattern";
        private const string StepLeftPattern = "StepLeftPattern";
        
        private const int AttackFromLeftPattern = 1;
        private const int AttackFromUpperPattern = 2;
        private const int AttackFromRightPattern = 3;

        /// <Summary>
        /// List for randomizing patterns of enemy actions.
        /// </Summary>
        [SerializeField] private WeightedList<string> meleeActionWeightedList = new WeightedList<string>(MeleeAttackPattern, GuardWhileMovingPattern, StepAvoidancePattern, IdlePattern);

        /// <Summary>
        /// List for randomizing patterns of enemy melee attack.
        /// </Summary>
        [SerializeField] private WeightedList<int> meleeAttackWeightedList = new WeightedList<int>(AttackFromLeftPattern, AttackFromUpperPattern, AttackFromRightPattern);

        /// <Summary>
        /// List for randomizing patterns of enemy move.
        /// </Summary>
        [SerializeField] private WeightedList<string> moveWeightedList = new WeightedList<string>(MoveRightPattern, MoveBackPattern, MoveLeftPattern);

        /// <Summary>
        /// List for randomizing patterns of enemy move.
        /// </Summary>
        [SerializeField] private WeightedList<string> stepWeightedList = new WeightedList<string>(StepRightPattern, StepBackPattern, StepLeftPattern);


        //文字列をハッシュという数字に予め変換しておくことで、処理の度に文字列化を行ないでよいようにして負荷を軽減します
        //また、文字列の打ち間違いをしないようにします
        private static readonly int AnimationMovingHash = Animator.StringToHash("Moving");
        private static readonly int AnimationBattleRandomHash = Animator.StringToHash("BattleRandom");


        /// <Summary>
        /// 敵が倒れるまでにかかる時間です
        /// </Summary>
        private readonly float _timeEnemyDead = 1.3f;

        /// <Summary>
        /// 敵を倒したときのスローを解除するまでの時間です
        /// </Summary>
        private readonly float _delayTime = 2.3f;

        /// <Summary>
        /// set enemy action pattern.
        /// </Summary>
        private string _actionPattern;
        
        /// <Summary>
        /// set enemy attack pattern.
        /// </Summary>
        private int _attackPattern;
        
        /// <Summary>
        /// set enemy move pattern.
        /// </Summary>
        private string _movePattern;
        
        /// <Summary>
        /// set enemy step pattern.
        /// </Summary>
        private string _stepPattern;
        
        /// <Summary>
        /// 敵の戦闘時の左右移動のための変数です
        /// </Summary>
        private const float MovingWaitSec = 3f;
        private float _actionWaitTimer = 0f;

        /// <summary>
        /// enemy move speed variables
        /// </summary>
        private float _actionSpeed = 0f;
        private Vector3 _rotateAxis = Vector3.zero;

        
        
        /// <Summary>
        /// 敵が倒れたときにアニメーションから呼び出される処理を定義します
        /// </Summary>
        private void OnDeath()
        {
            _audioSource.PlayOneShot(_se_death);
        }

        /// <Summary>
        /// 左右移動中かを判定します
        /// </Summary>
        private bool InAction()
        {
            // TODO: 以下、Timerを使った仮の判定。本来は移動アニメーションが終了してるかどうかで判定すべき。
            if (_actionWaitTimer > 0f)
            {
                _actionWaitTimer -= Time.deltaTime;
            }

            return (_actionWaitTimer > 0f);
        }

        /// <Summary>
        /// ゲームの起動中継続して実行される処理です
        /// </Summary>
        private void Update()
        {
            if (Vector3.Distance(_target.position, _navMeshAgent.transform.position) <
                noticeDistance)
            {
               //プレイヤーの位置まで移動します
               _navMeshAgent.SetDestination(_target.position);
            }

            //敵が動いたら歩行アニメーションを再生します
            //https://buravo46.hatenablog.com/entry/2014/09/07/154834
            if (!(_rigidbody.IsSleeping()))
            {
                animator.SetBool(AnimationMovingHash, true);
            }
            else
            {
                animator.SetBool(AnimationMovingHash, false);
            }
            
            //プレイヤーと敵の距離がNavMeshAgentで設定している停止距離より少し近くなったら敵が攻撃を開始します
            if (Vector3.Distance(_target.position, _navMeshAgent.transform.position) < _navMeshAgent.stoppingDistance + battleDistance)
            {
                //プレイヤーの位置から自分の位置を引くことで、敵から見たプレイヤーの位置を算出します（★原理がよくわかっていない） https://gomafrontier.com/unity/2883
                //y軸を固定することで敵が上を向かないようにします
                var direction = _target.position - transform.position;
                direction.y = 0;

                //敵がプレイヤーの方向を向くようにします
                //振り向く速さはLerp()の第三引数で調整します
                var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);

                var currentClipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                var battleRandomValue = animator.GetInteger(AnimationBattleRandomHash);

                // Verify enemy is walking or idling.
                if (currentClipName == "WalkForward" || currentClipName == "Idle" )
                {
                    //左右に移動中かを判定します
                    if (InAction())
                    {
                        switch (_actionPattern)
                        {
                            case GuardWhileMovingPattern:
                            case StepAvoidancePattern:
                                // _rotateAxisに応じて移動方向が変わります。
                                // Vector3.up:プレイヤーからみて右、Vector3.down:プレイヤーからみて左
                                //参考：https://nekojara.city/unity-circular-motion
                                transform.RotateAround(_target.position, _rotateAxis,
                                    360 / _actionSpeed * Time.deltaTime);
                                break;
                            case IdlePattern:
                                //do nothing
                                break;
                        }

                    }
                    else if(1 <= battleRandomValue && battleRandomValue <= 3)
                    {
                        // 攻撃への移行待ちです（何もしません）
                    }
                    // 次の行動を決められる状態です
                    else
                    {
                        _actionPattern = meleeActionWeightedList.RandomElement();
                        Debug.Log($"_actionPattern:{_actionPattern}");
                        switch (_actionPattern)
                        {
                            case MeleeAttackPattern:
                                //set random melee attack pattern.
                                _attackPattern =  meleeAttackWeightedList.RandomElement();
                                Debug.Log($"_attackPattern:{_attackPattern}");
                                animator.SetInteger(AnimationBattleRandomHash, _attackPattern);
                                break;
                            
                            case GuardWhileMovingPattern:
                                //reset melee attack pattern.
                                animator.SetInteger(AnimationBattleRandomHash, 0);
                                _actionSpeed = MoveSpeed;
                                _actionWaitTimer = MoveTime;
                                
                                _movePattern = moveWeightedList.RandomElement();
                                Debug.Log($"_movePattern:{_movePattern}");
                                switch (_movePattern)
                                {
                                    case MoveRightPattern:
                                        _rotateAxis = Vector3.up;
                                        break;
                                    case MoveLeftPattern:
                                        _rotateAxis = Vector3.down;
                                        break;
                                    case MoveBackPattern:
                                        //I haven't implemented yet, so do nothing.
                                        break;
                                }
                                break;
                            
                            case StepAvoidancePattern:
                                animator.SetInteger(AnimationBattleRandomHash, 0);
                                _actionSpeed = StepSpeed;
                                _actionWaitTimer = StepTime;
                                
                                _stepPattern = stepWeightedList.RandomElement();
                                Debug.Log($"_stepPattern:{_stepPattern}");
                                switch (_stepPattern)
                                {
                                    case StepRightPattern:
                                        _rotateAxis = Vector3.up;
                                        break;
                                    case StepLeftPattern:
                                        _rotateAxis = Vector3.down;
                                        break;
                                    case StepBackPattern:
                                        //I haven't implemented yet, so do nothing.
                                        break;
                                }
                                break;
                            
                            case IdlePattern:
                                //reset melee attack pattern.
                                animator.SetInteger(AnimationBattleRandomHash, 0);
                                _actionSpeed = 0; 
                                _actionWaitTimer = IdleTime;
                                break;
                        }
                    }
                }
                // 攻撃中です
                else if (
                    (currentClipName == "AttackFromUpper") || (currentClipName == "AttackFromRight") || (currentClipName == "AttackFromLeft")
                )
                {
                    //現状は特に何もしません
                }
            }
        }

    }
}