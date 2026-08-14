using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDamageable
{
    public float maxHealth { get; set; }

    public float currentHealth { get; set; }

    public void Damage(DamageInfo info)
    {

    }

    public void Die()
    {

    }
}
