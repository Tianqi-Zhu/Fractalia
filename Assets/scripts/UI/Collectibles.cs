using UnityEngine;
using UnityEngine.UI;


public class Collectibles : MonoBehaviour
{    public Text HealthPotionText;    public Text ShieldText;    private PersistentManager Manager;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        HealthPotionText.text = "X" + Manager.healthPotionNo;
        ShieldText.text = "X" + Manager.shieldNo;
    }

    public void setHealthPotionText(int healthPotionNo)    {        HealthPotionText.text = "X" + healthPotionNo;    }

    public void setShieldText(int ShieldNo)    {        ShieldText.text = "X" + ShieldNo;    }
}
