using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller2 : MonoBehaviour
{
    public enum EnemyType { Chicken, Sprout };

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform eye;
    [SerializeField] private float viewAngle;
    [SerializeField] private float ViewDistance;
    [SerializeField] private float attackRange;

    private GameObject player;
    private Vector3 lastSeenPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")?[0];
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, player.transform.position);
        lastSeenPosition = player.transform.position; // this only initializes "lastSeenPosition" !

        if (currentDistance < attackRange)
        {
            if (targetIsInFront() && targetIsInLineOfSight())
            {
                // If this object loses sight with the target then it moves towards the place the target was last seen
                lastSeenPosition = player.transform.position;

                // TODO: (approach? and) attack target
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
            }
            else
            {
                // TODO: move towards "lastSeenPosition"
            }
        }
    }

    private bool targetIsInFront()
    {
        Vector3 directionOfTarget = transform.position - player.transform.position;
        float angleWithPlayer = Vector3.Angle(transform.forward, directionOfTarget);

        if ((Mathf.Abs(angleWithPlayer) > viewAngle ) && (Mathf.Abs(angleWithPlayer) < 360-viewAngle ))
        {
            //Debug.DrawLine(transform.position, player.transform.position, Color.green);
            return true;
        }

        return false;
    }

    private bool targetIsInLineOfSight()
    {
        RaycastHit hit;
        Vector3 directionOfTarget = transform.position - player.transform.position; // TODO: might need to change the operands order...
        
        if (Physics.Raycast(transform.position, directionOfTarget, out hit, ViewDistance))
        {
            //if(hit.transform.gameObject.tag == "Player")
            //{
                Debug.Log(gameObject.name + " CAN see " + hit.transform.gameObject);
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
                return true;
            //}

        }

        return false;
    }
}
