using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity 
{
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 1.5f;
    float timeBetweenAttacks = 1.0f;
    float damage = 1.0f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;

        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            // vector distance 사용 금지 ==> 어마 무시하게 cost가 든다.
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                // attackDi + 를 해야지 적이 표면을 공격할 것이다.
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false; // UpdatePath의 에러를 발생 시킬 수 있다.

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3.0f;
        float percent = 0.0f;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = 4 * (-Mathf.Pow(percent, 2) + percent); // y = 4(-x^2 + x) >> 튕기도록 한다.
            // interpolation이 0이면 originalPosition에 1이면 attackPosition에 있을 것이다.
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation); // Lerp 메소드는 두 벡터 사이의 비례값(0 ~ 1)의 내분점 지점을 반환

            yield return null; // Update 메소드로 바로 넘어가서, Update 동작이 완료된다. Update 메소드가 종료되면 다음 루프가 진행
        }

        pathfinder.enabled = true;
        currentState = State.Chasing;
        skinMaterial.color = originalColor;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; // 적절하게 설정하면 됩니다.
                                   // 성능면에서 훨씬 좋다.

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2.0f);
                if (!dead) // 이게 없으면 아래가 계산되면 NULL exception이 발생할 수 있다.
                {
                    pathfinder.SetDestination(targetPosition); // Cost가 큰 동작이다.
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
