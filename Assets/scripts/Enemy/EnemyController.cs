using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{    public GameObject Bullet;    public GameObject Terminal;
    public float triggerRadius;
    public float fireRate;    public float turnSpeed;    public float bulletSpeed;
    public float health;    public int serialNo;    [TextArea(3, 10)]    public string terminalCenterText;    public string terminalLeftText;    public string terminalRightText;    public string terminalTitle;    public Transform[] firePoints;
    private PersistentManager Manager;    private Rigidbody rb;    private Transform player;
    private bool isSlain;
    private float timeToNextFire = 0f;
    private Vector3 terminalPos;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        isSlain = Manager.enemyBoolArr[serialNo];
        terminalPos = transform.position;

        if(isSlain)        {            Die();        }
    }

    void Update()    {        float distanceToPlayer = Vector3.Distance(player.position, transform.position);        if (distanceToPlayer <= triggerRadius)        {            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer); // Map Vector3 direction to its corresponding rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); // slerp smoothes the angle rotation            if (Time.time >= timeToNextFire)            {                timeToNextFire = Time.time + 1 / fireRate;                Fire();            }        }    }    void Fire()    {        foreach (var firePoint in firePoints)        {            GameObject bulletInstance = Instantiate(Bullet, firePoint.position, firePoint.rotation);            bulletInstance.transform.localScale = transform.localScale * 0.33f;            Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();            bulletRb.velocity = firePoint.forward * bulletSpeed;        }    }

    public void TakeDamage(float amount)    {        health -= amount;        if (health <= 0f)        {            Die();        }    }

    private void Die()    {        Destroy(gameObject);        SpawnTerminal();        Manager.enemyBoolArr[serialNo] = true;    }

    private void SpawnTerminal()    {        GameObject terminal = Instantiate(Terminal, terminalPos, Quaternion.identity);        terminal.transform.localScale = transform.localScale;        TerminalController controller = terminal.GetComponent<TerminalController>();        controller.serialNo = serialNo;        controller.centerText = terminalCenterText;        controller.leftText = terminalLeftText;        controller.rightText = terminalRightText;        controller.title = terminalTitle;    }

    // Visualize the trigger sphere in editor
    void OnDrawGizmosSelected()    {        Gizmos.color = Color.red;        Gizmos.DrawWireSphere(transform.position, triggerRadius);    }

}
