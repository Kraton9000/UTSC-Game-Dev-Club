using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    Vector3 localScale;
    private float xScale = 0.07f;
    private float yScale = 0.07f;
    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        localScale.x = xScale;
        localScale.y = yScale;
        transform.localScale = localScale;
    }

    public void updateRope(Vector2 start, Vector2 end)
    {
        Vector2 newVector = end - start;
        if (newVector.x == 0)
        {
            this.xScale = 0.07f;
            this.yScale = newVector.y;
        }
        else if (newVector.y == 0)
        {
            this.xScale = newVector.x;
            this.yScale = 0.07f;
        }
    }
}
