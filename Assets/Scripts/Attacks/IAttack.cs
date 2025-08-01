using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack 
{
    void Execute(Vector2 attackPoint, AttackData attack);
}
