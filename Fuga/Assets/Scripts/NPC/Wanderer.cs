using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Wanderer : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;
    public List<AnimationClip> walkAnimations;
    public List<float> walkOffsets;
    public Transform offset;

    private NavMeshAgent agent;
    public Animator animator;
    public SkinnedMeshRenderer meshRenderer;
    private float timer;
    private List<Rigidbody> rigidbodies;
    private bool dead = false;
    public Transform hand;
    public List<GameObject> handItems;
    public List<Color> modelColors;
    private void Awake()
    {
        rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        int index = Random.Range(0, walkAnimations.Count);
        AnimationClip randomWalk = walkAnimations[index];
        offset.localPosition = new Vector3(0, walkOffsets[index], 0);
        
        overrideController["Walk"] = randomWalk;
        animator.runtimeAnimatorController = overrideController;

        if (randomWalk.name.Contains("With"))
            Instantiate(handItems[Random.Range(0, handItems.Count)], hand.position, hand.rotation, hand);
        
        meshRenderer.material = new Material(meshRenderer.material);
        meshRenderer.material.color = modelColors[Random.Range(0, modelColors.Count)];
        
        timer = wanderInterval;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval && !dead)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
        
        if (agent.velocity.magnitude > 0.2f)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            dead = true;
            agent.enabled = false;
            animator.enabled = false;

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
            }
        }
    }
}
