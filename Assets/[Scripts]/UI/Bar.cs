using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField]
    private Image fillArea;
    [SerializeField, Range(0, 1)]
    private float _amount;
    public float amount
    {
        get
        {
            return _amount;
        }
        set
        {
            _amount = value;
            fillArea.fillAmount = _amount;
        }
    }

    private void OnValidate()
    {
        amount = _amount;
    }
}
