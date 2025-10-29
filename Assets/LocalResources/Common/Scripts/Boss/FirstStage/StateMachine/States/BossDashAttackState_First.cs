using System.Collections;
using UnityEngine;

public class BossDashAttackState_First : IBossStateFirstStage
{
    private BossFirstStateMachine _stateMachine;

    private Transform _chooseTrans;
    private bool _isLeft;
    private Transform _targetTrans;

    private Coroutine _dashAttack;

    public void EnterState(BossFirstStateMachine stateMachine)
    {
        _stateMachine = stateMachine;


        for (int i = 0; i < _stateMachine.Segments.Count; i++)
        {
            Rigidbody2D rb = _stateMachine.Segments[i].GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }
        DashAttack();
    }

    #region Dash
    private void DashAttack()
    {
        if (_stateMachine == null) return;

        _stateMachine.IsMove = false;
        _stateMachine.CurrentMoveSpeed = _stateMachine.DashSpeed;

        _chooseTrans = SelectPoint();

        if (TelePort(_chooseTrans.position))
        {
            _stateMachine.IsMove = true;

            // ���Ŀ����Ƿ���Ч
            if (_isLeft)
            {
                _targetTrans = _stateMachine.DashEndRightPoint;
            }
            else
            {
                _targetTrans = _stateMachine.DashEndLeftPoint;
            }

            _dashAttack = _stateMachine.StartCoroutine(Dash());
        }
        else
        {
            ChangeToNextState();
        }
    }

    private Transform SelectPoint()
    {
        if (_stateMachine == null) return null;

        int num = UnityEngine.Random.Range(0, 2);
        if (num == 0)
        {
            _isLeft = true;
            return _stateMachine.TelePortLeftPoint;
        }
        else
        {
            _isLeft = false;
            return _stateMachine.TelePortRightPoint;
        }
    }

    private bool TelePort(Vector2 targetPosition)
    {
        if (_stateMachine == null) return false;

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

    private IEnumerator Dash()
    {
        while (true)
        {
            _stateMachine.transform.position = Vector3.MoveTowards(
                _stateMachine.transform.position,
                _targetTrans.position,
                _stateMachine.CurrentMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    #endregion

    // �˳�״̬ʱ���ã�����
    public void ExitState()
    {
        if (_dashAttack != null && _stateMachine != null)
        {
            _stateMachine.StopCoroutine(_dashAttack);
            _dashAttack = null;
        }
        _stateMachine = null;
    }

    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {
        if (_stateMachine == null) return;

        // �����ƶ�
        if (_stateMachine.IsMove) _stateMachine.SegmentsMove();
    }

    // ÿ֡����
    public void UpdateState()
    {
        if (_stateMachine == null || _targetTrans == null) return;

        float distToTarget = Vector2.Distance(_stateMachine.transform.position, _targetTrans.position);
        if (distToTarget <= _stateMachine.DashTargetCheckLength)
        {
            ChangeToNextState();
        }
    }

    // ��ȡ״̬�л��߼�����������
    private void ChangeToNextState()
    {
        if (_stateMachine == null) return;

        Bean[] beans = UnityEngine.Object.FindObjectsOfType<Bean>();
        if (beans != null && beans.Length > 0)
        {
            _stateMachine.ChangeState(BossState.EatBeans);
        }
        else
        {
            // ��Ϊվ������湥�����������µ�����ƶ�����
            int randonNum = UnityEngine.Random.Range(0, 3);
            if (randonNum == 0)
            {
                _stateMachine.ChangeState(BossState.AttackIdle);
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

    // �������¼�
    public void OnAnimationEvent(string eventName)
    {
        // �������¼�
    }
}
