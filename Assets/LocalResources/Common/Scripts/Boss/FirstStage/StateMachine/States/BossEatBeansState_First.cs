using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 83��д˲��
/// </summary>

public class BossEatBeansState_First : IBossStateFirstStage
{
    private BossFirstStateMachine _stateMachine;

    private Bean _targetBean;
    private float NoBeansWaitTime = 0.2f;                             // û�ж���ʱ�ĵȴ�ʱ��
    private List<Vector3> _path;
    private int targetIndex;

    private Coroutine _noBeansCoroutine;
    private Coroutine _followPath;

    // ����״̬ʱ���ã���ʼ����
    public void EnterState(BossFirstStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _stateMachine.CurrentMoveSpeed = _stateMachine.EatBeanMoveSpeed;
        _stateMachine.IsMove = true;

        targetIndex = 0;
        _path = null;

        if (GridManager.Ins != null)
        {
            GridManager.Ins.CreateGrid();
        }

        EatBeansMove();
    }

    private void EatBeansMove()
    {
        SelectNextTarget();
        MoveToTarget();
    }

    #region FindPath & MoveToTarget

    /// <summary>
    /// �ҵ�_targetBean
    /// </summary>
    private void SelectNextTarget()
    {
        if (_noBeansCoroutine != null)
        {
            _stateMachine.StopCoroutine(_noBeansCoroutine);
            _noBeansCoroutine = null;
        }

        Bean[] beans = UnityEngine.Object.FindObjectsOfType<Bean>();

        if (beans == null || beans.Length == 0)
        {
            // ����δ���еȴ�Э�̣��������ȴ����Ѿ��ڵȴ�����ֱ�ӷ���
            if (_noBeansCoroutine == null)
            {
                _noBeansCoroutine = _stateMachine.StartCoroutine(NoBeansWaitCoroutine());
            }
            return;
        }

        Bean closestBean = null;
        float closestDistance = float.MaxValue;
        Vector3 currentPos = _stateMachine.transform.position;

        int validBeanCount = 0;
        foreach (Bean bean in beans)
        {
            validBeanCount++;
            float distance = Vector3.Distance(currentPos, bean.transform.position);
            if (distance < closestDistance)
            {
                closestBean = bean;
                closestDistance = distance;
            }
        }

        _targetBean = closestBean;

        // �������û�ҵ���Ч����
        if (_targetBean == null && _noBeansCoroutine == null)
        {
            _noBeansCoroutine = _stateMachine.StartCoroutine(NoBeansWaitCoroutine());
        }

        //if (_targetBean != null)
        //{
        //    Debug.Log($"ѡ���˶��ӣ�λ��: {_targetBean.transform.position}, ����: {Vector3.Distance(_stateMachine.transform.position, _targetBean.transform.position)}");
        //}
        //else
        //{
        //    Debug.Log("û���ҵ���Ч�Ķ���");
        //}
    }

    /// <summary>
    /// �Ҳ�������ʱ�ĵȴ�Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator NoBeansWaitCoroutine()
    {
        yield return new WaitForSeconds(NoBeansWaitTime);

        // �ȴ��������ٴμ�鳡���Ƿ��ж���
        Bean[] beansAfter = UnityEngine.Object.FindObjectsOfType<Bean>();
        _noBeansCoroutine = null;

        if (beansAfter == null || beansAfter.Length == 0)
        {
            _stateMachine.ChangeState(BossState.AttackIdle);
        }
        else
        {
            // �ж���������ѡĿ��
            SelectNextTarget();
            if (_targetBean != null)
            {
                MoveToTarget();
            }
        }
    }

    /// <summary>
    /// �ƶ���Ŀ�궹��
    /// </summary>
    /// <exception></exception>
    private void MoveToTarget()
    {
        if (_targetBean == null || _targetBean.gameObject == null || !_targetBean.gameObject.activeInHierarchy)
        {
            SelectNextTarget();
            return;
        }

        if (_followPath != null)
        {
            _stateMachine.StopCoroutine(_followPath);
            _followPath = null;
        }

        if (Pathfinding.Ins == null)
        {
            //Debug.LogError("Pathfinding instance is null!");
            // ·�����Ҳ����ã�ֱ�������ƶ�
            _followPath = _stateMachine.StartCoroutine(MoveDirectlyToBean());
            return;
        }

        _path = Pathfinding.Ins.FindPath(_stateMachine.transform.position, _targetBean.transform.position);

        if (_path != null && _path.Count > 0)
        {
            targetIndex = 0;
            _followPath = _stateMachine.StartCoroutine(FollowPath());
        }
        else
        {
            //Debug.LogWarning("·������ʧ�ܣ�ֱ�������ƶ�");
            _followPath = _stateMachine.StartCoroutine(MoveDirectlyToBean());
        }
    }

    private IEnumerator FollowPath()
    {
        if (_path == null || _path.Count == 0)
        {
            //Debug.LogWarning("·��Ϊ�գ��޷�����");
            yield break;
        }

        Vector3 currentWaypoint = _path[0];
        targetIndex = 0;

        while (true)
        {

            // ·���㵽����
            if (Vector3.Distance(_stateMachine.transform.position, currentWaypoint) <= 0.1f)
            {
                targetIndex++;
                if (targetIndex >= _path.Count)
                {
                    // ·�������ֱ�������ƶ���ֱ���Ե�����
                    yield return _stateMachine.StartCoroutine(MoveDirectlyToBean());
                    yield break;
                }
                currentWaypoint = _path[targetIndex];
            }

            // �ƶ�
            _stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position, currentWaypoint, _stateMachine.CurrentMoveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator MoveDirectlyToBean()
    {
        if (_targetBean == null) yield break;

        while (_targetBean != null && _targetBean.gameObject != null && _targetBean.gameObject.activeInHierarchy)
        {
            float distToBean = GetDistanceToBean();

            if (distToBean <= _stateMachine.EatDistance)
            {
                EatBean();
                break;
            }

            // ֱ������λ���ƶ�
            _stateMachine.transform.position = Vector3.MoveTowards(
                _stateMachine.transform.position,
                _targetBean.transform.position,
                _stateMachine.CurrentMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    // ͳһ�ľ�����㷽��
    private float GetDistanceToBean()
    {
        if (_targetBean == null) return float.MaxValue;

        return Vector2.Distance(
            new Vector2(_stateMachine.transform.position.x, _stateMachine.transform.position.y),
            new Vector2(_targetBean.transform.position.x, _targetBean.transform.position.y)
        );
    }
    #endregion

    public void UpdateState()
    {
        if (_stateMachine == null) return;

        if (_targetBean == null && _noBeansCoroutine == null)
        {
            EatBeansMove();
            return;
        }


        if (_targetBean != null)
        {
            if (_targetBean.gameObject == null || !_targetBean.gameObject.activeInHierarchy)
            {
                _targetBean = null;

                // ֹͣ�ƶ�
                if (_followPath != null)
                {
                    _stateMachine.StopCoroutine(_followPath);
                    _followPath = null;
                }

                EatBeansMove();
                return;
            }

            // �Զ���� - ֻ��û��·������ʱ��飨·����������ڲ������Զ���
            //if (_followPath == null)
            //{
            //    float distToBean = GetDistanceToBean();

            //    if (distToBean <= _stateMachine.EatDistance)
            //    {
            //        EatBean();
            //    }
            //    else
            //    {
            //        MoveToTarget();
            //    }
            //}
        }
    }

    // ͳһ�ĳԶ�����
    private void EatBean()
    {
        if (_followPath != null)
        {
            _stateMachine.StopCoroutine(_followPath);
            _followPath = null;
        }

        if (_targetBean != null && _targetBean.gameObject != null)
        {
            try
            {
                Bean bean = _targetBean.GetComponent<Bean>();
                if (bean != null)
                {
                    bean.BeEat();
                }
                else
                {
                    UnityEngine.Object.Destroy(_targetBean.gameObject);
                    _stateMachine.ChangeState(BossState.Grow);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"��������ʧ��: {e.Message}");
                UnityEngine.Object.Destroy(_targetBean.gameObject);
            }
        }

        _targetBean = null;

        // �л�״̬
        _stateMachine.ChangeState(BossState.Grow);
    }

    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {
        if (_stateMachine == null) return;

        if (_stateMachine.IsMove) _stateMachine.SegmentsMove();
    }

    // �˳�״̬ʱ���ã�������
    public void ExitState()
    {
        // ��������Э��
        if (_noBeansCoroutine != null)
        {
            _stateMachine.StopCoroutine(_noBeansCoroutine);
            _noBeansCoroutine = null;
        }

        if (_followPath != null)
        {
            _stateMachine.StopCoroutine(_followPath);
            _followPath = null;
        }

        _stateMachine = null;
        _targetBean = null;
        _path = null;
        targetIndex = 0;
    }

    // ���������¼�
    public void OnAnimationEvent(string eventName)
    {

    }

























    //// ����״̬ʱ���ã���ʼ����
    //public void EnterState(BossFirstStateMachine stateMachine)
    //{
    //    _stateMachine = stateMachine;
    //    _path = new NavMeshPath();
    //    _currentCornerIndex = 0;
    //    _repathTimer = 0f;
    //    _maxVerticalSpeed = _stateMachine.EatBeanMoveSpeed;

    //    _stateMachine.IsMove = true;

    //    SelectNextTarget();
    //}

    //// ÿ֡����
    //public void UpdateState()
    //{
    //    //���û��Ŀ�궹�ӣ�����ѡ����һ��
    //    if(_targetBean == null)
    //    {
    //        SelectNextTarget();
    //        return;
    //    }

    //    //�������һ����������ƻ�,������ѡ��Ŀ��
    //    if(_targetBean.gameObject == null || !_targetBean.gameObject.activeInHierarchy)
    //    {
    //        _targetBean = null;
    //        SelectNextTarget();
    //        return;
    //    }

    //    //��������·��
    //    _repathTimer += Time.deltaTime;
    //    if (_repathTimer >= RepathInterval)
    //    {
    //        _repathTimer = 0f;
    //        TryRepathToTarget();
    //    }
    //}

    //// �̶�ʱ�䲽�����£�������أ�
    //public void FixedUpdateState()
    //{


    //    #region Move

    //    if (_targetBean == null) return;

    //    // �ƶ���Ŀ�궹��
    //    Vector3 bossPos = _stateMachine.transform.position;
    //    Vector3 targetPos;

    //    if (_path != null && _path.status == NavMeshPathStatus.PathComplete && _path.corners != null && _currentCornerIndex < _path.corners.Length)
    //    {
    //        targetPos = _path.corners[_currentCornerIndex];
    //    }
    //    else
    //    {
    //        // û�п���·��ʱֱ������ bean ��λ�ã����ܱ��ϰ��赲��
    //        targetPos = _targetBean.transform.position;
    //    }

    //    // ������ֱ�ٶȣ����� Y ��ƽ����
    //    float desiredY = Mathf.MoveTowards(bossPos.y, targetPos.y, _maxVerticalSpeed * Time.fixedDeltaTime);
    //    targetPos.y = desiredY;

    //    Vector3 dir3 = targetPos - bossPos;
    //    // ֻ�� X/Y ƽ���ж� corner ���2D ��Ϸ���ã�
    //    Vector2 dir2 = new Vector2(dir3.x, dir3.y);
    //    float sqrDist = dir2.sqrMagnitude;
    //    if (sqrDist <= _reachCornerThreshold * _reachCornerThreshold)
    //    {
    //        // ���ﵱǰ corner���ƽ�����һ�� corner
    //        if (_path != null && _path.corners != null && _currentCornerIndex < _path.corners.Length - 1)
    //        {
    //            _currentCornerIndex++;
    //        }
    //    }

    //    //move(�������ٶ�) ���� ʹ�� Rigidbody2D.MovePosition������ Vector2��
    //    if (dir2.sqrMagnitude > 1e-6f)
    //    {
    //        Vector3 moveDir3 = dir3.normalized;
    //        Vector2 moveDir2 = new Vector2(moveDir3.x, moveDir3.y);
    //        float speed = _stateMachine.EatBeanMoveSpeed;
    //        _stateMachine.CurrentMoveSpeed = speed;

    //        Vector3 newPos3 = bossPos + moveDir3 * speed * Time.fixedDeltaTime;
    //        Vector2 newPos2 = new Vector2(newPos3.x, newPos3.y);

    //        _stateMachine.Rb.MovePosition(newPos2);

    //        // 2D ������ Z ����ת������ʹ�� LookRotation��
    //        if (moveDir2 != Vector2.zero)
    //        {
    //            float angle = Mathf.Atan2(moveDir2.y, moveDir2.x) * Mathf.Rad2Deg;
    //            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

    //            // ƽ����ת������˲�丩��
    //            _stateMachine.transform.rotation = Quaternion.Slerp(_stateMachine.transform.rotation, targetRot, Mathf.Clamp01(8f * Time.fixedDeltaTime));
    //        }
    //    }

    //    //���Զ����������� 2D ƽ����룩
    //    float distToBean = Vector2.Distance(new Vector2(bossPos.x, bossPos.y), new Vector2(_targetBean.transform.position.x, _targetBean.transform.position.y));

    //    #endregion

    //}

    //// �˳�״̬ʱ���ã�������
    //public void ExitState()
    //{
    //    if (_noBeansCoroutine != null)
    //    {
    //        _stateMachine.StopCoroutine(_noBeansCoroutine);
    //        _noBeansCoroutine = null;
    //    }

    //    _targetBean = null;
    //    _path = null;
    //}

    //// ���������¼�
    //public void OnAnimationEvent(string eventName)
    //{

    //}

    //#region Found Path

    //private void SelectNextTarget()
    //{
    //    Bean[] beans = UnityEngine.Object.FindObjectsOfType<Bean>();
    //    if(beans == null || beans.Length == 0)
    //    {
    //        // ����δ���еȴ�Э�̣��������ȴ����Ѿ��ڵȴ�����ֱ�ӷ���
    //        if (_noBeansCoroutine == null)
    //        {
    //            _noBeansCoroutine = _stateMachine.StartCoroutine(NoBeansWaitCoroutine());
    //        }
    //        return;
    //    }

    //    // ���ڵȴ��׶γ����˶��ӣ�ȡ���ȴ�Э��
    //    if (_noBeansCoroutine != null)
    //    {
    //        _stateMachine.StopCoroutine(_noBeansCoroutine);
    //        _noBeansCoroutine = null;
    //    }

    //    var ordered = beans.OrderByDescending(b => Vector3.Distance(_stateMachine.transform.position, b.transform.position)).ToArray();

    //    foreach (var candidate in ordered)
    //    {
    //        if (candidate == null || candidate.gameObject == null || !candidate.gameObject.activeInHierarchy) continue;

    //        // ����ͨ�� NavMesh ����·��
    //        if (TryComputePath(candidate.transform.position, out NavMeshPath candidatePath) && candidatePath.status == NavMeshPathStatus.PathComplete)
    //        {
    //            _targetBean = candidate;
    //            _path = candidatePath;
    //            _currentCornerIndex = 0;
    //            _repathTimer = 0f;
    //            return;
    //        }
    //    }

    //    // ���û���κ�Զ�Ŀɴ� bean���ٳ���ѡ����������ģ�����������
    //    var fallback = ordered.OrderBy(b => Vector3.Distance(_stateMachine.transform.position, b.transform.position)).FirstOrDefault();
    //    if (fallback != null)
    //    {
    //        // ��Ȼ����ֱ��·������Ȼ���ϰ���
    //        _targetBean = fallback;
    //        TryComputePath(_targetBean.transform.position, out _path); // �����ǲ�������·��
    //        _currentCornerIndex = 0;
    //        _repathTimer = 0f;
    //    }
    //    else
    //    {
    //        _stateMachine.ChangeState(BossState.AttackRandomMove);
    //    }
    //}

    //private IEnumerator NoBeansWaitCoroutine()
    //{
    //    yield return new WaitForSeconds(NoBeansWaitTime);

    //    // �ȴ��������ٴμ�鳡���Ƿ��ж���
    //    Bean[] beansAfter = UnityEngine.Object.FindObjectsOfType<Bean>();
    //    _noBeansCoroutine = null;

    //    if (beansAfter == null || beansAfter.Length == 0)
    //    {
    //        _stateMachine.ChangeState(BossState.AttackRandomMove);
    //    }
    //    else
    //    {
    //        // �ж���������ѡĿ��
    //        SelectNextTarget();
    //    }
    //}

    //private bool TryRepathToTarget()
    //{
    //    if (_targetBean == null) return false;
    //    return TryComputePath(_targetBean.transform.position, out _path);
    //}


    //private bool TryComputePath(Vector3 destination, out NavMeshPath outPath)
    //{
    //    outPath = new NavMeshPath();
    //    try
    //    {
    //        Vector3 source = _stateMachine.transform.position;
    //        // ʹ�� NavMesh ������ȫ·��
    //        bool ok = NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, outPath);
    //        return ok;
    //    }
    //    catch (Exception)
    //    {
    //        // ��� NavMesh �����û����쳣������ʧ�ܣ��ϲ���������ƶ���
    //        outPath = null;
    //        return false;
    //    }
    //}

    //#endregion
}
