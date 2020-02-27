using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT IN USE ANYMORE
public class HealthBar : MonoBehaviour
{
    Vector3 localScale;
    private float hpPercentage = 1f;
    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        localScale.x = hpPercentage;
        transform.localScale = localScale;
    }

    public void updateHPBar(float hpPercentage)
    {
        this.hpPercentage = hpPercentage;
    }
}
