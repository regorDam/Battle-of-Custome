using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour
{

    public float speed = 6f;


    private GameObject owner;
    int damage;
    float lifeTime;

    // Use this for initialization


    public void Config(GameObject owner, int damage)
    {
        this.owner = owner;
        this.damage = damage;

        Config(owner, speed, damage, 30);



    }

    public void Config(GameObject owner, float speed, int damage, float lifeTime)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        Destroy(gameObject, lifeTime);
    }

    
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            //NetworkServer.Destroy(gameObject);
        }
    }
    

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 8)
        {
            //new WaitForSeconds(4);
            //NetworkServer.Destroy(gameObject);
            Destroy(gameObject, 4);
            //lifeTime = 4;
        }
    }


}
