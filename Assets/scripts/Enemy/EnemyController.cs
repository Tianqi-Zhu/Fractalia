using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{    public GameObject Bullet;    public GameObject Terminal;
    public float triggerRadius;
    public bool chaseOn;
    public float standingDistance;
    public float fireRate;    public float turnSpeed;    public float moveSpeed;    public float bulletSpeed;
    public float health;    public enum PatrolMode    {        Square,        Triangle,        None    }    public PatrolMode patrolMode;    public float patrolSideLen;    public int serialNo;    [TextArea(3, 10)]    public string terminalCenterText;    public string terminalLeftText;    public string terminalRightText;    public string terminalTitle;    public Transform[] firePoints;
    private PersistentManager Manager;    private Rigidbody rb;    private Transform player;
    private bool isSlain;
    private float timeToNextFire = 0f;
    private Vector3 campingPos;
    private int currMovePoint;
    private int movePointsNo;    private Vector3[] movePoints;
    private enum State    {        Patrolling,        Attacking,        Returning    }
    private State state;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        isSlain = Manager.enemyBoolArr[serialNo];        if (isSlain)        {            Die();        }        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        campingPos = transform.position;
        currMovePoint = 0;
        state = State.Patrolling;
        switch (patrolMode)        {            case PatrolMode.None:                movePoints = new Vector3[0];                break;            case PatrolMode.Square:                movePoints = new Vector3[4];                movePoints[0] = new Vector3(campingPos.x - patrolSideLen, campingPos.y, campingPos.z - patrolSideLen);                movePoints[1] = new Vector3(campingPos.x - patrolSideLen, campingPos.y, campingPos.z + patrolSideLen);                movePoints[2] = new Vector3(campingPos.x + patrolSideLen, campingPos.y, campingPos.z + patrolSideLen);                movePoints[3] = new Vector3(campingPos.x + patrolSideLen, campingPos.y, campingPos.z - patrolSideLen);                break;            case PatrolMode.Triangle:                movePoints = new Vector3[3];                movePoints[0] = new Vector3(campingPos.x , campingPos.y, campingPos.z - patrolSideLen / 1.7321f);                movePoints[1] = new Vector3(campingPos.x - patrolSideLen / 2, campingPos.y, campingPos.z + patrolSideLen / 1.7321f);                movePoints[2] = new Vector3(campingPos.x + patrolSideLen / 2, campingPos.y, campingPos.z + patrolSideLen / 1.7321f);                break;        }

        movePointsNo = movePoints.Length;
    }

    void Update()    {        float distanceToPlayer = Vector3.Distance(player.position, transform.position);        Vector3 directionToPlayer = (player.position - transform.position).normalized;        switch (state)        {            case State.Patrolling:                if (movePointsNo > 0)                {                    Patrol();                }                if (distanceToPlayer <= triggerRadius)                {                    state = State.Attacking;                }                break;            case State.Attacking:                // Turn to face the player                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer); // Map Vector3 direction to its corresponding rotation                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); // slerp smoothes the angle rotation                // Raycast check whether player is in line of sight                RaycastHit playerHit;                if (Physics.Raycast(transform.position, directionToPlayer, out playerHit, triggerRadius)) // returns true if hit something                {                    if (playerHit.transform.tag == "Player") // If can see player:                    {                        if (Time.time >= timeToNextFire) // Fire                        {                            timeToNextFire = Time.time + 1 / fireRate;                            Fire();                        }                        if (distanceToPlayer > standingDistance && chaseOn) // If player is beyond standing distance and chase is on                        {                            rb.velocity = directionToPlayer * moveSpeed; // Move towards player                        }                    } else                    {                        state = State.Returning;                    }                }                if (distanceToPlayer > triggerRadius)                {                    state = State.Returning;                }                break;            case State.Returning:                if (distanceToPlayer <= triggerRadius)                {                    state = State.Attacking;                }                // Raycast check whether camping pos is in line of sight                Vector3 directionToCampingPos = (campingPos - transform.position).normalized;                float distanceToCampingPos = Vector3.Distance(campingPos, transform.position);                RaycastHit campingPosHit;                if (Physics.Raycast(transform.position, directionToCampingPos, out campingPosHit, distanceToCampingPos))                {                    // True: Hit anything: Line of sight not clear, teleport back                    transform.position = campingPos;                    // Add in particle effect for teleporting back                } else                {                    // False: Hit nothing: Line of sight clear, move back                    rb.velocity = directionToCampingPos * moveSpeed;                }                if (distanceToCampingPos < 0.1f)                {                    state = State.Patrolling;                }                break;        }    }    void Patrol()    {        transform.position = Vector3.MoveTowards(transform.position, movePoints[currMovePoint], moveSpeed * Time.deltaTime);        if (Vector3.Distance(transform.position, movePoints[currMovePoint]) < 0.1f)        {            currMovePoint = (currMovePoint + 1) % movePointsNo;        }    }    void Fire()    {        foreach (var firePoint in firePoints)        {            GameObject bulletInstance = Instantiate(Bullet, firePoint.position, firePoint.rotation);            bulletInstance.transform.localScale = transform.localScale * 0.33f;            Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();            bulletRb.velocity = firePoint.forward * bulletSpeed;        }    }

    public void TakeDamage(float amount)    {        health -= amount;        if (health <= 0f)        {            Die();        }    }

    private void Die()    {        Destroy(gameObject);        SpawnTerminal();        Manager.enemyBoolArr[serialNo] = true;    }

    private void SpawnTerminal()    {        GameObject terminal = Instantiate(Terminal, campingPos, Quaternion.identity);        terminal.transform.localScale = transform.localScale;        TerminalController controller = terminal.GetComponent<TerminalController>();        controller.serialNo = serialNo;        controller.centerText = terminalCenterText;        controller.leftText = terminalLeftText;        controller.rightText = terminalRightText;        controller.title = terminalTitle;    }

    // Visualize the trigger sphere in editor
    void OnDrawGizmosSelected()    {        Gizmos.color = Color.red;        Gizmos.DrawWireSphere(transform.position, triggerRadius);    }

}
