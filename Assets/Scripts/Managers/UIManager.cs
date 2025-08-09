using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI playerScoreTMP;

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

    public void ScoreUpdate(int score)
    {
        if (!playerScoreTMP) return;

        playerScoreTMP.text = "Score:" + score;
    }
}
