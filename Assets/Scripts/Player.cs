using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : Avatar
{
    [SerializeField] private LayerMask m_Enemy;
    [SerializeField] private LayerMask m_Ground;

    protected override void Update()
    {
        base.Update();

        // Left Click on Enemy
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Enemy))
            {
                StopAttack();

                Avatar target = hit.collider.GetComponent<Avatar>();
                SetTarget(target);
            }
        }

        // Right Click on Ground
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Ground))
            {
                StopAttack();
                SetTarget(null);
                Move(hit.point);
            }
        }
    }
}