using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");

    [SerializeField] private LayerMask m_Enemy;
    [SerializeField] private LayerMask m_Ground;

    // Component
    private Animator m_Anim;
    private NavMeshAgent m_Agent;

    // Enemy
    private Transform m_Target;

    // Status
    [SerializeField] private float m_MaxHp;
    [SerializeField] private float m_Atk;
    [SerializeField] private float m_AtkRange;
    [SerializeField] private float m_Speed;

    private float m_CurrentHp;

    private Coroutine m_AttackCoroutine;

    public bool isAlive = true;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();

        m_CurrentHp = m_MaxHp;
        m_Agent.speed = m_Speed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isAlive) return;

        // Attack
        if (m_Target && m_AttackCoroutine == null)
        {
            m_Agent.SetDestination(m_Target.position);

            if (Vector3.Distance(transform.position, m_Target.position) <= m_AtkRange)
            {
                m_Agent.SetDestination(transform.position);
                m_AttackCoroutine = StartCoroutine(AttackCoroutine());
            }
        }

        // Idle when stop
        if (Vector3.Distance(transform.position, m_Agent.destination) <= 0.01f && m_AttackCoroutine == null)
        {
            m_Anim.Play(Idle);
        }

        // Left Click on Enemy
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_Enemy))
            {
                StopAttack();
                m_Target = hit.collider.transform;
                m_Anim.Play(Walk);

                m_Agent.SetDestination(m_Target.position);
            }
        }

        // Right Click on Ground
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Ground))
            {
                StopAttack();
                m_Target = null;
                m_Anim.Play(Walk);

                m_Agent.SetDestination(hit.point);
            }
        }
    }

    public void TakeDamage(Enemy attacker, float damage)
    {
        m_CurrentHp -= damage;

        if (m_CurrentHp <= 0)
        {
            m_Anim.Play(Die);
            isAlive = false;
        }
    }

    private void StopAttack()
    {
        if(m_AttackCoroutine != null) StopCoroutine(m_AttackCoroutine);
        m_AttackCoroutine = null;
    }

    private IEnumerator AttackCoroutine()
    {
        if (isAlive)
        {
            if (Vector3.Distance(transform.position, m_Target.position) <= m_AtkRange)
            {
                m_Anim.Play(Attack);
                yield return new WaitForSeconds(m_Anim.GetCurrentAnimatorStateInfo(0).length);

                Enemy enemy = m_Target.GetComponent<Enemy>();

                if (enemy && enemy.isAlive)
                {
                    enemy.TakeDamage(this, m_Atk);
                    m_AttackCoroutine = StartCoroutine(AttackCoroutine());
                }
            }
        }
    }
}