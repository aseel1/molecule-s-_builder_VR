using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MoleculeGroup : NetworkBehaviour
{
    public List<GameObject> molecules = new List<GameObject>();  // List of molecules in the group
    public List<GameObject> bonds = new List<GameObject>();      // List of bonds in the group

    public GameObject formulaPanelPrefab;
    private GameObject formulaPanelInstance;
    private bool isFormulaPanelPositioned = false; // Flag to check if the formula panel is positioned

    private TextMeshProUGUI formulaText;
    // A dictionary to track the count of each type of element (e.g., H, O)
    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();


    private void Start()
    {
        // Find the specific canvas
        Canvas specificCanvas = GameObject.Find("MoleculeTable(canvas)").GetComponent<Canvas>();
        if (formulaPanelPrefab != null && specificCanvas != null)
        {
            // Instantiate the formula panel under the specific canvas
            formulaPanelInstance = Instantiate(formulaPanelPrefab, specificCanvas.transform);


            formulaText = formulaPanelInstance.transform.Find("Fourmla_text").GetComponent<TextMeshProUGUI>();
            // Initialize the MoleculeDisplay with the formulaText
            MoleculeDisplay display = GetComponent<MoleculeDisplay>();
            if (display != null)
            {
                display.InitializeDisplay(formulaText);
            }

            PositionFormulaPanel();

        }
    }


    // Add a molecule to the group
    public void AddMolecule(GameObject molecule)
    {
        if (!molecules.Contains(molecule))
        {

            //! Ensure the molecule has been spawned on the network
            var networkObject = molecule.GetComponent<NetworkObject>();
            if (networkObject != null && !networkObject.IsSpawned)
            {
                networkObject.Spawn();  // Spawn the network object first
            }

            molecules.Add(molecule);
            molecule.transform.SetParent(transform);  // Set the parent to the group for easy movement

            // Update element count based on the name of the molecule
            string elementType = molecule.name.Replace("(Clone)", "").Trim();
            if (ElementSymbols.Symbols.ContainsKey(elementType))
            {
                elementType = ElementSymbols.Symbols[elementType];
            }

            if (elementCounts.ContainsKey(elementType))
            {
                elementCounts[elementType]++;
            }
            else
            {
                elementCounts[elementType] = 1;
            }
            NotifyMoleculeGroupChanged();


        }
    }

    // Method to position the formula panel
    public void PositionFormulaPanel()
    {

        if (!isFormulaPanelPositioned && formulaPanelInstance != null)
        {

            Vector3 moleculePosition = molecules[1].transform.position; // Use the position of the second molecule
            Vector3 offset = new Vector3(3, 0, 0); // Adjust the offset as needed
            formulaPanelInstance.transform.position = moleculePosition + offset;
            isFormulaPanelPositioned = true; // Set the flag to true after positioning
        }
    }

    // Add a bond to the group
    public void AddBond(GameObject bond)
    {
        if (!bonds.Contains(bond))
        {
            //! Ensure the molecule has been spawned on the network
            var networkObject = bond.GetComponent<NetworkObject>();
            if (networkObject != null && !networkObject.IsSpawned)
            {
                networkObject.Spawn();  // Spawn the network object first
            }

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
            // Update element count based on the name of the molecule
            string elementType = molecule.name.Replace("(Clone)", "").Trim();
            if (ElementSymbols.Symbols.ContainsKey(elementType))
            {
                elementType = ElementSymbols.Symbols[elementType];
            }

            if (elementCounts.ContainsKey(elementType))
            {
                elementCounts[elementType]--;
                if (elementCounts[elementType] <= 0)
                {
                    elementCounts.Remove(elementType);
                }
            }

            Destroy(molecule);

            // Optionally remove associated bonds
            RemoveAssociatedBonds(molecule);
            NotifyMoleculeGroupChanged();

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

    private void NotifyMoleculeGroupChanged()
    {
        MoleculeDisplay display = GetComponent<MoleculeDisplay>();
        if (display != null)
        {
            display.OnMoleculeGroupChanged();
        }
    }
    // Get the molecular formula as a string    // Get the molecular formula as a string
    public string GetMolecularFormula()
    {
        // Define the order of elements
        List<string> elementOrder = new List<string> { "C", "H", "O", "N", "S", "P" }; // Add more elements as needed

        // Sort the elements based on the defined order
        var sortedElements = elementCounts.OrderBy(kv =>
        {
            int index = elementOrder.IndexOf(kv.Key);
            return index == -1 ? int.MaxValue : index; // Elements not in the list will be placed at the end
        }).ThenBy(kv => kv.Key); // Secondary sort by element name

        // Build the formula string
        string formula = "";
        foreach (var element in sortedElements)
        {
            formula += element.Key;
            if (element.Value > 1)
            {
                formula += $"<sub>{element.Value}</sub>"; // Display of the formula text. (with style small number)
            }
        }
        return formula;
    }
}
