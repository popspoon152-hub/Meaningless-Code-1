using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class RewardBean : MonoBehaviour
{
    [Range(1, 8)] public int BeanHealth = 5;            //��������ֵ
    [SerializeField] private int _currentHealth;
    public bool IsDestroy = false;
    public Animator Anim;
    public Animator transition;
    public float transitionTime = 1f;

    private void Start()
    {
        _currentHealth = BeanHealth;
        Anim = gameObject.GetComponent<Animator>();
        transition = GameObject.Find("LevelLoaderImage").GetComponent<Animator>();
    }

    private void Update()
    {
        //������

        if (_currentHealth <= 0)
        {
            //������
            StartCoroutine(Dead());

        }
    }

    private IEnumerator Dead()
    {
        Anim.SetTrigger("Dead");
        yield return new WaitForSeconds(3f);

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("GalAfterFirstStage");
    }

    private void OnDestroy()
    {

    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
    }
}
