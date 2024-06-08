using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dev : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    private void Awake()
    {
        _Text.text = "By:" + Application.companyName;
    }
}
