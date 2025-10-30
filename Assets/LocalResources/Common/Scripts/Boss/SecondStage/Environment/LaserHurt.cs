using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHurt : MonoBehaviour
{
    [Header("LaserHurt")]
    [Range(0f, 20f)] public float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            if (other.gameObject.GetComponent<BossFirstStateMachine>() != null)
            {
                other.gameObject.GetComponent<BossFirstStateMachine>().TakeDamage(damage);
                PlayerHealth.Ins.Health(damage);
            }
            if(other.gameObject.GetComponent<BossThirdStateMachine>() != null)
            {
                other.gameObject.GetComponent<BossThirdStateMachine>().TakeDamage(damage);
                PlayerHealth.Ins.Health(damage);
            }
            
        }
    }

}
