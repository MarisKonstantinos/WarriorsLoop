using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int playerScore = 0;
    [SerializeField] private GameObject player;
    [SerializeField] private Image playerScoreImage;
    
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
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        
    }

    public GameObject GetPlayer()
    {
        if (!player)
        {
            Debug.LogError("Returned null.");
            return null;
        }

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

    public void TogglePlayerInput(bool _input)
    {
        if (player.TryGetComponent(out PlayerInput input))
        {
            if (!_input)
            {
                Debug.LogError("Deactivating input.");
                input.DeactivateInput();
            }
            else
            {
                Debug.LogError("Activating input.");
                input.ActivateInput();
            }
        }
    }

    public void PlayerDied()
    {
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
        UIManager.Instance.ScoreUpdate(playerScore);
        StartCoroutine(DelayDestroyEnemy(enemy));

    }

    private IEnumerator DelayDestroyEnemy(GameObject enemy)
    {
        enemy.GetComponent<AnimatorController>().PlayDie();
        yield return new WaitForSeconds(2f);
        Destroy(enemy);
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
}
