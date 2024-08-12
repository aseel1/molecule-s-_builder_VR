using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bond : MonoBehaviour
{
    public GameObject molecule1;
    public GameObject molecule2;

    // Check if this bond is connected to a specific molecule
    public bool IsConnectedTo(GameObject molecule)
    {
        return molecule == molecule1 || molecule == molecule2;
    }
}
