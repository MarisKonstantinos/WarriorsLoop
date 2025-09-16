using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int highScore = 0;
    private int playerScore = 0;
    private GameObject player;
    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Initialize;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Initialize;
    }
    private void Start()
    {
        
    }

    /// <summary>
    /// Initializes the player and the score.
    /// </summary>
    private void Initialize(Scene scene, LoadSceneMode mode)
    {
        if(scene.name.Equals("Main level"))
        {
            playerScore = 0;
            UIManager.Instance.ScoreUpdate(playerScore);
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }

        if(scene.name.Equals("Main menu"))
        {
            LoadHighScore();
            MainMenuUIManager.Instance.UpdatePlayerHighScore(highScore);
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();  // important for WebGL to force save
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    #region Main game functionalities

    public void BackToMenu()
    {
        ToggleGamePause(false);
        SceneManager.LoadScene("Main menu");
    }

    public void RestartLevel()
    {
        if (playerScore > highScore)
            highScore = playerScore;
        playerScore = 0;
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentLevel.name);
        TogglePlayerInput(true);
        ToggleGamePause(false);
    }
    
    public GameObject GetPlayer()
    {
        if (!player) return null;

        return player;
    }

    public void SwitchInputActionMap(string actionMap)
    {
        if (player.TryGetComponent(out PlayerInput input))
        {
           input.SwitchCurrentActionMap(actionMap);
        }
    }

    public void TogglePlayerMovement(bool disableMovemet)
    {
        if (player.TryGetComponent(out PlayerMovement pMovement))
        {
            if(!disableMovemet)
                pMovement.DisableMovement();
            else
                pMovement.EnableMovement();
        }
    }

    public void TogglePlayerInput(bool toggle)
    {
        if (player.TryGetComponent(out PlayerInput input))
        {
            if (!toggle)
                input.DeactivateInput();
            else
                input.ActivateInput();
        }
    }

    public void PlayerDied()
    {
        if(playerScore > highScore)
        {
            highScore = playerScore;
            SaveHighScore();
        }
        UIManager.Instance.ToggleGameOverUI(true);
        StartCoroutine(delayPause());
    }

    private IEnumerator delayPause()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }

    public void EnemyDied(GameObject enemy, int score)
    {
        playerScore += score;
        if (playerScore > 999)
            playerScore = 999;
        UIManager.Instance.ScoreUpdate(playerScore);
        

    }

    public void ToggleGamePause(bool _pause)
    {
        if(_pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void HealPlayer(float value)
    {
        if(player.TryGetComponent(out HealthComponent playerHealth))
        {
            playerHealth.HealDamage(value);
        }
    }
    #endregion
}
