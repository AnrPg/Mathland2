using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Basilisk, Wyvern };

    [SerializeField] private float speed;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private LayerMask whatIsGround, whatIsTarget;
    [SerializeField] private float sightRange, attackRange, checkRate;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private bool targetInSightRange, targetInAttackRange;
    private float nextCheck;
    private float agentSpeed;
    private Animator animator;

    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    private float walkPointRange;

    // Attacking
    private float timeBetweenAttacks;
    private bool alreadyAttacked;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed; // or agent.velocity;

        checkRate = UnityEngine.Random.Range(checkRate*0.8f, checkRate*1.2f);
    }

    public void Update()
    {
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;

            targetInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsTarget);
            targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsTarget);
            
            if (!targetInSightRange && !targetInAttackRange) Patrolling();
            if (targetInSightRange && !targetInAttackRange) ChaseTarget();
            if (targetInSightRange && targetInAttackRange) AttackTarget();
            if (!targetInSightRange && !targetInAttackRange); //Patrolling(); // TODO: Move towards place the target was last seen
        }
    }

    public void Patrolling()
    {
        if (!walkPointSet) searchWalkPoints();

        if (walkPointSet)
        {
            Debug.Log("Patrolling");
            agent.SetDestination(walkPoint);
            //agent.speed = agentSpeed;
            animator.SetInteger("moving", 1);//agent.speed);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            //transform.Translate(new Vector3(0, 0, Time.deltaTime * agentSpeed));
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            // Walkpoint reached
            if (distanceToWalkPoint.magnitude < agent.stoppingDistance) // or just 1f...
            {
                walkPointSet = false;
            }
        }
    }

    private void searchWalkPoints()
    {
        // Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x+randomX, transform.position.y, transform.position.z+randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    public void ChaseTarget()
    {
        Debug.Log("Chasing target");
        agent.SetDestination(playerTransform.position);
        //agent.speed = 2*agentSpeed;
        animator.SetInteger("moving", 1); //agent.speed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //transform.Translate(new Vector3(0, 0, Time.deltaTime * agentSpeed));
    }

    public void AttackTarget()
    {
        // Make sure this object doesn't move
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);

        Debug.Log("Attacking");

        if (!alreadyAttacked)
        {
            // TODO: Set attack animation here (depending on this object enum type)
            /*
            switch(enemyType)
                {
                    case EnemyType.Chicken:
                    {
                        break;
                    }
                    case EnemyType.Sprout:
                    {
                        break;
                    }
                    default:
                        break;
                }
            */
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
