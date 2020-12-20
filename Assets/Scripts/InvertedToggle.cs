using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvertedToggle : MonoBehaviour
{
    [SerializeField] private GameObject offCheckmark;

    Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        OnToggleValueChanged(toggle.isOn);
    }

    private void OnValidate()
    {
        Toggle toggle = GetComponent<Toggle>();
        OnToggleValueChanged(toggle.isOn);
    }

    public void OnToggleValueChanged(bool value)
    {
        offCheckmark.SetActive(!value);
    }
}
