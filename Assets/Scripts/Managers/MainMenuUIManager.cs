using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance;
    [SerializeField] private TextMeshProUGUI playerHighScore;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdatePlayerHighScore(int highScore)
    {
        if (!playerHighScore) return; 

        playerHighScore.text = "High score\r\n" + highScore.ToString();
    }
}
