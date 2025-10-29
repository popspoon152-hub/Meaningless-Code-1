using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackSlider : MonoBehaviour
{
    [SerializeField] private Slider BackBar;

    public void UpdateCD(float current, float max)
    {
        BackBar.value = (max - current) / max;
    }
}
