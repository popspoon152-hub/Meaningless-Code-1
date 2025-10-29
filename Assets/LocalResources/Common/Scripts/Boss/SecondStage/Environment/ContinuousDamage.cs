using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ContinuousDamage : MonoBehaviour
{
    private Vector2 _moveDir;
    private float _moveSpeed;
    private float _damage;
    private float _lifeTime;
    private float _damageInterval = 0.5f;

    private BossThirdStateMachine _boss;
    private Collider2D _collider;
    private bool _isActive = false;

    private LayerMask _playerLayer;
    private Coroutine _damageRoutine;
  

    public void Initialize(Vector2 dir)
    {
        _moveDir = dir.normalized;
        _isActive = true;

        // �ɵ�����ʼ��������������Ч
        // e.g. Animator.SetTrigger("Activate");
    }

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;

        // �Զ�����Boss����
        _boss = FindObjectOfType<BossThirdStateMachine>();
        if (_boss == null)
        {
            Debug.LogError("[Shockwave] Could not find BossThirdStateMachine in scene!");
            Destroy(gameObject);
            return;
        }

        // ��״̬����ȡ����
        _moveSpeed = _boss.SmashShockwaveSpeed;
        _damage = _boss.DashImpactDamage;
        _lifeTime = _boss.SmashShockwaveLifetime;
        _playerLayer = _boss.PlayerLayerMask;

        // �����������ڼ�ʱ
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        //if (!_isActive) return;
        //transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            PlayerHealth.Ins.TakeDamageByEnemy(_damage);
            Destroy(this.gameObject);
        }
    }

}
