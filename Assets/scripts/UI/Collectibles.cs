using UnityEngine;
using UnityEngine.UI;


public class Collectibles : MonoBehaviour
{

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        HealthPotionText.text = "X" + Manager.healthPotionNo;
        ShieldText.text = "X" + Manager.shieldNo;
    }

    public void setHealthPotionText(int healthPotionNo)

    public void setShieldText(int ShieldNo)
}