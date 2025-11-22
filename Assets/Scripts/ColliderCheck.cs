using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    float contact;

    private HashSet<Collider> contacts = new HashSet<Collider>();

    public bool CheckTrigger()
    {
        return (contacts.Count > 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            contacts.Add(other);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            contacts.Remove(other);
        }
    }
}
