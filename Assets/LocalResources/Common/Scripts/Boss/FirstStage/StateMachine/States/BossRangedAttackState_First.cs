using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRangedAttackState_First : IBossStateFirstStage
{
    private BossFirstStateMachine _stateMachine;

    private Coroutine _RangedAttack;
    private Transform _playerPos;

    // ����״̬ʱ���ã���ʼ����
    public void EnterState(BossFirstStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        //if (_stateMachine.Animator != null)
        //{
        //    _stateMachine.Animator.SetTrigger("RangedAttack");
        //}

        if (_playerPos == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                _playerPos = playerObject.transform;
        }

        _RangedAttack = _stateMachine.StartCoroutine(RangedAttack());
    }

    #region Attack
    
    private IEnumerator RangedAttack()
    {
        if (_stateMachine.BulletPrefab_Attack != null && _stateMachine.FirePoint_Attack != null && _playerPos != null)
        {
            _stateMachine.IsMove = false;

            GameObject bullet = UnityEngine.Object.Instantiate(_stateMachine.BulletPrefab_Attack, _stateMachine.FirePoint_Attack.position, _stateMachine.FirePoint_Attack.rotation);
            
            Vector2 direction = (_playerPos.position - _stateMachine.FirePoint_Attack.position).normalized;

            if (bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = direction * _stateMachine.BulletSpeed_Attack;
            }



            for (int i = 1; i < 4; i++)
            {
                float angle = 0;
                if(_stateMachine.transform.position.x > 0)
                {
                    angle = -i * 20;
                }
                else
                {
                    // ����Ƕ�
                    angle = i * 20;
                }


                // ������ת
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                GameObject littleBullet = UnityEngine.Object.Instantiate(
                    _stateMachine.LittleBulletPrefab_Attack,
                    _stateMachine.FirePoint_Attack.position,
                    rotation
                );

                if (littleBullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D r))
                {
                    // ������ת���㷽��
                    if (littleBullet.transform.position.x > 0)
                    {
                        Vector2 littleBulletDirection = rotation * Vector2.left;
                        r.velocity = littleBulletDirection * _stateMachine.LittleBulletSpeed_Attack;
                    }
                    else
                    {
                        Vector2 littleBulletDirection = rotation * Vector2.right;
                        r.velocity = littleBulletDirection * _stateMachine.LittleBulletSpeed_Attack;
                    }

                }
            }

            _stateMachine.IsMove = true;
        }

        yield return new WaitForSeconds(_stateMachine.RangedAttackInvulnerableTime);



        //Attack State Choose
        if (UnityEngine.Object.FindObjectsOfType<Bean>() != null)
        {
            _stateMachine.ChangeState(BossState.EatBeans);
        }
        else
        {
            //��Ϊ��湥�����������µ�����ƶ�����
            int randonNum = UnityEngine.Random.Range(0, 2);
            if (randonNum == 0)
            {
                _stateMachine.ChangeState(BossState.DashAttack);
            }
            else if (randonNum == 1)
            {
                _stateMachine.ChangeState(BossState.AttackRandomMove);
            }
        }

    }
    #endregion







    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {
        if (_stateMachine == null) return;

        //�����ƶ�
        if (_stateMachine.IsMove) _stateMachine.SegmentsMove();
    }

    // �˳�״̬ʱ���ã�����
    public void ExitState()
    {
        if (_RangedAttack != null)
        {
            _stateMachine.StopCoroutine(_RangedAttack);
            _RangedAttack = null;
        }
        _stateMachine = null;
    }















    // ÿ֡����
    public void UpdateState()
    {

    }

    // �������¼�
    public void OnAnimationEvent(string eventName)
    {

    }
}
