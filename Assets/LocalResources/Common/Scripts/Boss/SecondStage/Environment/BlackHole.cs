using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlackHole : MonoBehaviour
{
    [Header("�ڶ���������Ԥ�����趨��")]
    public float Duration = 5f;             // �ڶ�����ʱ��
    public float TickInterval = 0.5f;       // ����Ӱ���������˺��ļ��
    public float DamagePerTick = 5f;        // ÿ���˺�
    public float PullStrength = 5f;         // ����ǿ��
    public float Radius = 2f;               // ������Χ�뾶

    [Header("Ӱ�����ų�����")]
    //public GameObject Owner;                // �����ߣ�����Ӱ�죩
    public LayerMask AffectedLayers;        //  ֻ����Щ��Ķ���Żᱻ����Ӱ��

    private float _lifeTimer;
    private float _tickTimer;
    private HashSet<Rigidbody2D> _insideBodies = new HashSet<Rigidbody2D>();
    private HashSet<Collider2D> _insideColliders = new HashSet<Collider2D>();
    private Collider2D _collider2d;

    void Awake()
    {
        _collider2d = GetComponent<Collider2D>();
        _collider2d.isTrigger = true;

        // ��ͬ���뾶�����ʹ�� CircleCollider2D��
        CircleCollider2D circle = _collider2d as CircleCollider2D;
        if (circle != null)
        {
            circle.radius = Radius;
        }
    }

    void Start()
    {
        _lifeTimer = 0f;
        _tickTimer = 0f;
    }

    void Update()
    {
        _lifeTimer += Time.deltaTime;
        _tickTimer += Time.deltaTime;

        // ��ʱ�Է�Χ�ڶ�������˺�
        if (_tickTimer >= TickInterval)
        {
            _tickTimer = 0f;
            foreach (var col in _insideColliders)
            {
                if (col == null) continue;
                if (((1 << col.gameObject.layer) & AffectedLayers) == 0) continue; // ֻ��ָ������Ч

                if (col.CompareTag("Player"))
                {
                    PlayerHealth.Ins.TakeDamageByEnemy(DamagePerTick);
                }
            }
        }

        // ����ʱ�䵽����ٺڶ�
        if (_lifeTimer >= Duration)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // �Է�Χ�ڸ���ʩ������
        Vector2 center = transform.position;
        foreach (var rb in _insideBodies)
        {
            if (rb == null) continue;
            GameObject obj = rb.gameObject;

            // ֻӰ��ָ�� Layer
            if (((1 << obj.layer) & AffectedLayers) == 0)
                continue;

            //// ��������������
            //if (Owner != null && obj == Owner)
            //    continue;

            Vector2 dir = (center - rb.position);
            float dist = dir.magnitude;
            if (dist <= 0.01f) continue;
            dir.Normalize();

            rb.AddForce(dir * PullStrength, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ����ˣ����������Ӱ��㣬����ӽ��б�
        if (((1 << other.gameObject.layer) & AffectedLayers) == 0)
            return;

        if (!_insideColliders.Contains(other))
            _insideColliders.Add(other);

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null && !_insideBodies.Contains(rb))
            _insideBodies.Add(rb);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_insideColliders.Contains(other))
            _insideColliders.Remove(other);

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null && _insideBodies.Contains(rb))
            _insideBodies.Remove(rb);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0f, 0.8f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
