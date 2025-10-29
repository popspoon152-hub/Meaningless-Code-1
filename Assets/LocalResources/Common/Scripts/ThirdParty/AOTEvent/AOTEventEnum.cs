using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CutyRoom.Core
{
    /// <summary>
    /// �¼� ö��
    public enum AOTEventEnum
    {
        /// <summary>
        /// ö�ٿ�ʼ
        /// </summary>
        BUEnum_Start = 1,
        /// <summary>
        /// ȡ�������¼�
        /// </summary>
        OffAllEvent,
        /// <summary>
        /// unity update ����
        /// </summary>
        OnUpdate,

        /// <summary>
        /// unity fixedUpdate ����
        /// </summary>
        OnFixedUpdate,

        /// <summary>
        /// �߼�֡����ǰ�¼�
        /// </summary>
        OnLogicFrameBefore,
        /// <summary>
        /// �߼�֡���ý������¼�
        /// </summary>
        OnLogicFrameAfter,
        /// <summary>
        /// ����Ⱦ֡����ǰ�¼�
        /// </summary>
        OnRenderFrameBefore,
        /// <summary>
        /// ����Ⱦ֡���ý������¼�
        /// </summary>
        OnRenderFrameAfter,

        /// <summary>
        /// ��UI
        /// </summary>
        OPEN_UI,

        /// <summary>
        /// �ر�UI
        /// </summary>
        CLOSE_UI,


        /// <summary>
        /// ������ײ
        /// </summary>
        OnColliderDisable,
        /// <summary>
        /// ���Ϊ�����ٵ� GameObject Awake
        /// </summary>
        OnAwakeStaticGameObject,

        /// <summary>
        /// ö�ٽ���
        /// </summary>
        BUEnum_End,
    }
}