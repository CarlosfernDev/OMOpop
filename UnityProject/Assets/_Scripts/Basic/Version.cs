using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Version : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    private void Awake()
    {
        _Text.text = "V." + Application.version;
    }
}
