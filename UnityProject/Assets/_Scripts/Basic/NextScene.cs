using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField] private string SceneName;

    private void Start()
    {
        SceneManager.LoadScene(SceneName);
    }
}
