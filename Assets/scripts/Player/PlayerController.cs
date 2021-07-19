using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed;
    public float sideSpeed;

    public GameObject Bullet;
    public Transform firePoint;    public float fireRate = 4f;
    public float bulletSpeed = 60f;

    public static float maxHealth = 80f;
    public static int maxLives = 3;
    public static float currentHealth = maxHealth;
    public static int currentLives = maxLives;
    public HealthBar healthBar;
    public LifeDots lifeDots;

    private Rigidbody rb;
    private PersistentManager Manager;
    private float timeToNextFire = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
    }
    private void FixedUpdate()
    {        if (Input.GetKey("space")) { rb.velocity = transform.forward * forwardSpeed;}
        else if (Input.GetKey("v")) { rb.velocity = -transform.forward * sideSpeed;}
        else if (Input.GetKey("w")) { rb.velocity = transform.up * sideSpeed;}
        else if (Input.GetKey("s")) { rb.velocity = -transform.up * sideSpeed;}
        else if (Input.GetKey("a")) { rb.velocity = -transform.right * sideSpeed;}
        else if (Input.GetKey("d")) { rb.velocity = transform.right * sideSpeed;}

        if (Input.GetKey(KeyCode.Return))        {            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);        }

        if (Input.GetButton("Fire3") && Time.time >= timeToNextFire)
        {
            timeToNextFire = Time.time + 1 / fireRate;
            Fire();
        }
    }    public void TakeDamage(float amount)    {        currentHealth -= amount;        healthBar.SetHealth(currentHealth);        Debug.Log("Player health " + currentHealth + " Player lives " + currentLives);        if (currentHealth <= 0f)        {            PartialDeath();        }    }

    void PartialDeath()    {        lifeDots.DarkenDot();        currentLives = currentLives - 1;        if (currentLives <= 0)        {            CompleteDeath();        } else        {            SceneManager.LoadScene(1);            currentHealth = maxHealth;            Debug.Log("Player health " + currentHealth + " Player lives " + currentLives);        }    }

    void CompleteDeath()    {        Manager.ResetGame();        SceneManager.LoadScene(1);        currentHealth = maxHealth;        currentLives = maxLives;    }


    void Fire()    {        GameObject bulletInstance = Instantiate(Bullet, firePoint.position, firePoint.rotation);        bulletInstance.transform.localScale = transform.localScale * 0.19f;        bulletInstance.GetComponent<Rigidbody>().velocity = firePoint.forward * bulletSpeed;    }
}
