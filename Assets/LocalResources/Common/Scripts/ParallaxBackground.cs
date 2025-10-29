
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform playerPos;
    [Header("�Ƿ��ǹ̶�λ��")]
    public bool isStable=false;
    public float followDistance;
    private Transform mainCameraTrans; // ���������Transform���
    private Vector3 lastCameraPosition; // ��һ֡�������λ��
    private float textureUnitSizeX; // ����ͼ��λ�ߴ�
    private Vector3 offset;
    public bool followCamera;

    public Vector2 followWeight; // �����������Ȩ��
                                 // ����ԽԶ������Ȩ��Խ�ߣ��� ��ա��ơ�̫���� ����0.8-1��ΧЧ���п�
                                 // ����Խ��������Ȩ��Խ�ͣ��� ��ߵ���ľ�����ݡ����ӵȵ�

    void Start()
    {
        if (isStable)
        {
            playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
            followCamera = false;
        }
        
            mainCameraTrans = Camera.main.transform; // ��ȡ���������Transform���
            lastCameraPosition = mainCameraTrans.position; // ��ʼ����һ֡�������λ��Ϊ��ǰ�������λ��

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture; // ��ȡSprite������
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit; // ���㱳��ͼ����Ϸ������ĵ�λ�ߴ�
            offset = transform.position - mainCameraTrans.position;
        
    }
    void Update()
    {

    }
    private void LateUpdate()
    {
        if (isStable)
        {
            float dis = Mathf.Abs(transform.position.x - playerPos.position.x);
            if (dis > followDistance)
            {
                lastCameraPosition = mainCameraTrans.position;
                return;
            }
        }
        ImageFollowCamera();
        if(followCamera)
        {
            ResetImageX();
        }

        lastCameraPosition = mainCameraTrans.position; // ������һ֡�������λ��
    }

    private void ResetImageX()
    {
        // ����Ƿ���Ҫ�ƶ�����
        if (Mathf.Abs(mainCameraTrans.position.x - transform.position.x) >= textureUnitSizeX)
        {
            // ���ñ���λ��
            transform.position = new Vector3(mainCameraTrans.position.x, transform.position.y, transform.position.z);
        }
    }

    private void ImageFollowCamera()
    {
        // ���������λ�õ�ƫ����
        Vector3 offsetPosition = mainCameraTrans.position - lastCameraPosition;

        // ����Ȩ�ص�������ͼƬ��λ��
        transform.position += new Vector3(offsetPosition.x * followWeight.x, offsetPosition.y * followWeight.y, 0);
    }
}