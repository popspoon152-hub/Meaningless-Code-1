using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAndMap : MonoBehaviour
{
    [Header("��ͼ")]
    public GameObject TileMapStairs1;                                                                      //��ͼ����
    public GameObject TileMapWall1;                                                                      //��ͼ����
    public Transform[] BeansPositions1;                                                                //�������ɵ�����
    public GameObject TileMapStairs2;                                                                      //��ͼ����
    public GameObject TileMapWall2;                                                                      //��ͼ����
    public Transform[] BeansPositions2;                                                                //�������ɵ�����

    [Header("���������߼�")]
    [Range(1f, 10f)] public int MinBeansInstantiatePerTime1 = 2;                                         //��ͼÿ���������ٵĶ�������
    [Range(1f, 10f)] public int MaxBeansInstantiatePerTime1 = 5;                                         //��ͼÿ���������Ķ�������
    [Range(1f, 5f)] public int BeansInstantiateTimes1 = 2;                                               //ÿ����ͼ���ɶ��ӵĴ���
    [Range(1f, 10f)] public int MinBeansInstantiatePerTime2 = 2;                                         //��ͼÿ���������ٵĶ�������
    [Range(1f, 10f)] public int MaxBeansInstantiatePerTime2 = 5;                                         //��ͼÿ���������Ķ�������
    [Range(1f, 5f)] public int BeansInstantiateTimes2 = 2;                                               //ÿ����ͼ���ɶ��ӵĴ���

    [Header("��ͼͣ��ʱ��")]
    [Range(5f, 120f)] public float MapStayTime1 = 20f;                                                     //��ͼͣ��ʱ��(Ȼ���л���һ����ͼ)
    [Range(5f, 120f)] public float MapStayTime2 = 20f;                                                     //��ͼͣ��ʱ��(Ȼ���л���һ����ͼ)

    public GameObject BeansPrefabs;

    public int CurrentBeansCount { get; private set; }

    private int _currentMapIndex;

    private int _currentBeansInstantiatedTime;

    private int _BeansInstantiateThisTime;

    private bool _isWaitingForNextMap = false;

    private Coroutine _waitCoroutine;

    private int _beansInstantiateThisTime;

    public static EnvironmentAndMap Ins { get; private set; }

    #region LifeCycle

    private void Awake()
    {
        if(Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (!ValidateReferences())
        {
            return;
        }
        _currentMapIndex = 1;

        LoadMap(_currentMapIndex);
    }

    private void Update()
    {
        if(CurrentBeansCount == 0 && !_isWaitingForNextMap)
        {
            CheckBeans();
        }
    }

    private void OnDestroy()
    {
        if(_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
        }
    }

    #endregion

    #region LoadBeansAndMap
    private void LoadMap(int currentMapIndex)
    {
        if (!ValidateReferences()) return;

        TileMapStairs1.SetActive(false);
        TileMapWall1.SetActive(false);
        TileMapStairs2.SetActive(false);
        TileMapWall2.SetActive(false);

        if (currentMapIndex == 1)
        {
            TileMapStairs1.SetActive(true);
            TileMapWall1.SetActive(true);
            _currentBeansInstantiatedTime = BeansInstantiateTimes1;
        }
        else if (currentMapIndex == 2)
        {
            TileMapStairs2.SetActive(true);
            TileMapWall2.SetActive(true);
            _currentBeansInstantiatedTime = BeansInstantiateTimes2;
        }

        _isWaitingForNextMap = false;

        //��������
        if (GridManager.Ins != null)
        {
            GridManager.Ins.CreateGrid();
        }
        else
        {
            Debug.LogWarning("GridManager.Instance Ϊ�գ�");
        }

        //ʵ��������
        ChooseInstantiatePoints();
    }

    private void ChooseInstantiatePoints()
    {
        if (!ValidateReferences()) return;

        Transform[] currentPositions;

        if (_currentMapIndex == 1)
        {
            currentPositions = BeansPositions1;
            _beansInstantiateThisTime = Mathf.Clamp(UnityEngine.Random.Range(MinBeansInstantiatePerTime1, MaxBeansInstantiatePerTime1 + 1), 1, currentPositions.Length);
        }
        else
        {
            currentPositions = BeansPositions2;
            _beansInstantiateThisTime = Mathf.Clamp(UnityEngine.Random.Range(MinBeansInstantiatePerTime2, MaxBeansInstantiatePerTime2 + 1), 1, currentPositions.Length);
        }

        // ѡ�����ɵ�
        Transform[] choosePoints = GetRandomSequence(currentPositions, _beansInstantiateThisTime);

        if (choosePoints == null || choosePoints.Length == 0)
        {
            //Debug.LogError("û����Ч�Ķ������ɵ㣡");
            return;
        }

        // ʵ��������
        int successfulInstantiates = 0;
        for (int i = 0; i < choosePoints.Length; i++)
        {
            if (choosePoints[i] != null)
            {
                Instantiate(BeansPrefabs, choosePoints[i].position, Quaternion.identity);
                successfulInstantiates++;
            }
        }

        CurrentBeansCount = successfulInstantiates;
        //Debug.Log($"�ɹ������� {successfulInstantiates} ������");
    }

    public static Transform[] GetRandomSequence(Transform[] array, int count)
    {
        if (array == null || array.Length == 0 || count <= 0)
        {
            //Debug.LogError("GetRandomSequence: ���������Ч");
            return new Transform[0];
        }

        // ȷ��count���������鳤��
        count = Mathf.Min(count, array.Length);
        Transform[] output = new Transform[count];

        // ���������Ա����޸�ԭ����
        Transform[] tempArray = new Transform[array.Length];
        array.CopyTo(tempArray, 0);

        // Fisher-Yatesϴ���㷨
        for (int i = tempArray.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Transform temp = tempArray[i];
            tempArray[i] = tempArray[j];
            tempArray[j] = temp;
        }

        // ȡǰcount��Ԫ��
        for (int i = 0; i < count; i++)
        {
            output[i] = tempArray[i];
        }

        return output;
    }

    #endregion

    #region CheckBeans

    private void CheckBeans()
    {
        if(_currentBeansInstantiatedTime > 0)
        {
            _currentBeansInstantiatedTime--;
            ChooseInstantiatePoints();
        }
        else
        {
            _isWaitingForNextMap = true;
            _waitCoroutine = StartCoroutine(WaitForBattleEnd());
        }
    }

    private IEnumerator WaitForBattleEnd()
    {
        float waitTime = _currentMapIndex == 1 ? MapStayTime1 : MapStayTime2;
        if(_currentMapIndex == 1)
        {
            TileMapStairs1.SetActive(false);
            TileMapWall1.SetActive(true);
            TileMapStairs2.SetActive(false);
            TileMapWall2.SetActive(false);
        }
        else
        {
            TileMapStairs1.SetActive(false);
            TileMapWall1.SetActive(false);
            TileMapStairs2.SetActive(false);
            TileMapWall2.SetActive(true);
        }

        yield return new WaitForSeconds(waitTime);

        // �л���ͼǰ�������Ƿ���Ȼ����
        if (this == null) yield break;

        // �л���ͼ
        _currentMapIndex = _currentMapIndex == 1 ? 2 : 1;
        LoadMap(_currentMapIndex);
        _isWaitingForNextMap = false; // ���õȴ�״̬
    }

    public void OnBeanEaten()
    {
        if (CurrentBeansCount > 0)
        {
            CurrentBeansCount--;
        }
    }
    #endregion

    #region Validation
    private bool ValidateReferences()
    {
        bool isValid = true;

        if (TileMapStairs1 == null)
        {
            Debug.LogError("TileMapStairs1 δ��ֵ��");
            isValid = false;
        }

        if(TileMapWall1 == null)
        {
            Debug.LogError("TileMapWall1 δ��ֵ");
            isValid = false;
        }

        if (TileMapWall2 == null)
        {
            Debug.LogError("TileMapWall2 δ��ֵ");
            isValid = false;
        }

        if (TileMapStairs2 == null)
        {
            Debug.LogError("TileMapStairs2 δ��ֵ��");
            isValid = false;
        }

        if (BeansPrefabs == null)
        {
            Debug.LogError("BeansPrefabs δ��ֵ��");
            isValid = false;
        }

        if (BeansPositions1 == null || BeansPositions1.Length == 0)
        {
            Debug.LogError("BeansPositions1 ����Ϊ�ջ�δ��ֵ��");
            isValid = false;
        }
        else
        {
            // ����������Ƿ��п�Ԫ��
            for (int i = 0; i < BeansPositions1.Length; i++)
            {
                if (BeansPositions1[i] == null)
                {
                    Debug.LogError($"BeansPositions1 ���� {i} Ϊ�գ�");
                    isValid = false;
                }
            }
        }

        if (BeansPositions2 == null || BeansPositions2.Length == 0)
        {
            Debug.LogError("BeansPositions2 ����Ϊ�ջ�δ��ֵ��");
            isValid = false;
        }
        else
        {
            for (int i = 0; i < BeansPositions2.Length; i++)
            {
                if (BeansPositions2[i] == null)
                {
                    Debug.LogError($"BeansPositions2 ���� {i} Ϊ�գ�");
                    isValid = false;
                }
            }
        }

        return isValid;
    }
    #endregion
}
