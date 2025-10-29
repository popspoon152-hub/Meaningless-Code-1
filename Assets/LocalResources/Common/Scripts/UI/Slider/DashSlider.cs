using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSlider : MonoBehaviour
{
    [SerializeField] private Slider Bar;

    public void UpdateCD(float current, float max)
    {
        Bar.value = (max - current) / max;
    }
}
