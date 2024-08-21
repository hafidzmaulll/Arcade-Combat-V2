using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollider : MonoBehaviour
{
    public GameObject door;


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            door.GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }
}
