using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHurt : MonoBehaviour
{
    [Header("LaserHurt")]
    [Range(0f, 20f)] public float damage;

    //private Transform owner;

    //private void Awake()
    //{
    //    // �ҵ�����ʱ�ĸ����󣨼���ң�
    //    owner = transform.parent;
    //    // ��������ɺ���������븸���壬���� Start �н�����Թ�ϵ
    //    IgnoreOwnerCollisions();
    //}

    //private void IgnoreOwnerCollisions()
    //{
    //    if (owner == null) return;

    //    Collider2D myCol = GetComponent<Collider2D>();
    //    if (myCol == null) return;

    //    // �����븸���弰���������������ײ
    //    foreach (var c in owner.GetComponentsInChildren<Collider2D>())
    //    {
    //        Physics2D.IgnoreCollision(myCol, c);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            if (other.gameObject.GetComponent<BossFirstStateMachine>() != null)
            {
                other.gameObject.GetComponent<BossFirstStateMachine>().TakeDamage(damage);
            }
            if(other.gameObject.GetComponent<BossThirdStateMachine>() != null)
            {
                other.gameObject.GetComponent<BossThirdStateMachine>().TakeDamage(damage);
            }
            
        }
    }

}
