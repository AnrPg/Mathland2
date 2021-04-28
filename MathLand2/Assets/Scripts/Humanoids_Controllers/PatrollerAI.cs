using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollerAI : MonoBehaviour
{
    public enum EnemyType { Centaur, Troll, Basilisk, Wyvern };
    [Range(10.0f, 200.0f)]
    [SerializeField] private float neighborRadius = 150.0f;

    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float speed;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private LayerMask whatIsGround, whatIsTarget;
    [SerializeField] private float sightRange, attackRange, checkRate;

    private GameObject _fireball;
    private bool _alive;
    private NavMeshAgent agent;
    private Transform playerTransform;
    private bool targetInSightRange, targetInAttackRange;
    private float nextCheck;
    private float agentSpeed;
    private Vector3 lastSeenPosition;
    //private Animator animator;

    // Patrolling
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool patrolling;
    //private float walkPointRange;

    // Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttacked;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed; // or agent.velocity;
        lastSeenPosition = playerTransform.position;

        checkRate = UnityEngine.Random.Range(checkRate * 0.8f, checkRate * 1.2f);
        _alive = true;
    }

    public void Update()
    {
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < agent.stoppingDistance)
        {
            walkPointSet = false;
            patrolling = false;
        }

        //if ((Time.time > nextCheck) && _alive)
        //{
            //nextCheck = Time.time + checkRate;

            Vector3 displacementForSight = transform.position + transform.forward * sightRange;
            Vector3 displacementForAttack = transform.position + transform.forward * attackRange;
            targetInSightRange = Physics.CheckSphere(displacementForSight, sightRange, whatIsTarget);
            targetInAttackRange = Physics.CheckSphere(displacementForAttack, attackRange, whatIsTarget);

            if (!targetInSightRange && !targetInAttackRange) Patrolling();
            if (targetInSightRange && !targetInAttackRange) ChaseTarget();
            //if (targetInSightRange && targetInAttackRange) AttackTarget();
            //if (!targetInSightRange && !targetInAttackRange) ;// Patrolling(lastSeenPosition); // TODO: Move towards place the target was last seen
        //}
    }

    public void Patrolling()
    {
        if (!agent.isOnNavMesh)
        {
            Destroy(this.gameObject);
        }

        if (!walkPointSet) searchWalkPoints();

        if (agent.pathPending)
        {
            return;
        }

        if (walkPointSet)
        {
            agent.speed = speed;

            if (!patrolling)
            {
                patrolling = true;
                agent.SetDestination(walkPoint);
                agent.speed = speed;
                //animator.SetInteger("moving", 1);//agent.speed);
            }
        }
    }

    /*
    public void Patrolling(Vector3 walkPoint)
    {
        //Debug.Log("8");
        if (!agent.isOnNavMesh)
        {
            Destroy(this.gameObject);
        }

        //Debug.Log("9");
        if (walkPoint == null) Patrolling(); // if walkPoint is null then call the method with no argument

        //Debug.Log("10");
        if (agent.pathPending)
        {
            return;
        }

        //Debug.Log("11");
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            //Debug.Log("12");
            walkPointSet = true;
            this.walkPoint = walkPoint;
            Debug.Log("Walk point set by user");
        }

        //Debug.Log("13");
        if (walkPointSet)
        {
            //Debug.Log("14");
            //if (!patrolling)
            //    {
            patrolling = true;
            Debug.Log("Patrolling");
            agent.SetDestination(walkPoint);
            agent.speed = speed;
            //animator.SetInteger("moving", 1);//agent.speed);
            //    }

            
        }
    }
    */

    private void searchWalkPoints()
    {        
        Vector2 randDiskPoint = neighborRadius * Random.insideUnitCircle + new Vector2(transform.position.x, transform.position.z);
        walkPoint = GetPointOnTerrain(randDiskPoint.x, randDiskPoint.y);

        walkPointSet = true;
        Debug.Log("Walk point set while searching for it");
    }

    public void ChaseTarget()
    {
        Debug.Log("Chasing target");
        patrolling = false;
        agent.SetDestination(lastSeenPosition - transform.forward);
        //Vector3 a = lastSeenPosition - transform.forward;
        //Debug.Log("lastSeenPosition: " + lastSeenPosition + "\tlastSeenPosition - transform.forward: " + a);
        //agent.speed = 1.5f * speed;
        //animator.SetInteger("moving", 1); //agent.speed);
        //transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //transform.Translate(new Vector3(0, 0, Time.deltaTime * agentSpeed));
        lastSeenPosition = playerTransform.position;
    }

    /*
    public void AttackTarget()
    {
        // Make sure this object doesn't move
        patrolling = false;
        //agent.SetDestination(transform.position); // or playerTransforms.position...
        transform.LookAt(playerTransform);

        Debug.Log("Attacking");

        //Debug.Log("18");
        if (!alreadyAttacked)
        {
            //Debug.Log("19");
            // TODO: Set attack animation here (depending on this object enum type)
            // /*
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
            
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            // *\

            if (_fireball == null)
            {
                _fireball = Instantiate(fireballPrefab) as GameObject;
                _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
                _fireball.transform.rotation = transform.rotation;

                Fireball fireballComponent = _fireball.GetComponent<Fireball>();
                if (fireballComponent != null)
                {
                    //Debug.Log("20");
                    _fireball.GetComponent<Fireball>().setTargetPoint(playerTransform.position);
                    _fireball.GetComponent<Fireball>().setShooter(transform.gameObject);
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    */

    private void OnDrawGizmosSelected()
    {        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position+ transform.forward*attackRange, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * sightRange, sightRange);
    }

    public bool ToggleAliveState()
    {
        bool lastState = _alive;
        _alive = !_alive;
        return lastState;
    }

    private Terrain GetClosestCurrentTerrain(Vector3 playerPos)
    {
        //Get all terrain
        Terrain[] terrains = Terrain.activeTerrains;

        //Make sure that terrains length is ok
        if (terrains.Length == 0)
            return null;

        //If just one, return that one terrain
        if (terrains.Length == 1)
            return terrains[0];

        //Get the closest one to the player
        float lowDist = (terrains[0].GetPosition() - playerPos).sqrMagnitude;
        var terrainIndex = 0;

        for (int i = 1; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            Vector3 terrainPos = terrain.GetPosition();

            //Find the distance and check if it is lower than the last one then store it
            var dist = (terrainPos - playerPos).sqrMagnitude;
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return terrains[terrainIndex];
    }

    private Vector3 GetPointOnTerrain(float xCord, float zCord)
    {
        // make Vector3 with global coordinates xVal and zVal (Y doesn't matter):
        Vector3 signPosition = new Vector3(xCord, 0, zCord);
        // Retrieve the terrain that is under the point called "signPosition"
        Terrain activeTerrain = GetClosestCurrentTerrain(signPosition);
        // set the Y coordinate according to terrain Y at that point:
        signPosition.y = activeTerrain.SampleHeight(signPosition) + activeTerrain.GetPosition().y;
        // you probably want to create the object a little above the terrain:
        signPosition.y += 0.5f; // move position 0.5 above the terrain
        
        return signPosition;
    }

    /*
    private GameObject SpawnAboveTerrain(Object prefab, float xCord, float zCord, string emptyObjectName= "PatrolTarget")
    {
        // make Vector3 with global coordinates xVal and zVal (Y doesn't matter):
        Vector3 signPosition = new Vector3(xCord, 0, zCord);
        // Retrieve the terrain that is under the point called "signPosition"
        Terrain activeTerrain = GetClosestCurrentTerrain(signPosition);
        // set the Y coordinate according to terrain Y at that point:
        signPosition.y = activeTerrain.SampleHeight(signPosition) + activeTerrain.GetPosition().y;
        // you probably want to create the object a little above the terrain:
        signPosition.y += 0.5f; // move position 0.5 above the terrain
        if (prefab != null)
        {
            return Instantiate(prefab, signPosition, Quaternion.identity) as GameObject;
        }
        else
        {
            GameObject emptyObject = new GameObject(emptyObjectName);
            emptyObject.transform.position = signPosition;
            emptyObject.transform.rotation = Quaternion.identity;
            return emptyObject;
        }
    }
    */
}
