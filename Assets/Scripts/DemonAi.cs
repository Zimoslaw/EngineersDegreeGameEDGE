using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonAi : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    public Vector3 CurrentDestination = new(0, 0.1f, 0);

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
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
