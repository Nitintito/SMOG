using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Linus Edlund

//Ska ligga under ett particle system som skjuter skotten
//Particle systemet behöver ha collision på med message påslagen och min killspeed 1 över den hastighet den åker.
//du behöver particle system liggandes under din huvud particle system som inte skjuter ut några particles. Den ska ligga som hiteffect
//bulletshoot är själva huvudsystemet. Alltså den detta scripten ska sitta på.
//när hdrp är på lägg då in decalen på det som ska vara det visuella hålet på hitEffect2
public class Bullets : MonoBehaviour
{
    float damage = 5;
    public ParticleSystem bulletShoot;
    public ParticleSystem hitEffect;
    public GameObject hitEffect2;

    List<ParticleCollisionEvent> collisionEvents;
    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hi");
        ParticlePhysicsExtensions.GetCollisionEvents(bulletShoot, other, collisionEvents);
        for (int i = 0; i < collisionEvents.Count; i++)
        {
            impactEffect(collisionEvents[i]);
            impactEffect2(collisionEvents[i]);
        }
        Target target = other.transform.GetComponent<Target>();      //Skadar Target
        

        if (target != null)
        {
            target.TakeDamage(damage);
        }
        
        //Instantiate(hitEffect, other.transform.position, other.transform.rotation);

        //if (other.GetComponent<Rigidbody>() != null)
        //{
        //    other.GetComponent<Rigidbody>().AddForce(-other.normal * impactForce);
        //}
    }
    void impactEffect(ParticleCollisionEvent particleCollisionEvent)
    {
        hitEffect.transform.position = particleCollisionEvent.intersection;
        hitEffect.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
        hitEffect.Emit(10);
    }
    void impactEffect2(ParticleCollisionEvent particleCollisionEvent)
    {
        
        Instantiate(hitEffect2, hitEffect.transform.position, hitEffect.transform.rotation);
        
    }
}
