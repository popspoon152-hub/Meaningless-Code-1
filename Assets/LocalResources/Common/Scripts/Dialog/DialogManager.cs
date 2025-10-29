using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DialogManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset dialogDataFile;//csv�ĵ�


    public SpriteRenderer spriteLeft;//���ͼ��

    public SpriteRenderer spriteRight;//�Ҳ�ͼ��

    public SpriteRenderer SayPart;//��


    public TMP_Text nameText;//��ɫ�����ı�

    public TMP_Text dialogText;//�Ի������ı�

    public float waitingTime;//ÿ���ֵȴ�ʱ��

    public List<Sprite> sprites = new List<Sprite>();//��ɫͼƬ�б�

    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();//��ɫ����ͼƬ��Ӧ�ֵ�

    public int dialogIndex;//��ǰ�Ի�����

    public string[] dialogRows;//���зָ�����

    public Button nextButton;//����

    public GameObject OptionButton;//ѡ��Ԥ����

    public Transform ButtonGroup;//ѡ��ڵ㣬��������

    private Coroutine typeTextCoroutine;

    public Animator TransfromAnim;

    private void Awake()
    {
        SayPart.gameObject.SetActive(false);
        ReadText(dialogDataFile);
        ReadCharacter();
    }
    void Start()
    {
        //ReadText(dialogDataFile);
        //ReadCharacter();
    }

    IEnumerator TypeText(TMP_Text tmp_text, string str, float interval)
    {
        int i = 0;
        while (i <= str.Length)
        {
            tmp_text.text = str.Substring(0, i++);
            yield return new WaitForSeconds(interval);
        }
    }
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        //dialogText.text = _text;
        // ֹͣ�������е�Э��
        if (typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
        }

        // �����µ�Э��
        typeTextCoroutine = StartCoroutine(TypeText(dialogText, _text, waitingTime));
    }

    public void UpdataImage(string _name, string _position)
    {
        if (_position == "��")
        {
            SayPart.gameObject.SetActive(true);
            spriteLeft.sprite = imageDic[_name];
        }
        else if (_position == "��")
        {
            SayPart.gameObject.SetActive(true);
            spriteRight.sprite = imageDic[_name];
        }
    }

    public void ReadCharacter()
    {
        for (int i = 1; i < sprites.Count + 1; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[8] != null && cells[8] != "")
            {
                imageDic[cells[8]] = sprites[i - 1];
            }

        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        //dialogRows = _textAsset.text.Split('\n');
        //Debug.Log("��ȡ�ɹ�");
        // ʹ�� UTF-8 �����ȡ�ļ�����
        string[] lines = _textAsset.text.Split(new[] { "NEXT" }, System.StringSplitOptions.RemoveEmptyEntries);

        // ����ÿһ�в��ָ�
        dialogRows = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            dialogRows[i] = lines[i].Trim(); // �Ƴ�ÿ�п�ͷ�ͽ�β�Ŀո�
        }

        // ���ÿһ��
        Debug.Log("��ȡ�ɹ�");
    }


    public void ShowDialogRow()
    {
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                //Debug.Log("succeed");
                UpdateText(cells[2], cells[4]);
                UpdataImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                nextButton.gameObject.SetActive(true);
                break;
            }
            else if (cells[0] == "$" && int.Parse(cells[1]) == dialogIndex)
            {
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {
                dialogIndex = 0;
                StartCoroutine(LoadNextScene());
            }
        }


    }

    private IEnumerator LoadNextScene()
    {
        TransfromAnim.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    public void OnClickNext()
    {
        Debug.Log("�����");
        ShowDialogRow();
    }

    public void GenerateOption(int _index)
    {
        string[] cells = dialogRows[_index].Split(",");
        if (cells[0] == "$")
        {
            GameObject button = Instantiate(OptionButton, ButtonGroup);//�󶨰�ť�¼�
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    OnOptionClick(int.Parse(cells[5]));
                }
            );
            GenerateOption(_index + 1);
        }

    }

    public void OnOptionClick(int _id)//���ѡ����ת
    {
        dialogIndex = _id;
        ShowDialogRow();
        for (int i = 0; i < ButtonGroup.childCount; i++)
        {
            Destroy(ButtonGroup.GetChild(i).gameObject);
        }
        nextButton.gameObject.SetActive(true);
    }

    public void OptionEffect(string _effect, int _param, string _target)//Ч������ֵ��Ŀ��
    {

    }
}