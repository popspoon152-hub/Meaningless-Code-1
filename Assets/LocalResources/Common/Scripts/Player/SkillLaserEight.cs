using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserSkillEightWay : MonoBehaviour
{
    public PlayerMovement Movement;
    public LaserSlider Slider;

    [Header("�������")]
    public float laserLength = 5f;
    public float laserWidth = 0.2f;
    public float laserDuration = 0.4f;
    [Range(1f,10f)]public float laserCollDownTime = 5f;              //�������ȴ
    private float _laserTimer;

    [Header("ǰ��ҡ���")]
    public float preCastTime = 0.25f;
    public float postCastTime = 0.25f;
    public float backStepForce = 6f; // �� ������С���������ʱ��

    [Header("����")]
    public Transform firePoint;
    public GameObject laserPrefab;
    public MonoBehaviour moveScriptToDisable; // PlayMove �ű�
    public PlayerDirection directionController;

    private Rigidbody2D rb;
    private PlayerInputControls PlayerInputControls;
    private bool isCasting = false;

    // ��¼ԭʼԼ��
    private RigidbodyConstraints2D originalConstraints;

    [Header("Anim")]
    public Animator Anim;
    public float AnimOffset = 0.5f;     //anim������ֵ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerInputControls = new PlayerInputControls();
        PlayerInputControls.Player.Shoot.started += OnShoot;
    }

    private void Update()
    {
        if (_laserTimer > 0f) _laserTimer -= Time.deltaTime;
        Slider.UpdateCD(_laserTimer, laserCollDownTime);
    }

    private void OnEnable() => PlayerInputControls.Enable();
    private void OnDisable() => PlayerInputControls.Disable();

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (!isCasting && _laserTimer <= 0f && !Movement._isDashing && !Movement._isJumping && !Movement._isFalling)
        {
            StartCoroutine(LaserSequenceCoroutine());
        }
    }

    private IEnumerator LaserSequenceCoroutine()
    {
        isCasting = true;

        // ����Y�ᣨ������
        originalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;

        // ��ֹ�ƶ�
        if (moveScriptToDisable != null) moveScriptToDisable.enabled = false;


        SetPreAnim();
        yield return new WaitForSeconds(preCastTime);

        Vector2 dir = GetShootDirection();
        SpawnLaser(dir);

        yield return new WaitForSeconds(laserDuration);

        // �����󳷣�ʩ�ӷ��������
        //Vector2 recoil = -dir.normalized * backStepForce;
        //rb.AddForce(recoil, ForceMode2D.Impulse);

        // ����Y��
        rb.constraints = originalConstraints;

        yield return new WaitForSeconds(postCastTime);

        // �ָ��ƶ�
        if (moveScriptToDisable != null) moveScriptToDisable.enabled = true;
        isCasting = false;

        _laserTimer = laserCollDownTime;
    }

    /// <summary>
    /// �������λ���趨����ǰҡ�Ķ���
    /// </summary>
    /// <exception></exception>
    private void SetPreAnim()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = transform.position;

        if(mousePos.x > playerPos.x)
        {
            //��
            Anim.SetTrigger("Right");
        }
        else
        {
            //��
            Anim.SetTrigger("Left");
        }
        //if(mousePos.x < playerPos.x - AnimOffset && playerPos.y - AnimOffset < mousePos.y && mousePos.y < playerPos.y + AnimOffset)
        //{
        //    //��
        //    Anim.SetTrigger("Left");
        //}
        //else if(mousePos.x > playerPos.x + AnimOffset && playerPos.y - AnimOffset < mousePos.y && mousePos.y < playerPos.y + AnimOffset)
        //{
        //    //��
        //    Anim.SetTrigger("Right");
        //}
        //else if(mousePos.x < playerPos.x - AnimOffset && mousePos.y > playerPos.y + AnimOffset)
        //{
        //    //����
        //    Anim.SetTrigger("LeftUp");
        //}
        //else if (mousePos.x > playerPos.x + AnimOffset && mousePos.y > playerPos.y + AnimOffset)
        //{
        //    //����
        //    Anim.SetTrigger("RightUp");
        //}
        //else if (mousePos.x < playerPos.x - AnimOffset && mousePos.y < playerPos.y - AnimOffset)
        //{
        //    //����
        //    Anim.SetTrigger("LeftDown");
        //}
        //else if (mousePos.x > playerPos.x + AnimOffset && mousePos.y < playerPos.y - AnimOffset)
        //{
        //    //����
        //    Anim.SetTrigger("RightDown");
        //}
        //else if (playerPos.x - AnimOffset < mousePos.x && mousePos.x < playerPos.x + AnimOffset && mousePos.y > playerPos.y + AnimOffset)
        //{
        //    //��
        //    Anim.SetTrigger("Up");
        //}
        //else if(playerPos.x - AnimOffset < mousePos.x && mousePos.x < playerPos.x + AnimOffset && mousePos.y < playerPos.y - AnimOffset)
        //{
        //    //��
        //    Anim.SetTrigger("Down");
        //}
        //else
        //{
        //    Vector2 posVector = mousePos - playerPos;
        //    if(posVector.x > 0 && posVector.y > 0)
        //    {
        //        //����
        //        Anim.SetTrigger("RightUp");
        //    }
        //    else if (posVector.x > 0 && posVector.y < 0)
        //    {
        //        //����
        //        Anim.SetTrigger("RightDown");
        //    }
        //    else if (posVector.x < 0 && posVector.y > 0)
        //    {
        //        //����
        //        Anim.SetTrigger("LeftUp");
        //    }
        //    else
        //    {
        //        //����
        //        Anim.SetTrigger("LeftDown");
        //    }
        //}
    }

    private Vector2 GetShootDirection()
    {
        if (directionController == null)
        {
            Debug.LogWarning("[LaserSkill] δ�� PlayerDirectionController��Ĭ�����ҡ�");
            return Vector2.right;
        }

        // ���з������룬�������뷽�򣬷����õ�ǰ����
        if (directionController.hasInput)
            return directionController.directionState.normalized;
        else
            return directionController.facingDirection == Vector2.zero ? Vector2.right : directionController.facingDirection;
    }

    private void SpawnLaser(Vector2 direction)
    {
        if (laserPrefab == null || firePoint == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        Vector3 scale = laser.transform.localScale;
        scale.x = laserLength;
        scale.y = laserWidth;
        laser.transform.localScale = scale;

        Destroy(laser, laserDuration);
    }
}
