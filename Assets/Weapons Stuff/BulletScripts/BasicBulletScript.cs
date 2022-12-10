using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletScript : MonoBehaviour
{
    public GameObject Pew;
    public float Dmg = 25;

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

        if (Physics.Raycast(this.transform.position, transform.forward, out hit, Time.deltaTime * 50 + 1, layermask, QueryTriggerInteraction.Ignore))
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
        if (other.tag == "Enemy")
        {
            //other.gameObject.GetComponent<Enemy>().Health.Value -= Dmg;
        }
        //insert damage functionality
        Pew = Instantiate(Pew, this.gameObject.transform.position, this.gameObject.transform.rotation);
        Pew.transform.forward = this.transform.forward * (-1);
        Death();
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }

}
