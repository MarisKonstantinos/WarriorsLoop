using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject gameOverUI;

    private void Awake()
    {
        if(Instance != null &&  Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ToggleGameOverUI(bool toggle)
    {
        gameOverUI.SetActive(toggle);
    }
}
