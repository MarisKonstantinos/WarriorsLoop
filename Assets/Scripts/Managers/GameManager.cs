using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int playerScore = 0;
    

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

    public void PlayerDied()
    {
        UIManager.Instance.ToggleGameOverUI(true);
        StartCoroutine(delayPause());
        
    }

    private IEnumerator delayPause()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0f;
    }

    public void EnemyDied(GameObject enemy, int score)
    {
        
        playerScore += score;
        Debug.LogError("Player score: " + playerScore);
        StartCoroutine(DelayDestroyEnemy(enemy));

    }

    private IEnumerator DelayDestroyEnemy(GameObject enemy)
    {
        //Play anim
        yield return new WaitForSeconds(1);
        Destroy(enemy);
    }
}
