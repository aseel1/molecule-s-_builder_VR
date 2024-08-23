using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Import the new Input System package
using UnityEngine.EventSystems;  // Import this namespace

public class ElementSpawner : MonoBehaviour
{
    public GameObject hydrogenPrefab;
    public GameObject oxygenPrefab;
    public GameObject carbonPrefab;
    private GameObject selectedElement;
    private InputAction clickAction;
    private InputAction rightClickAction;
    private GameObject draggedMolecule;
    private MoleculeGroup draggedGroup;
    private GameObject moleculeToMove;
    private List<GameObject> connectedBonds = new List<GameObject>();

    private void Awake()
    {
        // Init left click
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();

        // Init right click
        rightClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        rightClickAction.performed += ctx => OnRightClick();
        rightClickAction.canceled += ctx => OnRightClickRelease();
        rightClickAction.Enable();
    }

    public void SelectHydrogen()
    {
        selectedElement = hydrogenPrefab;
    }

    public void SelectOxygen()
    {
        selectedElement = oxygenPrefab;
    }


    public void SelectCarbon()
    {
        selectedElement = carbonPrefab;
    }

    // Call this method when clicking on the board to place the selected element
    public void SpawnElement(Vector3 position)
    {
        if (selectedElement != null)
        {
            Instantiate(selectedElement, position, Quaternion.identity);
        }
    }

    private void OnClick()
    {
        // Check if the click is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  // Exit the method to prevent spawning a sphere
        }
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            DeleteMolecule(ray);
        }
        else
        {
            // Regular spawn
            float distance = 2f;
            Vector3 spawnPosition = ray.GetPoint(distance);
            SpawnElement(spawnPosition);
        }
    }

    private void OnRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.CompareTag("Molecule"))
            {
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    // Move entire group
                    draggedMolecule = clickedObject;
                    draggedGroup = draggedMolecule.GetComponentInParent<MoleculeGroup>();
                }
                else if (Keyboard.current.leftCtrlKey.isPressed)
                {
                    // Move single molecule and its connected bonds
                    moleculeToMove = clickedObject;
                    MoleculeGroup group = moleculeToMove.GetComponentInParent<MoleculeGroup>();
                    if (group != null)
                    {
                        connectedBonds.Clear();
                        foreach (GameObject bond in group.bonds)
                        {
                            if (IsBondConnectedToMolecule(bond, moleculeToMove))
                            {
                                connectedBonds.Add(bond);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnRightClickRelease()
    {
        draggedMolecule = null;
        draggedGroup = null;
        moleculeToMove = null;
        connectedBonds.Clear();
    }

    private void Update()
    {
        if (draggedMolecule != null && draggedGroup != null)
        {
            // Move the entire group
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 targetPosition = ray.GetPoint(2f);  // Adjust distance as needed
            Vector3 delta = targetPosition - draggedMolecule.transform.position;

            draggedGroup.transform.position += delta;  // Move the entire group
        }
        else if (moleculeToMove != null)
        {
            // Move the single molecule and its connected bonds
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 targetPosition = ray.GetPoint(2f);  // Adjust distance as needed
            Vector3 delta = targetPosition - moleculeToMove.transform.position;

            moleculeToMove.transform.position += delta;  // Move the molecule
            foreach (GameObject bond in connectedBonds)
            {
                UpdateBondPosition(bond);
            }
        }
    }

    private bool IsBondConnectedToMolecule(GameObject bond, GameObject molecule)
    {
        // Check if the bond is connected to the molecule
        return bond.GetComponent<Bond>().IsConnectedTo(molecule);
    }

    private void UpdateBondPosition(GameObject bond)
    {
        Bond bondComponent = bond.GetComponent<Bond>();
        if (bondComponent != null)
        {
            Vector3 startPosition = bondComponent.molecule1.transform.position;
            Vector3 endPosition = bondComponent.molecule2.transform.position;
            bond.transform.position = (startPosition + endPosition) / 2;
            bond.transform.LookAt(endPosition);
            bond.transform.localScale = new Vector3(bond.transform.localScale.x, bond.transform.localScale.y, Vector3.Distance(startPosition, endPosition));
        }
    }

    private void DeleteMolecule(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            // Check if the clicked object is a molecule
            if (clickedObject.CompareTag("Molecule"))
            {
                // Find the MoleculeGroup this molecule belongs to
                MoleculeGroup group = clickedObject.GetComponentInParent<MoleculeGroup>();
                if (group != null)
                {
                    group.RemoveMolecule(clickedObject);
                    // Check if the group is empty after removal and destroy it if necessary
                    if (group.molecules.Count == 0 && group.bonds.Count == 0)
                    {
                        Destroy(group.gameObject);
                    }
                }
                else
                {
                    // If no group, simply destroy the molecule
                    Destroy(clickedObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        clickAction.Disable();
        rightClickAction.Disable();
    }
}