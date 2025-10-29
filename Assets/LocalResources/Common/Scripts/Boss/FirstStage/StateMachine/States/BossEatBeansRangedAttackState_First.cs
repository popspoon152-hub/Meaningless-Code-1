using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEatBeansRangedAttackState_First : IBossStateFirstStage
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
        //    _stateMachine.Animator.SetTrigger("EatBeansRangedAttack");
        //}

        if (_playerPos == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                _playerPos = playerObject.transform;
        }

        _RangedAttack = _stateMachine.StartCoroutine(RangedAttack());

        _stateMachine.IsMove = true;
    }

    #region Attack
    private IEnumerator RangedAttack()
    {
        if(_stateMachine.BulletPrefab == null || _stateMachine.FirePoint == null || _playerPos == null)
        {
            Debug.LogWarning("Bullet Prefab, Fire Point, or Player Position is not assigned.");

            _stateMachine.ChangeState(BossState.EatBeans);
        }
        else if(_stateMachine.BulletPrefab != null && _stateMachine.FirePoint != null && _playerPos != null)
        {
            _stateMachine.IsMove = false;

            GameObject bullet = UnityEngine.Object.Instantiate(_stateMachine.BulletPrefab, _stateMachine.FirePoint.position, _stateMachine.FirePoint.rotation);

            Vector2 direction = (_playerPos.position - _stateMachine.FirePoint.position).normalized;

            if(bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = direction * _stateMachine.BulletSpeed;
            }

            _stateMachine.IsMove = true;
        }

        yield return new WaitForSeconds(_stateMachine.EatBeansRangedAttackInvulnerableTime);

        _stateMachine.ChangeState(BossState.EatBeans);
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
        if(_RangedAttack != null)
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
