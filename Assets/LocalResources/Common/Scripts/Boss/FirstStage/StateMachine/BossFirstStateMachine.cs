using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum BossState
{
    //�Զ�����״̬
    EatBeans,            //�Զ�
    EatBeansRangedAttack, //�Զ�����Զ�̹���

    //��������״̬
    AttackIdle,          //��������
    RangedAttack,       //Զ�̹���
    DashAttack,         //��湥��
    AttackRandomMove,   //�������͵��ƶ�

    //ͨ������״̬
    Hurt,               //����
    Die,                //����
    Grow,               //�����䳤һ��
}

#endregion

#region AudioEnum
public enum EBossDash { on, off }
public enum EBossEatBeansMove { on, off }
public enum EBossEatBean { on, off }
public enum EBossRangedAttack { on, off }
#endregion

public class BossFirstStateMachine : MonoBehaviour
{
    #region State Machine Config
    [Header("״̬����")]
    public BossState startingState = BossState.EatBeans;
    public LayerMask PlayerLayer;

    [Header("������Ϣ")]
    [SerializeField] private BossState _currentState;
    [SerializeField] private string _currentStateName;
    #endregion

    #region Boss����
    [Header("��������")]
    [Range(10f, 1000f)] public float MaxHealth = 100f;
    [Range(1f, 60f)] public float EatBeanMoveSpeed = 3f;
    [Range(1f, 60f)] public float AttackMoveSpeed = 5f;
    private float _currentMoveSpeed;
    [SerializeField] private BossSlider _slider;

    [Header("�ߵĽ�������")]
    [Range(1f, 20f)] public int StartSnakeSegments = 5;
    [Range(1f, 20f)] public int MaxSnakeSegments = 10;
    public Transform SegmentPrefab;
    [HideInInspector] public List<Transform> Segments = new List<Transform>();
    [HideInInspector] public bool IsMove = true;
    [HideInInspector] public int CurrentSegmentNum;

    [Header("�ܻ�����")]
    [Range(0f, 1f)] public float HurtInvulnerableTime = 0.1f;       //�ܻ���Ķ����޵�ʱ��(�����ظ��ж�)

    [Header("��ײ�˺�")]
    [Range(1f, 50f)] public float HitDamage = 15f;
    private float _playerInvulnerableTime = 0.2f;
    private bool _canHurt = true;

    [HideInInspector] public bool _isAtLeft = false;
    #endregion



    #region BossEatBeansRangedAttackState_First
    [Header("BossEatBeansRangedAttackState_First���ӵ�")]
    public GameObject BulletPrefab;
    public Transform FirePoint;
    [Range(1f, 30f)] public float BulletSpeed = 5f;

    [Header("BossEatBeansRangedAttackState_First��Boss���н�ֱʱ��")]
    [Range(0f, 2f)] public float EatBeansRangedAttackInvulnerableTime;
    #endregion

    #region BossGrowState_First
    [Header("BossGrowState_First")]
    [Range(0f, 2f)] public float StateInvulnerableTime = 1f;
    #endregion

    #region BossEatBeansState_First
    [Header("BossEatBeansState_First")]
    [Range(0.5f, 2f)] public float RepathInterval = 0.5f;                  // ��������·������ֹ�ϰ�/�����ƶ�����·��ʧЧ
    [Range(0.1f, 10f)] public float EatDistance = 1f;                     // ����˾�����Ϊ���Ե�������

    public LayerMask BeanLayer;
    #endregion



    #region BossAttackIdleState_First
    [Header("BossAttackIdleState_First��Boss��λ���趨")]
    public Transform IdleLeftTransfrom;
    public Transform IdleRightTransfrom;

    [Header("BossAttackIdleState_First��Boss�Ľ�ֱʱ��")]
    [Range(1f, 5f)] public float IdleTime = 2f;
    #endregion

    #region BossRangedAttackState_First
    [Header("BossRangedAttackState_First���ӵ�")]
    public GameObject BulletPrefab_Attack;
    public GameObject LittleBulletPrefab_Attack;
    public Transform FirePoint_Attack;
    [Range(1f, 50f)] public float BulletSpeed_Attack = 15f;
    [Range(1f, 50f)] public float LittleBulletSpeed_Attack = 17f;

    [Header("BossRangedAttackState_First��Boss���н�ֱʱ��")]
    [Range(0f, 2f)] public float RangedAttackInvulnerableTime;
    #endregion

    #region BossDashAttackState_First
    [Header("��ײ�������")]
    public Transform TelePortLeftPoint;
    public Transform TelePortRightPoint;

    [Header("��̵Ľ���λ��")]
    public Transform DashEndLeftPoint;
    public Transform DashEndRightPoint;
    public float DashTargetCheckLength = 1f;
    [Range(1f, 50f)] public float DashSpeed = 8f;
    #endregion

    #region BossAttackRandomMoveState_First
    [Header("BossAttackRandomMoveState_First�������")]
    public Transform AttackRandomMoveJumpLeftPostion;      //������λ��
    public Transform AttackRandomMoveJumpRightPostion;
    
    public Transform AttackRandomMoveStartPostionLeftPoint;
    public Transform AttackRandomMoveStartPostionRightPoint;

    public Transform GroundPoint;
    [Range(1f, 10f)] public float DownHurtRange = 2f;
    [Range(10f, 100f)] public float DownDamage = 20f;


    [Range(1f, 100f)] public float AttackRandomMoveUpSpeed = 10f;
    [Range(0f, 2f)] public float UpStayTime = 0.5f;
    [Range(1f, 100f)] public float AttackRandomMoveDownSpeed = 10f;
    #endregion


    #region BossDieState_First
    [Header("BossDieState_First��Boss���н�ֱʱ��")]
    [Range(0f, 2f)] public float DieInvulnerableTime;
    public Transform BossDieTransform;
    public GameObject RewardPrefab;
    public Transform RewardTransform;
    #endregion




    #region private fields
    // ����ʱ�ֶ�
    private bool _isInvulnerable = false;
    private Coroutine _hurtCoroutine;

    public BossState CurrentState => _currentState;
    public float CurrentMoveSpeed
    {
        get { return _currentMoveSpeed; }
        set {  _currentMoveSpeed = value; }
    }

    // ״̬�ֵ�
    private Dictionary<BossState, IBossStateFirstStage> _states;
    private IBossStateFirstStage _currentStateInstance;

    // ������ã�����״̬������
    public Animator Animator { get; private set; }
    public Transform Player { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    public float CurrentHealth { get; set; }
    #endregion


    #region LifeCycle
    void Awake()
    {
        // ��ȡ�������
        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        // ��ʼ��״̬�ֵ�
        _states = new Dictionary<BossState, IBossStateFirstStage>
        {
            { BossState.EatBeans, new BossEatBeansState_First() },
            { BossState.EatBeansRangedAttack, new BossEatBeansRangedAttackState_First() },
            { BossState.Grow, new BossGrowState_First() },


            { BossState.AttackIdle, new BossAttackIdleState_First() },
            { BossState.RangedAttack, new BossRangedAttackState_First() },
            { BossState.DashAttack, new BossDashAttackState_First() },
            { BossState.AttackRandomMove, new BossAttackRandomMoveState_First() },
            { BossState.Hurt, new BossHurtState() },
            { BossState.Die, new BossDieState_First() },
        };
    }

    void Start()
    {
        Segments.Add(this.transform); // ����ͷ����Ϊ��һ��

        //for (int i = 1; i <= StartSnakeSegments; i++)
        //{
        //    Transform segment = Instantiate(SegmentPrefab);
        //    segment.position = Segments[Segments.Count - i].position;

        //    Segments.Add(segment);
        //}
        for (int i = 1; i <= StartSnakeSegments; i++)
        {
            Transform segment = Instantiate(SegmentPrefab);
            // ���ó�ʼλ�ã�ȷ���ؽ�֮�����ʵ�����
            Vector3 spawnPos = Segments[Segments.Count - 1].position - Vector3.up * 9f * i;
            segment.position = spawnPos;

            Segments.Add(segment);
        }

        CurrentHealth = MaxHealth;
        CurrentMoveSpeed = EatBeanMoveSpeed;
        CurrentSegmentNum = Segments.Count;

        // ��ʼ״̬
        ChangeState(startingState);
    }

    void Update()
    {
        _currentStateInstance?.UpdateState();
        _slider.UpdateHealth(CurrentHealth, MaxHealth);
    }

    void FixedUpdate()
    {
        _currentStateInstance?.FixedUpdateState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && _canHurt)
        {
            PlayerHealth.Ins.TakeDamageByEnemy(HitDamage);
            StartCoroutine(HitPlayer());
        }
    }

    private IEnumerator HitPlayer()
    {
        _canHurt = false;
        yield return new WaitForSeconds(_playerInvulnerableTime);
        _canHurt = true;
    }
    #endregion


    #region ChangeState

    // ״̬�л�����
    public void ChangeState(BossState newState)
    {

        // �˳���ǰ״̬
        _currentStateInstance?.ExitState();

        // ��ȡ��״̬ʵ��
        if (_states.TryGetValue(newState, out IBossStateFirstStage nextState))
        {
            _currentStateInstance = nextState;
            _currentState = newState;
            _currentStateName = newState.ToString();


            // ������״̬
            _currentStateInstance.EnterState(this);
        }
    }

    #endregion

    #region Boss Segments Move

    public void SegmentsMove()
    {
        if (Segments == null || Segments.Count < 2) return;

        float segmentSpacing = 4.5f; // �ؽڼ��
        float followSpeed = CurrentMoveSpeed;

        for (int i = 1; i < Segments.Count; i++)
        {
            Transform currentSegment = Segments[i];
            Transform previousSegment = Segments[i - 1];

            Vector2 targetPosition = (Vector2)previousSegment.position -
                                    ((Vector2)(previousSegment.position - currentSegment.position)).normalized * segmentSpacing;

            // ʹ��ƽ���ƶ�
            Vector2 newPosition = Vector2.Lerp(
                currentSegment.position,
                targetPosition,
                followSpeed * Time.fixedDeltaTime
            );

            //����ʹ�� MoveTowards ��ø���ȷ�Ŀ���
            //Vector2 newPosition = Vector2.MoveTowards(
            //    currentSegment.position,
            //    targetPosition,
            //    followSpeed * Time.fixedDeltaTime
            //);


            currentSegment.position = newPosition;

            //// �򵥵�2D��ת
            //Vector2 direction = (Vector2)(previousSegment.position - currentSegment.position);
            //if (direction.sqrMagnitude > 0.01f)
            //{
            //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //    currentSegment.rotation = Quaternion.Euler(0, 0, angle);
            //}
        }
    }

    #endregion

    #region Boss -> Player

    public bool IsPlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, Player.position) <= range;
    }

    public void LookAtPlayer()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    #endregion

    #region Player Take Damage
    // ����ҹ���ʱ����
    public void TakeDamage(float damage)
    {
        if (_currentState == BossState.Die) return;
        if(_isInvulnerable) return;

        CurrentHealth -= damage;

        //if (Animator != null)
        //{
        //    Animator.SetTrigger("Hurt");
        //}

        StartHurtRoutine();

        if (CurrentHealth <= 0)
        {
            ChangeState(BossState.Die);
        }
    }

    private void StartHurtRoutine()
    {
        if(_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
            _hurtCoroutine = null;
        }

        _hurtCoroutine = StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        _isInvulnerable = true;
        bool prevMove = IsMove;
        IsMove = false; // �ܻ�ʱֹͣ�ƶ�����״̬Ӧע�� IsMove��

        yield return new WaitForSeconds(HurtInvulnerableTime);

        IsMove = prevMove;
        _isInvulnerable = false;
        _hurtCoroutine = null;
    }

    #endregion

    #region Animation Events

    // �����¼��ص�
    public void OnAnimationEvent(string eventName)
    {
        _currentStateInstance?.OnAnimationEvent(eventName);
    }

    #endregion
}
