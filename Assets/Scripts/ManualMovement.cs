using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMovement : MonoBehaviour
{
    private Rigidbody2D r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        r.velocity += new Vector2(Input.GetAxis("Horizontal") * .3f, Input.GetAxis("Vertical") * .3f);
    }
}
