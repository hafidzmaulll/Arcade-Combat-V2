using UnityEngine;
using System.Collections;

public class ShowHideObject : MonoBehaviour
{

    public GameObject hidey;
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
            hidey.gameObject.SetActive(false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
            hidey.gameObject.SetActive(true);
    }

}