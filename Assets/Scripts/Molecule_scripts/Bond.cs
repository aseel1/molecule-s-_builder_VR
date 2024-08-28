using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bond : NetworkBehaviour
{
    public GameObject molecule1;
    public GameObject molecule2;

    // Check if this bond is connected to a specific molecule
    public bool IsConnectedTo(GameObject molecule)
    {
        return molecule == molecule1 || molecule == molecule2;
    }
}
