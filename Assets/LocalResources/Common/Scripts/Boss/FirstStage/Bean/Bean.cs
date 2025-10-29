using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bean : MonoBehaviour
{
    [Range(1, 8)] public int BeanHealth = 5;            //��������ֵ
    [SerializeField] private int _currentHealth;
    public bool IsDestroy = false;

    public Animator Anim;
    public Collider2D Col;

    private void Start()
    {
        _currentHealth = BeanHealth;
    }

    private void Update()
    {
        //������

        if (_currentHealth <= 0)
        {
            //
            StartCoroutine(BeanDestroy());

        }
    }

    private IEnumerator BeanDestroy()
    {
        Anim.SetTrigger("Dead");
        Col.enabled = false;
        
        yield return new WaitForSeconds(1f);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if(_currentHealth <= 0)
        {
            PlayerHealth.Ins.Health(10);
        }
        EnvironmentAndMap environment = FindObjectOfType<EnvironmentAndMap>();
        if (environment != null)
        {
            environment.OnBeanEaten();
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
    }

    public void BeEat()
    {
        Destroy(this.gameObject);
    }
}
