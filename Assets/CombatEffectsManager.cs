using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffectsManager : MonoBehaviour
{
    public static CombatEffectsManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void HitPause(float duration)
    {
        StartCoroutine(DoHitPause(duration));
    }

    private IEnumerator DoHitPause(float duration)
    {
        Time.timeScale = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
