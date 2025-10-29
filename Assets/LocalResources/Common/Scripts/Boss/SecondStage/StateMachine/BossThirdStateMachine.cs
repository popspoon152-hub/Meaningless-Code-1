using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState_Third
{
    // ���ֹ�������״̬
    HoleAttack,          // �ڶ�����
    DevourGround,        // ���ɵ���
    Teleport,            // ����
    DashAttack,          // ��湥��
    SmashAttack,         // ���ҹ���

    // ��������״̬
    AttackIdle,          // ��������
    AttackRandomMove,    // �������͵�����ƶ�

    // ͨ��״̬
    Hurt,                // ����
    Die,                 // ����
}

public class BossThirdStateMachine : MonoBehaviour
{
    #region State Machine Config
    [Header("״̬����")]
    public BossState_Third startingState = BossState_Third.AttackRandomMove;
    public BossSlider Slider;

    [Header("������Ϣ")]
    [SerializeField] private BossState_Third _currentState_Third;
    [SerializeField] private string _currentStateName_Third;

    #endregion

    #region Boss����
    [Header("��������")]
    [Range(10f, 1000f)] public float MaxHealth = 100f;
    //[Range(1f, 20f)] public float AttackMoveSpeed = 5f;
    [SerializeField] private float _currentMoveSpeed_Third;
    [HideInInspector] public bool IsMove = true;

    public Animator TransfromAnim;

    private bool _canHurt = true;

    [Header("�ܻ�����")]
    [Range(0f, 1f)] public float HurtInvulnerableTime = 0.1f;       //�ܻ���Ķ����޵�ʱ��(�����ظ��ж�)
    #endregion

    #region Boss�ƶ�����ײ����
    [Header("Boss�ƶ��ٶ�����")]
    [Range(1f, 40f)] public float MoveSpeed_Normal = 10f;        // ��ͨ�ƶ��ٶ�;���ɵ���Ѱ���ٶ�
    [Range(1f, 40f)] public float MoveSpeed_Attack = 7f;        // �����ƶ��ٶȣ�����AttackRandomMove��
    //[Range(1f, 40f)] public float MoveSpeed_Dash = 12f;         // ��湥���ٶ�
    [Range(1f, 40f)] public float MoveSpeed_ToIdle = 12f;         // ǰ���������ٶ�
    //[Range(1f, 30f)] public float MoveSpeed_Teleport = 10f;     // ���ֻ����ٶȣ���ѡ��

    [Header("��ײ�˺�����")]
    [Range(1f, 100f)] public float CollisionDamage = 10f;        // ��ײ�������ɵ��˺�
    //[Range(0.1f, 5f)] public float CollisionCooldown = 1f;       // ������ײ�˺���ȴʱ��
    private bool _canDealCollisionDamage = true;
    #endregion

    #region BossDashAttackState_Third
    [Header("BossDashAttackState_Third ����")]
    [Range(5f, 50f)] public float DashSpeed = 20f;               // ����ٶ�
    [Range(0.1f, 3f)] public float DashChargeTime = 1f;           // �������ʱ��
    [Range(1f, 70f)] public float DashMaxDistance = 15f;          // �������������
    [Range(0.01f, 1f)] public float DashTrailSpawnInterval = 0.2f; // ·��������ɼ��
    [Range(1f, 40f)] public float DashAlignSpeed = 5f; // ��ֱ�����ٶ�

    [Header("BossDashAttackState_Third �˺�����")]
    [Range(1f, 50f)] public float DashTrailDamagePerTick = 5f;    // �����˺�������˺�
    [Range(1f, 100f)] public float DashImpactDamage = 20f;        // ���ֱ��ײ������˺�

    [Header("�����˺�����")]
    public float ZoneDamage = 10f;           // ÿ����ɵ��˺�
    public float ZoneDamageInterval = 0.5f;  // �˺�Ƶ�ʣ��룩
    public float ZoneDuration = 3f;          // �������ʱ��

    [Header("BossDashAttackState_Third ��Ч����")]
    public GameObject DashTrailPrefab;                            // ·�����Ԥ����
    public LayerMask WallLayerMask;                               // ��̼��ǽ��Ĳ�
    [HideInInspector] public bool IsCharging = false;              // ���ڶ��������ж�
    #endregion


    #region ���ɵ�������
    [Header("Boss���ɵ�������")]
    public GroundTileManager GroundTileManager;          // ���������
    [Range(0f, 5f)] public float DevourPreWarnTime = 1f; // ����ǰҡʱ�䣨Ԥ��ʱ�䣩
    [Range(1f, 50f)] public float DevourSpeed = 10f;     // ����ʱ�ĺ����ƶ��ٶ�
    [Range(1f, 10f)] public float DevourDistance = 5f;   // ���ɵ����Χ����ƽ̨λ�ú���Խ�ľ��룩(δʹ�ã�����ֱ����ĵ�ש����)
    //[Range(0f, 5f)] public float DevourTileRespawnTime = 5f; // ���ɺ�ĵ�ש����ʱ��
    [Range(1f, 30f)] public float DevourSideOffset =5f;   // ����ʱ�ĺ���ƫ����
    #endregion

    #region BossHoleAttackState_Third ����
    [Header("BossHoleAttackState_Third ����")]
    public GameObject HolePrefab;        // �ڶ�Ԥ����
    public float HolePreWarnTime = 1f;   // ǰҡʱ��
    public float HoleSpawnEndDelay = 0.5f; // ���ɺ�ȴ�ʱ��
    #endregion

    #region BossSmashAttackState_Third ����
    [Header("BossSmashAttackState_Third ����")]
    [Range(0f, 2f)] public float SmashPreWarnTime = 1.0f;       // ����ǰҡʱ�䣨Ԥ����
    [Range(1f, 50f)] public float SmashFallSpeed = 5f;           // ���ҵ��ٶ�
    [Range(1f, 50f)] public float SmashRiseSpeed = 5f;           // ����ǰҡ���ٶ�
    [Range(1f, 10f)] public float SmashPostDelay = 0.3f;        // ���Һ��ͣ��ʱ��
    public LayerMask HoleGroundLayerMask;                       // ���ڼ������Ƿ���ڵ�LayerMask
    [Range(0f, 10f)] public float SmashChargeRiseHeight = 2f;       // ǰҡ�����߶�
    public GameObject SmashShockwavePrefab;        // �����Ԥ����
    #endregion

    #region  Smash �����
    [Header("Smash ����� Settings")]
    public float SmashShockwaveSpeed = 10f;       // ������ƶ��ٶ�
    public float SmashShockwaveDamage = 15f;      // ������˺�
    public float SmashShockwaveLifetime = 3f;     // ���������ʱ��
    public LayerMask PlayerLayerMask;             // ��Ҳ㣨���ڼ���˺���
    #endregion





    #region Move
    [Header("Pathfinding")]
    public Pathfinding pathfinding;
    #endregion


    #region BossAttackIdleState_Third
    [Header("BossAttackIdleState_Third��Boss��λ���趨")]
    public Transform IdleLeftTransfrom;
    public Transform IdleRightTransfrom;

    [Header("BossAttackIdleState_Third��Boss�Ľ�ֱʱ��")]
    [Range(1f, 5f)] public float IdleTime = 2f;
    #endregion

    //#region BossRangedAttackState_First
    //[Header("BossRangedAttackState_First���ӵ�")]
    //public GameObject BulletPrefab_Attack;
    //public Transform FirePoint_Attack;
    //[Range(1f, 14f)] public float BulletSpeed_Attack = 5f;

    //[Header("BossRangedAttackState_First��Boss���н�ֱʱ��")]
    //[Range(0f, 2f)] public float RangedAttackInvulnerableTime;
    //#endregion



    #region BossDieState_Third
    [Header("BossDieState_Third��Boss���н�ֱʱ��")]
    [Range(0f, 2f)] public float DieInvulnerableTime;
    #endregion

    #region private fields
    // ����ʱ�ֶ�
    private bool _isInvulnerable = false;
    private Coroutine _hurtCoroutine;

    public BossState_Third CurrentState => _currentState_Third;
    public float CurrentMoveSpeed
    {
        get { return _currentMoveSpeed_Third; }
        set { _currentMoveSpeed_Third = value; }
    }

    // ״̬�ֵ�
    private Dictionary<BossState_Third,IBossStateThirdStage> _states_Third;
    private IBossStateThirdStage _currentStateInstance;

    // ������ã�����״̬����
    public Animator Animator_Third { get; private set; }
    public Transform Player_Third { get; private set; }
    public Rigidbody2D Rb_Third { get; private set; }

    public float CurrentHealth { get; set; }
    #endregion

    #region LifeCycle
    void Awake()
    {
        // ��ȡ�������
        Animator_Third = GetComponent<Animator>();
        Rb_Third = GetComponent<Rigidbody2D>();
        Player_Third = GameObject.FindGameObjectWithTag("Player").transform;
        pathfinding = GetComponent<Pathfinding>();

        // ��ʼ��״̬�ֵ�
        _states_Third = new Dictionary<BossState_Third, IBossStateThirdStage>
        {
            { BossState_Third.DevourGround, new BossDevourGroundState_Third() },
            { BossState_Third.HoleAttack, new BossHoleAttackState_Third() },
            { BossState_Third.Teleport, new BossTeleportState_Third() },
            { BossState_Third.DashAttack, new BossDashAttackState_Third() },
            { BossState_Third.SmashAttack, new BossSmashAttackState_Third() },

            { BossState_Third.AttackIdle, new BossAttackIdleState_Third() },
            { BossState_Third.AttackRandomMove, new BossAttackRandomMoveState_Third() },

            { BossState_Third.Hurt, new BossHurtState_Third() },
            { BossState_Third.Die, new BossDieState_Third() },
        };
    }

    void Start()
    {

        CurrentHealth = MaxHealth;
        CurrentMoveSpeed = MoveSpeed_Normal;


        // ��ʼ״̬
        ChangeState(startingState);
    }

    void Update()
    {
        _currentStateInstance?.UpdateState();
        Slider.UpdateHealth(CurrentHealth, MaxHealth);
    }

    void FixedUpdate()
    {
        _currentStateInstance?.FixedUpdateState();
    }

    #endregion

    #region ChangeState

    // ״̬�л�����
    public void ChangeState(BossState_Third newState)
    {
        // �˳���ǰ״̬
        _currentStateInstance?.ExitState();

        // ��ȡ��״̬ʵ��
        if (_states_Third.TryGetValue(newState, out IBossStateThirdStage nextState))
        {
            _currentStateInstance = nextState;
            _currentState_Third = newState;
            _currentStateName_Third = newState.ToString();

            // ������״̬
            _currentStateInstance.EnterState(this);
        }
        else
        {
            Debug.LogError($"״̬ {newState} δ���ֵ���ע��!");
        }
    }

    #endregion

    #region Attack State Choose
    public void AttackStateChoose()
    {
        //// �������ѡ�񹥻�״̬
        int randomNum = UnityEngine.Random.Range(0, 5);
        if (randomNum == 0)
        {
            ChangeState(BossState_Third.HoleAttack);
        }
        //else if (randomNum == 1)
        //{
        //    ChangeState(BossState_Third.DevourGround);
        //}
        //else if (randomNum == 2)
        //{
        //    ChangeState(BossState_Third.Teleport);
        //}
        else if (randomNum == 3)
        {
            ChangeState(BossState_Third.DashAttack);
        }
        else
        {
            ChangeState(BossState_Third.SmashAttack);
        }
        //ChangeState(BossState_Third.AttackIdle);
    }
    #endregion


    #region ��ײ�˺�����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _canHurt)
        {
            PlayerHealth.Ins.TakeDamageByEnemy(CollisionDamage);
            StartCoroutine(HitPlayer());
        }
    }

    private IEnumerator HitPlayer()
    {
        _canHurt = false;
        yield return new WaitForSeconds(2f);
        _canHurt = true;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        TryDealCollisionDamage(collision.gameObject);
    //    }
    //}

    private void TryDealCollisionDamage(GameObject player)
    {
        if (!_canDealCollisionDamage) return;

        PlayerHealth.Ins.TakeDamageByEnemy(CollisionDamage);

        if (_currentState_Third == BossState_Third.AttackRandomMove &&_currentStateInstance is BossAttackRandomMoveState_Third moveState)
        {
            moveState.OnBossHitPlayer();//����ƶ�ײ��ֹͣ
        }

        // ������ײ��ȴ����ֹ��������
        //StartCoroutine(CollisionCooldownRoutine());
    }

    //private IEnumerator CollisionCooldownRoutine()
    //{
    //    _canDealCollisionDamage = false;
    //    yield return new WaitForSeconds(CollisionCooldown);
    //    _canDealCollisionDamage = true;
    //}

    #endregion



    #region Boss -> Player

    public bool IsPlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, Player_Third.position) <= range;
    }

    public void LookAtPlayer()
    {
        Vector2 direction = (Player_Third.position - transform.position).normalized;
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
        if (_currentState_Third == BossState_Third.Die) return;
        if (_isInvulnerable) return;

        CurrentHealth -= damage;
        Debug.Log("Successfully hurt");

        //if (Animator != null)
        //{
        //    Animator.SetTrigger("Hurt");
        //}

        StartHurtRoutine();

        if (CurrentHealth <= 0)
        {
            ChangeState(BossState_Third.Die);
        }
    }

    private void StartHurtRoutine()
    {
        if (_hurtCoroutine != null)
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
