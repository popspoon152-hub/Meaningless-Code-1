using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStageBullet : MonoBehaviour
{
    public float Damage = 10f;                                  //���ӵ�����˺� 

    public float AliveTime = 15f;                                //�ӵ����ʱ��

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth.Ins.TakeDamageByEnemy(Damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        AliveTime -= Time.deltaTime;
        if (AliveTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
