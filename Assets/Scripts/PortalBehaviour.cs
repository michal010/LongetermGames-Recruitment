using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    public Transform TeleportTo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AIAgent"))
        {
            Debug.Log("No, wszed³");
            other.gameObject.transform.position = TeleportTo.position;
        }
    }
}
