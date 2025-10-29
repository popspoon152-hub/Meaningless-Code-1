using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڵ��� Pathfinding ����Ŀ��ӻ��ű������޸� Pathfinding ����
/// </summary>
[ExecuteAlways]
public class PathfindingDebugger : MonoBehaviour
{
    [Header("���Բ���")]
    public Pathfinding pathfinding;     // ����Ҫ���Ե� Pathfinding ���
    public Transform startPoint;        // ��㣨ͨ����Boss��Seeker��
    public Transform endPoint;          // �յ㣨ͨ����Player��Target��
    public bool autoUpdate = true;      // �Ƿ�ʵʱ����·��
    public float updateInterval = 0.5f; // �Զ����¼�����룩

    [Header("���ӻ�����")]
    public Color lineColor = Color.green;
    public Color nodeColor = Color.cyan;
    public Color startColor = Color.yellow;
    public Color endColor = Color.red;
    public float nodeRadius = 0.15f;

    private List<Vector3> _currentPath;
    private float _updateTimer;

    private void Update()
    {
        if (pathfinding == null || startPoint == null || endPoint == null)
            return;

        if (!autoUpdate) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer >= updateInterval)
        {
            _updateTimer = 0f;
            UpdatePath();
        }
    }

    [ContextMenu("Manual Update Path")]
    public void UpdatePath()
    {
        if (pathfinding == null)
        {
            Debug.LogWarning("[PathfindingDebugger] Missing Pathfinding reference.");
            return;
        }

        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning("[PathfindingDebugger] Missing startPoint or endPoint reference.");
            return;
        }

        _currentPath = pathfinding.FindPath(startPoint.position, endPoint.position);

        //if (_currentPath == null || _currentPath.Count == 0)
        //{
        //    Debug.LogWarning("[PathfindingDebugger] Pathfinding returned no valid path.");
        //}
        //else
        //{
        //    Debug.Log($"[PathfindingDebugger] Path updated. Node count = {_currentPath.Count}");
        //}
    }

    private void OnDrawGizmos()
    {
        if (_currentPath == null || _currentPath.Count == 0)
            return;

        Gizmos.color = lineColor;
        for (int i = 0; i < _currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(_currentPath[i], _currentPath[i + 1]);
        }

        // ���ƽڵ�
        foreach (var node in _currentPath)
        {
            Gizmos.color = nodeColor;
            Gizmos.DrawSphere(node, nodeRadius);
        }

        // ����������յ�
        if (startPoint != null)
        {
            Gizmos.color = startColor;
            Gizmos.DrawSphere(startPoint.position, nodeRadius * 1.2f);
        }

        if (endPoint != null)
        {
            Gizmos.color = endColor;
            Gizmos.DrawSphere(endPoint.position, nodeRadius * 1.2f);
        }
    }
}
