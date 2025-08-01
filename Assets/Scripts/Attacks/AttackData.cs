using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Attacks/Attack Data")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public float damage;
    public float cooldown;
    public float range;
    public float knockbackPower;
}
