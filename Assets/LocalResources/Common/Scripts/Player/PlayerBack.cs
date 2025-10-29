using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBack : MonoBehaviour
{
    [Header("��ɫ����")]
    [Range(1f, 5f)] public float BackTime = 2f;                     //���ݵ�����ǰ��λ��
    public PlayerHealth Health;

    [System.Serializable]
    public class TimedPostion
    {
        public Vector2 Position;
        public float Time;

        public TimedPostion(Vector2 pos, float t)
        {
            Position = pos;
            Time = t;
        }
    }
    private Queue<TimedPostion> _positionQueue = new Queue<TimedPostion>();

    private void Update()
    {
        RecordCurrentPostion();

        RemoveOldPostions();

        BackCheck();
    }

    #region Quene
    private void RecordCurrentPostion()
    {
        _positionQueue.Enqueue(new TimedPostion(transform.position, Time.time));
    }

    private void RemoveOldPostions()
    {
        float currentTime = Time.time;

        while (_positionQueue.Count > 0 && currentTime - _positionQueue.Peek().Time > BackTime)
        {
            _positionQueue.Dequeue();
        }
    }

    #endregion
    
    private void BackCheck()
    {
        if (InputManager.BackWasPressed)
        {
            Vector2 backPos;

            if (_positionQueue.Count > 0)
            {
                // ��������ɵ�λ�þ�������ǰ��λ��
                backPos = _positionQueue.Peek().Position;
                Health.HealthUntilExtraHealth();
            }
            else
            {
                // ���û���㹻����ʷ���ݣ����ص�ǰλ��
                Debug.LogWarning("Not enough position history, returning current position");
                backPos = transform.position;
            }

            transform.position = backPos;
        }

    }
}
