using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �ҵ��Զ���Button���̳� Button
/// </summary>
public class MyButton : Button
{
    // ���캯��
    protected MyButton()
    {
        my_onDoubleClick = new ButtonClickedEvent();
        my_onLongPress = new ButtonClickedEvent();
    }

    // ����
    public ButtonClickedEvent my_onLongPress;
    public ButtonClickedEvent OnLongPress
    {
        get { return my_onLongPress; }
        set { my_onLongPress = value; }
    }

    // ˫��
    public ButtonClickedEvent my_onDoubleClick;
    public ButtonClickedEvent OnDoubleClick
    {
        get { return my_onDoubleClick; }
        set { my_onDoubleClick = value; }
    }

    // ������Ҫ�ı�������
    private bool my_isStartPress = false;
    private float my_curPointDownTime = 0f;
    private float my_longPressTime = 0.6f;
    private bool my_longPressTrigger = false;


    void Update()
    {
        CheckIsLongPress();
    }

    #region ����

    /// <summary>
    /// ��������
    /// </summary>
    void CheckIsLongPress()
    {
        if (my_isStartPress && !my_longPressTrigger)
        {
            if (Time.time > my_curPointDownTime + my_longPressTime)
            {
                my_longPressTrigger = true;
                my_isStartPress = false;
                if (my_onLongPress != null)
                {
                    my_onLongPress.Invoke();
                }
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        my_curPointDownTime = Time.time;
        my_isStartPress = true;
        my_longPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        my_isStartPress = false;

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        my_isStartPress = false;

    }

    #endregion

    #region ˫����������

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!my_longPressTrigger)
        {
            if (eventData.clickCount == 2)
            {

                if (my_onDoubleClick != null)
                {
                    my_onDoubleClick.Invoke();
                }

            }
            else if (eventData.clickCount == 1)
            {
                onClick.Invoke();
            }
        }
    }
    #endregion
}