using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlashing : MonoBehaviour
{
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField, Range(0.0f, 1.0f)] float speed;
    private SpriteRenderer spriteRenderer;
    public bool enabled = false;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!enabled || !spriteRenderer) return;
        
        spriteRenderer.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    }

    public void EnableSpriteFlashing()
    {
        enabled = true;
    }
}