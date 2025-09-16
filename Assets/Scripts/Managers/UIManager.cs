using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI playerScoreTMP;
    [SerializeField] private GameObject abilityDescriptionPanel;
    private void Awake()
    {
        if(Instance != null &&  Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ToggleGameOverUI(bool toggle)
    {
        gameOverUI.SetActive(toggle);
    }

    public void ScoreUpdate(int score)
    {
        if (!playerScoreTMP) return;

        playerScoreTMP.text = score.ToString();
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        if (!abilityDescriptionPanel) return;
        
        if (context.performed)
        {
            GameManager.Instance.ToggleGamePause(true);
            GameManager.Instance.TogglePlayerMovement(false);
            abilityDescriptionPanel.SetActive(true);
        }
        else
        {
            GameManager.Instance.ToggleGamePause(false);
            GameManager.Instance.TogglePlayerMovement(true);
            abilityDescriptionPanel.SetActive(false);
        }
    }
}
