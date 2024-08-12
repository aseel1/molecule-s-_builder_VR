using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeGroup : MonoBehaviour
{
    public List<GameObject> molecules = new List<GameObject>();

    public void AddMolecule(GameObject molecule)
    {
        molecules.Add(molecule);
        molecule.transform.parent = transform;
    }

    public void MergeGroup(MoleculeGroup otherGroup)
    {
        foreach (GameObject molecule in otherGroup.molecules)
        {
            AddMolecule(molecule);
        }
        Destroy(otherGroup.gameObject);
    }

    public void MoveGroup(Vector3 position)
    {
        transform.position = position;
    }
}
