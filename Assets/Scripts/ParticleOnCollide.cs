using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOnCollide : MonoBehaviour
{
    public GameObject particle;

    void OnCollisionEnter2D(Collision2D col)
    {
        var go = Instantiate(particle, col.GetContact(0).point, Quaternion.identity);
        Destroy(go, 5f);
    }
}
