using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(collider.GetComponent<PlayerController>() != null)
            {
                collider.GetComponent<PlayerController>().currentHP -= 50;
            } else 
            {
                collider.GetComponent<Player2Controller>().currentHP -= 50;
            }
        }
    }
}
