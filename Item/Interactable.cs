using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    private Transform player;

    private void Awake()
    {
        player = CharacterManager.instance.player;
    }


    private void Update()
    {

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < radius)
        {   
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    public virtual void Interact()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}