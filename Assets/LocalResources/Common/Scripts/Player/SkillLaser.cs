using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShootLaser : MonoBehaviour
{
    [Header("�������")]
    public float laserLength = 5f;//��ʵ�����ű���
    public float laserWidth = 0.2f;
    public float laserDuration = 0.4f;

    [Header("ʱ�����")]
    public float preCastTime = 0.25f;//��ǰ
    //��
    public float postCastTime = 0.25f;//���
    public float backStepDistance = 0.5f;//�󳷾���
    public float backStepTime = 0.1f;//��ʱ��

    [Header("����")]
    public Transform firePoint;
    public GameObject laserPrefab;

    private Rigidbody2D rb;
    private PlayerInputControls PlayerInputControls;
    private MonoBehaviour moveScript; // �����ƶ��ű�
    private bool isCasting = false;

    // �󳷿���
    private Vector2 backStartPos;
    private Vector2 backTargetPos;
    private float backStartTime;

    // ��¼ԭʼԼ��
    private RigidbodyConstraints2D originalConstraints;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerInputControls = new PlayerInputControls();
        PlayerInputControls.Player.Shoot.started += OnShootLaser;
        moveScript = GetComponent<PlayerMovement>(); // �Զ��������ƶ��ű�
    }

    private void OnEnable() => PlayerInputControls.Enable();
    private void OnDisable() => PlayerInputControls.Disable();

    private void OnShootLaser(InputAction.CallbackContext ctx)
    {
        if (!isCasting)
            StartCoroutine(CastLaserRoutine());
    }

    private System.Collections.IEnumerator CastLaserRoutine()
    {
        isCasting = true;
        DisableMovement(); // === ��ֹ�ƶ� ===

        //  ����Y�ᣨ������
        originalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;

        // ǰҡ
        yield return new WaitForSeconds(preCastTime);

        // ���伤�⣨�������ɫ��ͬ��
        SpawnLaser();

        // ��������ڼ�
        yield return new WaitForSeconds(laserDuration);

        // ƽ����
        yield return StartCoroutine(BackStepSmooth());

        // �������
        rb.constraints = originalConstraints;

        // ��ҡ
        yield return new WaitForSeconds(postCastTime);

        EnableMovement(); // === ����ƶ����� ===
        isCasting = false;
    }

    private void SpawnLaser()
    {
        if (!laserPrefab || !firePoint) return;

        float facing = Mathf.Sign(transform.localScale.x != 0 ? transform.localScale.x : 1f);
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
        laser.transform.right = transform.right * facing; // ���ⷽ�����ɫ����һ��
        laser.transform.localScale = new Vector3(laserLength, laserWidth, 1f);
        Destroy(laser, laserDuration);
    }

    private System.Collections.IEnumerator BackStepSmooth()
    {
        float facing = Mathf.Sign(transform.localScale.x != 0 ? transform.localScale.x : 1f);
        backStartPos = rb.position;
        backTargetPos = backStartPos - new Vector2(facing * backStepDistance, 0f);
        backStartTime = Time.time;

        while (Time.time - backStartTime < backStepTime)
        {
            float t = (Time.time - backStartTime) / backStepTime;
            Vector2 newPos = Vector2.Lerp(backStartPos, backTargetPos, t);
            rb.MovePosition(newPos);
            yield return null;
        }
        rb.MovePosition(backTargetPos);
    }

    private void DisableMovement()
    {
        if (moveScript != null)
            moveScript.enabled = false;
        rb.velocity = Vector2.zero;
    }

    private void EnableMovement()
    {
        if (moveScript != null)
            moveScript.enabled = true;
    }
}
