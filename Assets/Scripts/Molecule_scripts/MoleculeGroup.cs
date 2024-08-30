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
    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();

    private void Start()
    {
        Debug.Log("MoleculeGroup Start");
        Canvas specificCanvas = GameObject.Find("MoleculeTable(canvas)").GetComponent<Canvas>();
        if (formulaPanelPrefab != null && specificCanvas != null)
        {
            formulaPanelInstance = Instantiate(formulaPanelPrefab, specificCanvas.transform);
            formulaText = formulaPanelInstance.transform.Find("Fourmla_text").GetComponent<TextMeshProUGUI>();
            MoleculeDisplay display = GetComponent<MoleculeDisplay>();
            if (display != null)
            {
                display.InitializeDisplay(formulaText);
            }
            PositionFormulaPanel();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void AddMoleculeServerRpc(NetworkObjectReference moleculeReference)
    {
        Debug.Log("Server: Adding molecule via ServerRpc");

        if (moleculeReference.TryGet(out NetworkObject moleculeObject))
        {
            GameObject molecule = moleculeObject.gameObject;
            AddMoleculeToList(molecule);

            // Notify all clients about the added molecule
            AddMoleculeClientRpc(moleculeReference);
        }
    }

    [ClientRpc]
    private void AddMoleculeClientRpc(NetworkObjectReference moleculeReference)
    {
        if (moleculeReference.TryGet(out NetworkObject moleculeObject))
        {
            GameObject molecule = moleculeObject.gameObject;
            AddMoleculeToList(molecule);
        }
    }

    private void AddMoleculeToList(GameObject molecule)
    {
        Debug.Log($"Adding molecule: {molecule.name}");
        if (!molecules.Contains(molecule))
        {
            molecules.Add(molecule);
            molecule.transform.SetParent(transform);

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

    public void PositionFormulaPanel()
    {
        if (!isFormulaPanelPositioned && formulaPanelInstance != null)
        {
            Vector3 moleculePosition = molecules[1].transform.position;
            Vector3 offset = new Vector3(3, 0, 0);
            formulaPanelInstance.transform.position = moleculePosition + offset;
            isFormulaPanelPositioned = true;
            Debug.Log("Formula panel positioned");
        }
    }

 
    [ServerRpc(RequireOwnership = false)]
    public void AddBondServerRpc(NetworkObjectReference bondReference)
    {
        Debug.Log("Server: Adding bond via ServerRpc");

        if (bondReference.TryGet(out NetworkObject bondObject))
        {
            GameObject bond = bondObject.gameObject;
            AddBondToList(bond);

            // Notify all clients about the added bond
            AddBondClientRpc(bondReference);
        }
    }

    [ClientRpc]
    private void AddBondClientRpc(NetworkObjectReference bondReference)
    {
        if (bondReference.TryGet(out NetworkObject bondObject))
        {
            GameObject bond = bondObject.gameObject;
            AddBondToList(bond);
        }
    }

    private void AddBondToList(GameObject bond)
    {
        Debug.Log($"Adding bond: {bond.name}");
        if (!bonds.Contains(bond))
        {
            bonds.Add(bond);
            bond.transform.SetParent(transform);
        }
    }

    public void RemoveMolecule(GameObject molecule)
    {
        Debug.Log($"Removing molecule: {molecule.name}");
        if (molecules.Contains(molecule))
        {
            molecules.Remove(molecule);

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
            RemoveAssociatedBonds(molecule);
            NotifyMoleculeGroupChanged();
        }
    }

    private void RemoveAssociatedBonds(GameObject molecule)
    {
        for (int i = bonds.Count - 1; i >= 0; i--)
        {
            if (bonds[i].GetComponent<Bond>().IsConnectedTo(molecule))
            {
                Debug.Log($"Removing bond: {bonds[i].name}");
                Destroy(bonds[i]);
                bonds.RemoveAt(i);
            }
        }
    }

    public void DestroyGroup()
    {
        Debug.Log("Destroying group");
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

    public string GetMolecularFormula()
    {
        List<string> elementOrder = new List<string> { "C", "H", "O", "N", "S", "P" };

        var sortedElements = elementCounts.OrderBy(kv =>
        {
            int index = elementOrder.IndexOf(kv.Key);
            return index == -1 ? int.MaxValue : index;
        }).ThenBy(kv => kv.Key);

        string formula = "";
        foreach (var element in sortedElements)
        {
            formula += element.Key;
            if (element.Value > 1)
            {
                formula += $"<sub>{element.Value}</sub>";
            }
        }
        return formula;
    }
}