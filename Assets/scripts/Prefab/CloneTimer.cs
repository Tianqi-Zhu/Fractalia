using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneTimer : MonoBehaviour
{
    public float lifeTime;
    private float timeAlive;

    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifeTime)
        {
            Destroy(gameObject); // destroy after a certain time
        }
    }
}
