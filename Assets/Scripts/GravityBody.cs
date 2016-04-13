using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{

    GravityAttractor planet;
    Rigidbody rgb;

    public bool dontAffectForce;
    public bool customForce;

    public float customGravity;

    void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        rgb = GetComponent<Rigidbody>();
        rgb.useGravity = false;
        rgb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    void FixedUpdate()
    {
        if (customForce)
            planet.Attract(transform, customGravity, false, false);
        else if(!dontAffectForce)
            planet.Attract(transform);
        else
            planet.Attract(transform,0,true,true);
    }

}
