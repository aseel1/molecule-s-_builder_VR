using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoleculeBond : NetworkBehaviour
{
    public float bondDistance = 2f;  // Distance within which molecules will bond
    public GameObject bondPrefab;    // Prefab for the bond (e.g., a cylinder)
    public GameObject moleculeGroupPrefab; // Prefab for the MoleculeGroup

    void Update()
    {
        if (!IsServer) return; // Ensure bond creation only runs on the server

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

        // Call the Server RPC to create the bond
        //! here is okay to call the NetworkObjectId since the molecule is spawned in the elementspawner.
        CreateBondServerRpc(molecule1.GetComponent<NetworkObject>().NetworkObjectId, molecule2.GetComponent<NetworkObject>().NetworkObjectId);
    }
    [ServerRpc(RequireOwnership = false)]
    void CreateBondServerRpc(ulong molecule1Id, ulong molecule2Id)
    {
        Debug.Log($"CreateBondServerRpc called with molecule1Id: {molecule1Id}, molecule2Id: {molecule2Id}");

        NetworkObject molecule1 = null;
        NetworkObject molecule2 = null;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(molecule1Id, out molecule1))
        {
            Debug.LogError($"Failed to resolve NetworkObjectId for molecule1: {molecule1Id}");
        }

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(molecule2Id, out molecule2))
        {
            Debug.LogError($"Failed to resolve NetworkObjectId for molecule2: {molecule2Id}");
        }

        if (molecule1 == null)
        {
            Debug.LogError("molecule1 is null");
        }

        if (molecule2 == null)
        {
            Debug.LogError("molecule2 is null");
        }

        if (molecule1 != null && molecule2 != null)
        {
            Debug.Log("Both molecules are resolved successfully");

            // Create a bond (e.g., a cylinder) between the two molecules
            Vector3 bondPosition = (molecule1.transform.position + molecule2.transform.position) / 2;
            GameObject bond = Instantiate(bondPrefab, bondPosition, Quaternion.identity);


            // Ensure the bond is spawned on the network
            NetworkObject bondNetworkObject = bond.GetComponent<NetworkObject>();
            if (bondNetworkObject == null)
            {
                Debug.LogError("bondPrefab does not have a NetworkObject component");
                return;
            }

            bondNetworkObject.Spawn();
            Debug.Log("Bond spawned successfully");

            Bond bondComponent = bond.AddComponent<Bond>();
            if (bondComponent == null)
            {
                Debug.LogError("Failed to add Bond component to bond");
                return;
            }

            bondComponent.molecule1 = molecule1.gameObject;
            bondComponent.molecule2 = molecule2.gameObject;

            // Adjust the bond's rotation and scale
            bond.transform.LookAt(molecule2.transform);
            bond.transform.localScale = new Vector3(bond.transform.localScale.x, bond.transform.localScale.y, Vector3.Distance(molecule1.transform.position, molecule2.transform.position));

            // Find or create a MoleculeGroup to manage the molecules and bonds
            MoleculeGroup group = FindOrCreateGroup(molecule1.gameObject, molecule2.gameObject);

            if (group == null)
            {
                // Create a new group synchronously
                GameObject newGroup = Instantiate(moleculeGroupPrefab);
                MoleculeGroup moleculeGroup = newGroup.GetComponent<MoleculeGroup>();

                // Ensure the group is spawned on the network
                NetworkObject groupNetworkObject = newGroup.GetComponent<NetworkObject>();
                groupNetworkObject.Spawn();
                Debug.Log("New MoleculeGroup spawned successfully");

                // Add the molecules to the new group using ServerRpc
                moleculeGroup.AddMoleculeServerRpc(molecule1.GetComponent<NetworkObject>());
                moleculeGroup.AddMoleculeServerRpc(molecule2.GetComponent<NetworkObject>());

                group = moleculeGroup;
            }



            // Add molecules and bond to the group using ServerRpc
            group.AddMoleculeServerRpc(molecule1.GetComponent<NetworkObject>());
            group.AddMoleculeServerRpc(molecule2.GetComponent<NetworkObject>());
            group.AddBondServerRpc(new NetworkObjectReference(bondNetworkObject));
            Debug.Log("Molecules and bond added to the group");

            // Mark the molecules as bonded
            molecule1.gameObject.AddComponent<Bonded>();
            molecule2.gameObject.AddComponent<Bonded>();
            Debug.Log("Molecules marked as bonded");
        }
        else
        {
            Debug.LogError("Failed to resolve NetworkObjectId for one or both molecules.");
        }
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
            // Call the Server RPC to create a new group
            CreateGroupServerRpc(molecule1.GetComponent<NetworkObject>().NetworkObjectId, molecule2.GetComponent<NetworkObject>().NetworkObjectId);
            return null; // Return null for now, the group will be created asynchronously
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateGroupServerRpc(ulong molecule1Id, ulong molecule2Id)
    {
        // Create a new group if none exists
        GameObject newGroup = Instantiate(moleculeGroupPrefab);
        MoleculeGroup moleculeGroup = newGroup.GetComponent<MoleculeGroup>();

        // Ensure the group is spawned on the network
        NetworkObject groupNetworkObject = newGroup.GetComponent<NetworkObject>();
        groupNetworkObject.Spawn();

        // Add the molecules to the new group using ServerRpc
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(molecule1Id, out NetworkObject molecule1) &&
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(molecule2Id, out NetworkObject molecule2))
        {
            moleculeGroup.AddMoleculeServerRpc(molecule1);
            moleculeGroup.AddMoleculeServerRpc(molecule2);
        }
    }

    void MergeGroups(MoleculeGroup group1, MoleculeGroup group2)
    {
        // Transfer all molecules and bonds from group2 to group1
        foreach (GameObject molecule in group2.molecules)
        {
            NetworkObject moleculeNetworkObject = molecule.GetComponent<NetworkObject>();
            if (moleculeNetworkObject != null)
            {
                group1.AddMoleculeServerRpc(moleculeNetworkObject);
            }
        }

        foreach (GameObject bond in group2.bonds)
        {
            NetworkObject bondNetworkObject = bond.GetComponent<NetworkObject>();
            if (bondNetworkObject != null)
            {
                // Add the bond to group1 via ServerRpc
                group1.AddBondServerRpc(new NetworkObjectReference(bondNetworkObject));
            }
        }

        // Destroy the now empty group2
        Destroy(group2.gameObject);
    }

    // Helper script to mark molecules as bonded
    public class Bonded : MonoBehaviour { }
}