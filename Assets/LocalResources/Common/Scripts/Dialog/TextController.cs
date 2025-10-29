using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

public class TextController : MonoBehaviour
{
    public TMP_Text testText;
    IEnumerator TypeText(TMP_Text tmp_text, string str, float interval)
    {
        int i = 0; 
        while (i<= str.Length)
        {
            tmp_text.text = str.Substring(0, i++); 
            yield return new WaitForSeconds(interval);
        }
    }


    void Start()
    {
        StartCoroutine(TypeText(testText,"����һ���������ִ�ӡ�Ĵ��룬����������Ϊ�Ի����������ʾ",0.15f));
    }

}
