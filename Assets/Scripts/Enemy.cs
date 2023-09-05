using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Avatar
{
    [SerializeField] private float HomeSickDistance = 10;

    private Vector3 m_SpawnPosition;

    protected override void Start()
    {
        base.Start();

        m_SpawnPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, m_SpawnPosition) > HomeSickDistance)
        {
            StopAttack();
            SetTarget(null);
            Move(m_SpawnPosition);
        }
    }

    public override void TakeDamage(Avatar attacker, float damage)
    {
        base.TakeDamage(attacker, damage);
        SetTarget(attacker);
    }
}