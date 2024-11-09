using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonAi : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    LabyrinthGenerator labyrinth;
    bool isChasingPlayer = false;

    int labyrinthSize = 5;

    public float ViewRadius = 16f;
    public float ViewAngle = 180f;
    public float HearRadius = 12f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Vector3 CurrentDestination;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        labyrinth = GameObject.FindWithTag("Labyrinth").GetComponent<LabyrinthGenerator>();
        if (!labyrinth.IsUnityNull())
            labyrinthSize = labyrinth.labyrinthSize;

        StartCoroutine(nameof(LookForDestinationWithDelay), 0.2f);
        StartCoroutine(nameof(ListenToSteps), 2f);
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

    IEnumerator ListenToSteps(float delay)
    {
        while (!isChasingPlayer)
        {
            yield return new WaitForSeconds(delay);
            ListenForPlayer();
        }
    }

    void LookForPlayer()
    {
		GameObject lamp = GameObject.FindGameObjectWithTag("PlayerLamp");
		if (!lamp.IsUnityNull())
		{
			if (lamp.GetComponent<KeroseneLamp>().KeroseseneLevel <= 0)
            {
				GameObject player = GameObject.FindGameObjectWithTag("Player");
                if(!player.IsUnityNull())
                {
                    CurrentDestination = player.transform.position;
                    isChasingPlayer = true;
                    Debug.Log("Podążam za graczem bez nafty");
                    return;
                }
			}
		}

		// Looking for player in view range
		Collider[]  collidersInView = Physics.OverlapSphere(transform.position, ViewRadius, playerMask);

        foreach (Collider collider in collidersInView)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (collider.transform.position - transform.position).normalized;
                //if (Vector3.Angle(transform.forward, directionToPlayer) < ViewAngle / 2)
                //{
                    float distanceToPlayer = Vector3.Distance(transform.position, collider.transform.position);
                    if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                    {
                        CurrentDestination = collider.transform.position;
                        isChasingPlayer = true;
                        Debug.Log("Gracz zauważony");
                    }
                //}
            }
        }
    }

    void IsAtDestination()
    {
        if (Vector3.Distance(transform.position, CurrentDestination) < 0.1f)
        {
            Debug.Log("Demon u celu");
            isChasingPlayer = false;
            int x = UnityEngine.Random.Range(0, labyrinthSize);
            int z = UnityEngine.Random.Range(0, labyrinthSize);
            CurrentDestination = new Vector3(x * 4, 0.1f, z * 4);
        }
    }

    void ListenForPlayer()
    {
        // Looking for moving player in hearing range
        Collider[] collidersInHear = Physics.OverlapSphere(transform.position, HearRadius, playerMask);

        foreach (Collider collider in collidersInHear)
        {
            if (collider.CompareTag("Player"))
            {
                if (Vector3.Distance(Vector3.zero, collider.GetComponent<Rigidbody>().velocity) > 0.1f)
                {
                    Debug.Log("Gracz usłyszany");
                    float z = collider.transform.position.z + UnityEngine.Random.Range(-4f, 4f);
                    float x = collider.transform.position.x + UnityEngine.Random.Range(-4f, 4f);
                    CurrentDestination = new Vector3(x, 0.1f, z);
                }
            }
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
