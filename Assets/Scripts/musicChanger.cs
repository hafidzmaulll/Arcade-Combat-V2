using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicChanger : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance.Stop("Musik Main");
        AudioManager.instance.Play("Musik Boss");
    }

    void Update()
    {
        
    }
}
