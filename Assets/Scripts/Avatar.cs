using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class Avatar : MonoBehaviour
{
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Walk = Animator.StringToHash("Walk");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Die = Animator.StringToHash("Die");

    // Component
    protected Animator m_Anim;
    protected NavMeshAgent m_Agent;

    // Enemy
    private Avatar m_Target;

    // Status
    [SerializeField] protected float hp;
    [SerializeField] protected float atk;
    [SerializeField] protected float atkRange;
    [SerializeField] protected float spd;

    public bool isAlive = true;

    private Coroutine m_AttackCoroutine;
    
    protected virtual void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();

        m_Agent.speed = spd;
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        // Attack
        if (m_Target && m_AttackCoroutine == null)
        {
            m_Agent.SetDestination(m_Target.transform.position);

            if (Vector3.Distance(transform.position, m_Target.transform.position) <= atkRange)
            {
                m_Agent.SetDestination(transform.position);
                m_AttackCoroutine = StartCoroutine(AttackCouroutine());
            }
        }

        // Idle when stoping
        if (Vector3.Distance(transform.position, m_Agent.destination) <= m_Agent.stoppingDistance && m_AttackCoroutine == null)
        {
            m_Anim.Play(Idle);
        }
    }

    public virtual void TakeDamage(Avatar attacker, float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            m_Anim.Play(Die);
            isAlive = false;

            StartCoroutine(LoadSceneDelay());
        }
    }

    private IEnumerator LoadSceneDelay()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Battle");
    }

    public void Move(Vector3 position)
    {
        m_Anim.Play(Walk);

        m_Agent.SetDestination(position);
    }

    public void StopAttack()
    {
        if (m_AttackCoroutine != null) StopCoroutine(m_AttackCoroutine);
        m_AttackCoroutine = null;
    }

    public void SetTarget(Avatar target) 
    {
        m_Target = target;
    }

    private IEnumerator AttackCouroutine()
    {
        if (m_Target.isAlive && isAlive)
        {
            if (isAlive)
            {
                transform.LookAt(m_Target.transform);
                m_Anim.Play(Attack);
            }
            yield return new WaitForSeconds(m_Anim.GetCurrentAnimatorStateInfo(0).length);

            if (isAlive)
            {
                m_Target.TakeDamage(this, atk);
                if (Vector3.Distance(transform.position, m_Target.transform.position) <= atkRange)
                {
                    m_AttackCoroutine = StartCoroutine(AttackCouroutine());
                }
                else
                {
                    m_AttackCoroutine = null;
                    m_Anim.Play(Walk);
                    m_Agent.SetDestination(m_Target.transform.position);
                }
            }
        }
        else
        {
            m_Anim.Play(Idle);
        }
    }
}
