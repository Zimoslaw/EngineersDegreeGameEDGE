using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonAi : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    LabyrinthGenerator labyrinth;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] demonSounds;
    bool isChasingPlayer = false;

    int labyrinthSize = 5;
    int randomSoundtimer = 0;
    int randomSoundInterval = 1;
    int heardSoundTimer = 50;
    int sawSoundTimer = 10;

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

        randomSoundInterval = UnityEngine.Random.Range(3, 20);

        StartCoroutine(nameof(LookForDestinationWithDelay), 0.2f);
        StartCoroutine(nameof(ListenToSteps), 2f);
        StartCoroutine(nameof(PlayRandomSound));
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

    IEnumerator PlayRandomSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            randomSoundtimer++;
            if (randomSoundtimer >= randomSoundInterval)
            {
                audioSource.clip = demonSounds[UnityEngine.Random.Range(2, demonSounds.Length)];
                audioSource.Play();
                randomSoundInterval = UnityEngine.Random.Range(3, 20);
                randomSoundtimer = 0;
                Debug.Log("Demon wydał " + audioSource.clip.name + " dźwięk");
            }
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
                    sawSoundTimer++;
                    if (sawSoundTimer >= 10)
                    {
                        audioSource.clip = demonSounds[1];
                        audioSource.Play();
                        sawSoundTimer = 0;
                    }
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
                float distanceToPlayer = Vector3.Distance(transform.position, collider.transform.position);
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    CurrentDestination = collider.transform.position;
                    isChasingPlayer = true;
                    sawSoundTimer++;
                    if (sawSoundTimer >= 50)
                    {
                        audioSource.clip = demonSounds[1];
                        audioSource.Play();
                        sawSoundTimer = 0;
                    }
                    Debug.Log("Gracz zauważony");
                }
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
                    float z = collider.transform.position.z + UnityEngine.Random.Range(-4f, 4f);
                    float x = collider.transform.position.x + UnityEngine.Random.Range(-4f, 4f);
                    CurrentDestination = new Vector3(x, 0.1f, z);
                    heardSoundTimer++;
                    if (heardSoundTimer >= 50)
                    {
                        audioSource.clip = demonSounds[0];
                        audioSource.Play();
                        heardSoundTimer = 0;
                    }
                    Debug.Log("Gracz usłyszany");
                }
            }
        }
    }

    public void ChangeDestination()
    {
        if (!agent.IsUnityNull() && agent.isOnNavMesh)
        {
            if(Vector3.Distance(agent.destination, CurrentDestination) > 0.2f)
            {
                agent.SetDestination(CurrentDestination);
                Debug.Log("Nowy cel demona:" + agent.destination);
            }
        }
    }
}
