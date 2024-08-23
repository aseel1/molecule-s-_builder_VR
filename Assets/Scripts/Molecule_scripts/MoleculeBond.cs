using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeBond : MonoBehaviour
{
    public float bondDistance = 2f;  // Distance within which molecules will bond
    public GameObject bondPrefab;    // Prefab for the bond (e.g., a cylinder)
    public GameObject moleculeGroupPrefab; // Prefab for the MoleculeGroup

    void Update()
    {
        // Find all molecules (spheres) in the scene
        List<GameObject> molecules = new List<GameObject>(GameObject.FindGameObjectsWithTag("Molecule"));

        for (int i = 0; i < molecules.Count; i++)
        {
            for (int j = i + 1; j < molecules.Count; j++)
            {
                if (Vector3.Distance(molecules[i].transform.position, molecules[j].transform.position) <= bondDistance)
                {
                    CreateBond(molecules[i], molecules[j]);
                }
            }
        }
    }

    void CreateBond(GameObject molecule1, GameObject molecule2)
    {
        // Check if they are already bonded
        if (molecule1.GetComponent<Bonded>() && molecule2.GetComponent<Bonded>())
            return;

        // Create a bond (e.g., a cylinder) between the two molecules
        Vector3 bondPosition = (molecule1.transform.position + molecule2.transform.position) / 2;
        GameObject bond = Instantiate(bondPrefab, bondPosition, Quaternion.identity);

        bond.AddComponent<Bond>();
        bond.GetComponent<Bond>().molecule1 = molecule1;
        bond.GetComponent<Bond>().molecule2 = molecule2;

        // Adjust the bond's rotation and scale
        bond.transform.LookAt(molecule2.transform);
        bond.transform.localScale = new Vector3(bond.transform.localScale.x, bond.transform.localScale.y, Vector3.Distance(molecule1.transform.position, molecule2.transform.position));

        // Find or create a MoleculeGroup to manage the molecules and bonds
        MoleculeGroup group = FindOrCreateGroup(molecule1, molecule2);




        // Add molecules and bond to the group
        group.AddMolecule(molecule1);
        group.AddMolecule(molecule2);
        group.AddBond(bond);

        // Mark the molecules as bonded
        molecule1.AddComponent<Bonded>();
        molecule2.AddComponent<Bonded>();
    }

    MoleculeGroup FindOrCreateGroup(GameObject molecule1, GameObject molecule2)
    {
        MoleculeGroup group1 = molecule1.GetComponentInParent<MoleculeGroup>();
        MoleculeGroup group2 = molecule2.GetComponentInParent<MoleculeGroup>();

        if (group1 != null && group2 != null)
        {
            // Merge groups if they are different
            if (group1 != group2)
            {
                MergeGroups(group1, group2);
                return group1;
            }
            return group1;
        }
        else if (group1 != null)
        {
            return group1;
        }
        else if (group2 != null)
        {
            return group2;
        }
        else
        {
            // Create a new group if none exists
            GameObject newGroup = Instantiate(moleculeGroupPrefab);
            MoleculeGroup moleculeGroup = newGroup.GetComponent<MoleculeGroup>();
            return moleculeGroup;
        }
    }

    void MergeGroups(MoleculeGroup group1, MoleculeGroup group2)
    {
        // Transfer all molecules and bonds from group2 to group1
        foreach (GameObject molecule in group2.molecules)
        {
            group1.AddMolecule(molecule);
        }
        foreach (GameObject bond in group2.bonds)
        {
            group1.AddBond(bond);
        }
        // Destroy the now empty group2
        Destroy(group2.gameObject);
    }

    // Helper script to mark molecules as bonded
    public class Bonded : MonoBehaviour { }
}
