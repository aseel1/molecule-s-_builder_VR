using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeGroup : MonoBehaviour
{
    public List<GameObject> molecules = new List<GameObject>();  // List of molecules in the group
    public List<GameObject> bonds = new List<GameObject>();      // List of bonds in the group

    // Add a molecule to the group
    public void AddMolecule(GameObject molecule)
    {
        if (!molecules.Contains(molecule))
        {
            molecules.Add(molecule);
            molecule.transform.SetParent(transform);  // Set the parent to the group for easy movement
        }
    }

    // Add a bond to the group
    public void AddBond(GameObject bond)
    {
        if (!bonds.Contains(bond))
        {
            bonds.Add(bond);
            bond.transform.SetParent(transform);  // Set the parent to the group for easy movement
        }
    }

    // Remove a molecule and its associated bonds
    public void RemoveMolecule(GameObject molecule)
    {
        if (molecules.Contains(molecule))
        {
            molecules.Remove(molecule);
            Destroy(molecule);

            // Optionally remove associated bonds
            RemoveAssociatedBonds(molecule);
        }
    }

    // Remove associated bonds when a molecule is removed
    private void RemoveAssociatedBonds(GameObject molecule)
    {
        for (int i = bonds.Count - 1; i >= 0; i--)
        {

            if (bonds[i].GetComponent<Bond>().IsConnectedTo(molecule))
            {
            Debug.Log("they are");
                Destroy(bonds[i]);
                bonds.RemoveAt(i);
            }
        }
    }

    // Destroy the entire group
    public void DestroyGroup()
    {
        foreach (var molecule in molecules)
        {
            Destroy(molecule);
        }

        foreach (var bond in bonds)
        {
            Destroy(bond);
        }

        Destroy(gameObject);
    }
}
