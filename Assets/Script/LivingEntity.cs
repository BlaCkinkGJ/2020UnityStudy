using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start() // virtual 키워드가 없으면 override가 된다.
    {
        health = startingHealth;
    }
    public void TakeHit(float damage, RaycastHit hit)
    {
        // Do some stuff here with hit variable
        // 발사체가 적을 맞춘 지점
        // 파티클 오브젝트의 생성 등
        TakeDamage(damage);
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
}
