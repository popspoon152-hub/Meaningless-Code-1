using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BossAttackIdleState_First : IBossStateFirstStage
{
    private BossFirstStateMachine _stateMachine;
    private Coroutine _AttackIdle;
    private Transform _chooseTrans;
    private bool _isLeft;

    // ����״̬ʱ���ã���ʼ����
    public void EnterState(BossFirstStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        //if(_stateMachine.Animator != null)
        //{
        //    _stateMachine.Animator.SetTrigger("AttackIdle");
        //}
        
        _stateMachine.IsMove = false;
        for(int i = 0; i < _stateMachine.Segments.Count; i++)
        {
            Rigidbody2D rb = _stateMachine.Segments[i].GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }

        _AttackIdle = _stateMachine.StartCoroutine(AttackIdle());
    }

    #region Idle
    private void ChooseBossPos()
    {
        int num = UnityEngine.Random.Range(0, 2);
        if (num == 0)
        {
            _isLeft = true;
            _chooseTrans = _stateMachine.IdleLeftTransfrom;
        }
        else 
        {
            _isLeft = false;
            _chooseTrans = _stateMachine.IdleRightTransfrom;
        }
    }

    private IEnumerator AttackIdle()
    {
        ChooseBossPos();

        _stateMachine.IsMove = false;
        //˲�ƻ��ƶ�
        if (TelePort(_chooseTrans.position))
        {
            yield return new WaitForSeconds(_stateMachine.IdleTime);

            // ����Ƿ��ж��Ӵ���
            Bean[] beans = UnityEngine.Object.FindObjectsOfType<Bean>();
            if (beans != null && beans.Length > 0)
            {
                _stateMachine.ChangeState(BossState.EatBeans);
            }
            else
            {
                //��ΪԶ�̹�������湥�����������µ�����ƶ�����
                int randonNum = UnityEngine.Random.Range(0, 3);
                if (randonNum == 0)
                {
                    _stateMachine.ChangeState(BossState.RangedAttack);
                }
                else if (randonNum == 1)
                {
                    _stateMachine.ChangeState(BossState.DashAttack);
                }
                else
                {
                    _stateMachine.ChangeState(BossState.AttackRandomMove);
                }
            }
        }
        else
        {
            // ˲��ʧ�ܣ�ֱ��ѡ�񹥻�״̬

            //��ΪԶ�̹�������湥�����������µ�����ƶ�����
            int randonNum = UnityEngine.Random.Range(0, 3);
            if (randonNum == 0)
            {
                _stateMachine.ChangeState(BossState.RangedAttack);
            }
            else if (randonNum == 1)
            {
                _stateMachine.ChangeState(BossState.DashAttack);
            }
            else
            {
                _stateMachine.ChangeState(BossState.AttackRandomMove);
            }
        }
    }

    private bool TelePort(Vector2 targetPosition)
    {
        _stateMachine.transform.position = targetPosition;

        Rigidbody2D rb = _stateMachine.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        return SetupSegmentsPosition();
    }

    private bool SetupSegmentsPosition()
    {
        for (int i = 1; i < _stateMachine.Segments.Count; i++)
        {
            // ʹ�����λ��
            Vector3 segmentOffset = _isLeft ?
                new Vector3(-i * 4.5f, 0, 0) :
                new Vector3(i * 4.5f, 0, 0);

            _stateMachine.Segments[i].position = _stateMachine.transform.position + segmentOffset;
        }

        return true;
    }
    #endregion



    // �˳�״̬ʱ���ã�����
    public void ExitState()
    {
        if (_AttackIdle != null)
        {
            _stateMachine.StopCoroutine(_AttackIdle);
            _AttackIdle = null;
        }
        _stateMachine.IsMove = true;
        _stateMachine = null;
    }













    // ÿ֡����
    public void UpdateState()
    {

    }

    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {

    }

    // �������¼�
    public void OnAnimationEvent(string eventName)
    {

    }
}