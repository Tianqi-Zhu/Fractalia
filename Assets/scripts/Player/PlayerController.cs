using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public LifeDots lifeDots;    public GameObject shieldTimerUI;    [HideInInspector]    public float shieldTimer = 0f;

    private Rigidbody rb;
    private PersistentManager Manager;    private Collectibles collectibles;
    private float timeToNextFire = 0f;
    private bool isInvincible;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();        collectibles = GameObject.FindWithTag("Collectibles").GetComponent<Collectibles>();
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
        }        if (Input.GetKey(KeyCode.LeftShift))        {            UseHealthPotion();        }        if (Input.GetKey(KeyCode.RightShift))        {            UseShield();        }

        if (isInvincible)        {            shieldTimer -= Time.deltaTime;            shieldTimerUI.GetComponent<Text>().text = shieldTimer.ToString("F2");            if (shieldTimer <= 0f)            {                isInvincible = false;                shieldTimerUI.SetActive(false);            }        }

    }    public void TakeDamage(float amount)    {        Debug.Log("Player health " + currentHealth + " Player lives " + currentLives);        if (!isInvincible)        {            currentHealth -= amount;            healthBar.SetHealth(currentHealth);        }        if (currentHealth <= 0f)        {            PartialDeath();        }    }

    void PartialDeath()    {        lifeDots.DarkenDot();        currentLives = currentLives - 1;        if (currentLives <= 0)        {            CompleteDeath();        } else        {            SceneManager.LoadScene(1);            currentHealth = maxHealth;            Debug.Log("Player health " + currentHealth + " Player lives " + currentLives);        }    }

    void CompleteDeath()    {        Manager.ResetGame();        SceneManager.LoadScene(1);        currentHealth = maxHealth;        currentLives = maxLives;    }


    void Fire()    {        GameObject bulletInstance = Instantiate(Bullet, firePoint.position, firePoint.rotation);        bulletInstance.transform.localScale = transform.localScale * 0.19f;        bulletInstance.GetComponent<Rigidbody>().velocity = firePoint.forward * bulletSpeed;    }

    void UseHealthPotion()    {        if (Manager.healthPotionNo > 0)        {            currentHealth += Manager.addHealthAmount;            if (currentHealth > maxHealth)            {                currentHealth = maxHealth;            }            healthBar.SetHealth(currentHealth);            Manager.healthPotionNo -= 1;            collectibles.setHealthPotionText(Manager.healthPotionNo);        }    }

    void UseShield()    {        if(Manager.shieldNo > 0)        {            Manager.shieldNo -= 1;            collectibles.setShieldText(Manager.shieldNo);            isInvincible = true;            shieldTimer = Manager.shiedDuration;            shieldTimerUI.SetActive(true);        }    }
}
