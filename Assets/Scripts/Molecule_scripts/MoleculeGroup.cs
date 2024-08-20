using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleculeGroup : MonoBehaviour
{
    public List<GameObject> molecules = new List<GameObject>();  // List of molecules in the group
    public List<GameObject> bonds = new List<GameObject>();      // List of bonds in the group

    // A dictionary to track the count of each type of element (e.g., H, O)
    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();



    // Add a molecule to the group
    public void AddMolecule(GameObject molecule)
    {
        if (!molecules.Contains(molecule))
        {
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
            Debug.Log("Element counts: " + string.Join(", ", elementCounts.Select(kv => kv.Key + ": " + kv.Value)));
            NotifyMoleculeGroupChanged();
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
