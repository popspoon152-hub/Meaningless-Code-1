using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossAttackRandomMoveState_First : IBossStateFirstStage
{
    private BossFirstStateMachine _stateMachine;

    private Coroutine _attackRandomMove;
    private int targetIndex;
    private Vector2 _chooseJumpPos;
    private Transform _chooseTrans;

    private bool _isLeft;

    // ����״̬ʱ���ã���ʼ����
    public void EnterState(BossFirstStateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
        //if (_stateMachine.Animator != null)
        //{
        //    _stateMachine.Animator.SetTrigger("AttackRandomMove");
        //}

        for (int i = 0; i < _stateMachine.Segments.Count; i++)
        {
            Rigidbody2D rb = _stateMachine.Segments[i].GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }
        RandomMoveAttack();
    }

    private void RandomMoveAttack()
    {
        _stateMachine.IsMove = false;
        _stateMachine.CurrentMoveSpeed = _stateMachine.AttackRandomMoveUpSpeed;

        // ����Ҫ��Transform����
        if (_stateMachine.AttackRandomMoveJumpLeftPostion == null ||
            _stateMachine.AttackRandomMoveJumpRightPostion == null)
        {
            Debug.LogError("AttackRandomMoveJump positions are not assigned!");
        }

        _isLeft = _stateMachine.transform.position.x <= _stateMachine.AttackRandomMoveJumpLeftPostion.position.x;

        SelectPoint();
        if (TelePort(_chooseTrans.position))
        {
            _stateMachine.IsMove = true;
            _attackRandomMove = _stateMachine.StartCoroutine(AttackRandomMove());
        }
    }


    #region AttackRandomMove
    private void SelectPoint()
    {
        float randomX = UnityEngine.Random.Range(_stateMachine.AttackRandomMoveJumpLeftPostion.position.x, _stateMachine.AttackRandomMoveJumpRightPostion.position.x);
        _chooseJumpPos = new Vector2(randomX, _stateMachine.AttackRandomMoveJumpLeftPostion.position.y);

        if (_isLeft)
        {
            _chooseTrans = _stateMachine.AttackRandomMoveStartPostionLeftPoint;
        }
        else
        {
            _chooseTrans = _stateMachine.AttackRandomMoveStartPostionRightPoint;
        }
    }

    private bool TelePort(Vector2 position)
    {
        _stateMachine.transform.position = position;

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

    private IEnumerator AttackRandomMove()
    {
        while (true)
        {
            _stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position, _chooseJumpPos, _stateMachine.CurrentMoveSpeed * Time.deltaTime);

            yield return null;

            float distToTarget = Vector2.Distance(_stateMachine.transform.position, _chooseJumpPos);
            if (distToTarget <= _stateMachine.DashTargetCheckLength)
            {
                break;
            }
        }

        yield return new WaitForSeconds(_stateMachine.UpStayTime);

        Vector2 groundTarget = new Vector2(_stateMachine.transform.position.x, _stateMachine.GroundPoint.position.y);

        _stateMachine.CurrentMoveSpeed = _stateMachine.AttackRandomMoveDownSpeed;

        while (true)
        {
            _stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position, groundTarget, _stateMachine.CurrentMoveSpeed * Time.deltaTime);

            yield return null;

            float distToGround = Vector2.Distance(_stateMachine.transform.position, groundTarget);
            if (distToGround <= _stateMachine.DashTargetCheckLength)
            {
                Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(_stateMachine.transform.position, _stateMachine.DownHurtRange, _stateMachine.PlayerLayer);

                if (hitPlayer != null && hitPlayer.Length > 0)
                {
                    // ȷ��PlayerHealthʵ������
                    if (PlayerHealth.Ins != null)
                    {
                        PlayerHealth.Ins.TakeDamageByEnemy(_stateMachine.DownDamage);
                    }
                    else
                    {
                        Debug.LogWarning("PlayerHealth instance is null!");
                    }
                }

                yield return new WaitForSeconds(1f);

                if (UnityEngine.Object.FindObjectsOfType<Bean>() != null)
                {
                    _stateMachine.ChangeState(BossState.EatBeans);
                }
                else
                {
                    //��Ϊվ��,��湥�����������µ�����ƶ�����
                    int randonNum = UnityEngine.Random.Range(0, 3);
                    if (randonNum == 0)
                    {
                        _stateMachine.ChangeState(BossState.DashAttack);
                    }
                    else if (randonNum == 1)
                    {
                        _stateMachine.ChangeState(BossState.AttackRandomMove);
                    }
                    else
                    {
                        _stateMachine.ChangeState(BossState.AttackIdle);
                    }
                }
                yield break;
            }
        }
        //_stateMachine.transform.position = Vector3.down * _stateMachine.CurrentMoveSpeed * Time.deltaTime;
    }

    #endregion

    // ÿ֡����
    public void UpdateState()
    {
        
    }

    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {
        if (_stateMachine == null) return;

        //�����ƶ�
        if (_stateMachine.IsMove) _stateMachine.SegmentsMove();
    }

    // �˳�״̬ʱ���ã�������
    public void ExitState()
    {
        if (_attackRandomMove != null)
        {
            _stateMachine.StopCoroutine(_attackRandomMove);
            _attackRandomMove = null;
        }
        _stateMachine = null;
    }














    // ���������¼�
    public void OnAnimationEvent(string eventName)
    {

    }
}
