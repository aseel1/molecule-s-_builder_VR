using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeBond : MonoBehaviour
{
    public float bondDistance = 2f;  // Distance within which molecules will bond
    public GameObject bondPrefab;    // Prefab for the bond (e.g., a cylinder)

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
        // Check if the molecules are already bonded
        if (molecule1.GetComponent<Bonded>() && molecule2.GetComponent<Bonded>())
            return;

        // Create a bond (e.g., a cylinder) between the two molecules
        Vector3 bondPosition = (molecule1.transform.position + molecule2.transform.position) / 2;
        GameObject bond = Instantiate(bondPrefab, bondPosition, Quaternion.identity);

        // Adjust the bond's rotation and scale
        bond.transform.LookAt(molecule2.transform);
        bond.transform.localScale = new Vector3(bond.transform.localScale.x, bond.transform.localScale.y, Vector3.Distance(molecule1.transform.position, molecule2.transform.position));

        // Handle group assignment and merging
        MoleculeGroup group1 = molecule1.GetComponentInParent<MoleculeGroup>();
        MoleculeGroup group2 = molecule2.GetComponentInParent<MoleculeGroup>();

        if (group1 != null && group2 != null)
        {
            if (group1 != group2)
            {
                group1.MergeGroup(group2);
            }
        }
        else if (group1 != null)
        {
            group1.AddMolecule(molecule2);
        }
        else if (group2 != null)
        {
            group2.AddMolecule(molecule1);
        }
        else
        {
            GameObject groupObject = new GameObject("MoleculeGroup");
            MoleculeGroup newGroup = groupObject.AddComponent<MoleculeGroup>();
            newGroup.AddMolecule(molecule1);

            newGroup.AddMolecule(molecule2);
        }

        // Mark the molecules as bonded
        molecule1.AddComponent<Bonded>();
        molecule2.AddComponent<Bonded>();
    }
}

// Helper script to mark molecules as bonded
public class Bonded : MonoBehaviour { }
