using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumBulletScript : MonoBehaviour
{
    private int EntityCount = 1;
    public float Dmg = 50;

    public GameObject Pew;


    protected int layermask;

    private void Awake()
    {
        layermask = (1 << 10);
        layermask |= (1 << 5);
        layermask |= (1 << 8);

        layermask = ~layermask;
    }

    private void Start()
    {
        Invoke(nameof(Death), 7.5f);
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, transform.forward, out hit, Time.deltaTime * 50, layermask, QueryTriggerInteraction.Ignore))
        {
            this.transform.position = hit.point;
            OnHitEnter(hit.collider);
        }
        else
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime * 50);
        }
    }

    private void OnHitEnter(Collider other)
    {
        //insert Damage Function here
        if (other.tag == "Enemy" && EntityCount != 0)
        {
            EntityCount--;
            //other.gameObject.GetComponent<Enemy>().Health.Value -= Dmg;
        }
        else
        {
            Pew = Instantiate(Pew, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Pew.transform.forward = this.transform.forward * (-1);
            Death();
        }
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }

}
