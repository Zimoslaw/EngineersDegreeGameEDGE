using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonAi : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    public float ViewRadius = 16f;

    public Vector3 CurrentDestination = new(0, 0.1f, 0);

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        StartCoroutine("LookForDestinationWithDelay", 0.5f);
    }

    IEnumerator LookForDestinationWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            LookForPlayer();
            IsAtDestination();
        }
    ]

    void LookForPlayer()
    {
        Collider[] collidersInView = Physics.OverlapSphere(transform.position, ViewRadius, playerMask);

        foreach (Collider collider in collidersInView)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (collider.transform.postion - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToPlayer) < ViewRadius / 2)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, collider.transform.position);
                    if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                    {
                        ChangeDestination(collider.transform.position);
                        Debug.Log("Gracz zauważony");
                    }
                }
            }
        }
    }

    void IsAtDestination()
    {
        if (agent.remainingDistance < 0.1f)
        {
            ChangeDestination(LabyrinthGenerator.GetRandomCell());
        }
    }

    public void ChangeDestination(Vector3 destination)
    {
        if (!agent.IsUnityNull() && agent.isOnNavMesh)
        {
            agent.SetDestination(destination);
            Debug.Log("Nowy cel demona:" + agent.destination);
        }
    }
}
