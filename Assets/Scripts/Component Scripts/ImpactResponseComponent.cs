using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactResponseComponent : MonoBehaviour
{
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip hitSound;

    public void PlayHitFeedback()
    {
        if (hitSound)
            SoundManager.Instance.PlaySFX(hitSound, 0.1f);
    }

    public void PlayBreakFeedback()
    {
        if (breakSound)
            SoundManager.Instance.PlaySFX(breakSound, 0.1f);
    }
}
