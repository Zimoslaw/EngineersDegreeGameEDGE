using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonAi : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    LabyrinthGenerator labyrinth;

    int labyrinthSize = 5;

    public float ViewRadius = 16f;
    public float ViewAngle = 180f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Vector3 CurrentDestination;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        labyrinth = GameObject.FindWithTag("Labyrinth").GetComponent<LabyrinthGenerator>();
        if (!labyrinth.IsUnityNull())
            labyrinthSize = labyrinth.labyrinthSize;

        StartCoroutine("LookForDestinationWithDelay", 0.2f);
    }

    IEnumerator LookForDestinationWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            LookForPlayer();
            IsAtDestination();
            ChangeDestination();
        }
    }

    void LookForPlayer()
    {
        Collider[] collidersInView = Physics.OverlapSphere(transform.position, ViewRadius, playerMask);

        foreach (Collider collider in collidersInView)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Gracz w zasięgu");
                Vector3 directionToPlayer = (collider.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToPlayer) < ViewAngle / 2)
                {
                    Debug.Log("Gracz w polu widzenia");
                    float distanceToPlayer = Vector3.Distance(transform.position, collider.transform.position);
                    if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                    {
                        CurrentDestination = collider.transform.position;
                        Debug.Log("Gracz zauważony");
                    }
                }
            }
        }
    }

    void IsAtDestination()
    {
        if (Vector3.Distance(transform.position, CurrentDestination) < 0.1f)
        {
            Debug.Log("Demon u celu");
            int x = UnityEngine.Random.Range(0, labyrinthSize);
            int z = UnityEngine.Random.Range(0, labyrinthSize);
            CurrentDestination = new Vector3(x * 4, 0.1f, z * 4);
        }
    }

    public void ChangeDestination()
    {
        if (!agent.IsUnityNull() && agent.isOnNavMesh)
        {
            if(Vector3.Distance(agent.destination, CurrentDestination) > 0.1f)
            {
                agent.SetDestination(CurrentDestination);
                Debug.Log("Nowy cel demona:" + agent.destination);
            }
        }
    }
}
