using UnityEngine;
using System.Collections;

public class GravityAccordingMesh : MonoBehaviour {

    Vector2 xPrime, yPrime;

    Vector2 contact;

    float pPrevRotation;
    float prevRotation = 0;
    float rotation = 0;

    float b = 0;

    void Start () {
        xPrime = Vector2.zero;
        yPrime = Vector2.zero;
    }
	
	
	void Update () {

        b += .5f;

        //if(rotation - prevRotation >= 1)
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, prevRotation), Quaternion.Euler(0, 0, rotation), b);
        prevRotation = rotation;
        
        //Camera.main.transform.position = transform.position;
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            
            GetComponent<Rigidbody2D>().AddForce(xPrime * 8f, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            GetComponent<Rigidbody2D>().AddForce(xPrime * -8f, ForceMode2D.Force);

        }

        if(Mathf.RoundToInt((Mathf.Atan2(yPrime.y, yPrime.x) * Mathf.Rad2Deg - 90)) - prevRotation >= 5)
        {
            prevRotation = rotation;
            rotation = Mathf.RoundToInt((Mathf.Atan2(yPrime.y, yPrime.x) * Mathf.Rad2Deg - 90));
            b = 0;
        }   
        

        
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(other.transform.tag == "T2D")
        {
            Debug.Log(other.contacts.Length);
            xPrime = Quaternion.Euler(0, 0, -90) * other.contacts[0].normal;
            //yPrime = Quaternion.Euler(0, 0, 180) * other.contacts[other.contacts.Length - 1].normal;
            yPrime = other.contacts[0].normal;

            //xPrime = Quaternion.Euler(0, 0, 5) * xPrime;
            other.transform.GetComponent<AreaEffector2D>().forceAngle = Mathf.Atan2(other.contacts[0].normal.y, other.contacts[0].normal.x) * Mathf.Rad2Deg + 180;
            //Vector2 p = ((Vector2)transform.position - other.contacts[other.contacts.Length - 1].point).normalized;

            contact = other.contacts[0].point;
        }        
    }
    void OnDrawGizmos()
    {
        /*Gizmos.DrawLine(transform.position, yPrime * 2 + (Vector2)transform.position);
        Gizmos.DrawLine(transform.position, xPrime * 2 + (Vector2)transform.position);*/

        Gizmos.DrawSphere(contact,.1f);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        other.transform.GetComponent<AreaEffector2D>().angularDrag = Mathf.Round(Mathf.Lerp(0, 1f, GetComponent<Rigidbody2D>().velocity.magnitude/5f)*100)/100;
        other.transform.GetComponent<AreaEffector2D>().drag = Mathf.Round(Mathf.Lerp(0, 7f, GetComponent<Rigidbody2D>().velocity.magnitude/5f)*100)/100;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.tag == "T2D")
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "T2D")
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
            
        }
    }
}
