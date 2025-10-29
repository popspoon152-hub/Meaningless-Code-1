using System.Collections;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public bool IsDevoured = false;    // �Ƿ�����
    public float RespawnTime = 5f;     // ��������ʱ��

    private Renderer _renderer;
    private Collider2D _collider;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider2D>();
    }

    public void Devour()
    {
        if (!IsDevoured)
        {
            IsDevoured = true;
            _renderer.enabled = false;   // ���ص���
            _collider.enabled = false;   // ������ײ��

            // ������������
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);

        IsDevoured = false;
        _renderer.enabled = true;      // �ָ���ʾ
        _collider.enabled = true;      // �ָ���ײ
    }
}
