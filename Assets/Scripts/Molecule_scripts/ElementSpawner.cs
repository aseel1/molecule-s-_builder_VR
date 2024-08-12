using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Import the new Input System package
using UnityEngine.EventSystems;  // Import this namespace

public class ElementSpawner : MonoBehaviour
{
    public GameObject hydrogenPrefab;
    public GameObject oxygenPrefab;
    // Add references for other element prefabs as needed

    private GameObject selectedElement;
    private InputAction clickAction;

    private void Awake()
    {
        // Initialize the InputAction for detecting mouse clicks
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();
    }

    public void SelectHydrogen()
    {
        selectedElement = hydrogenPrefab;
    }

    public void SelectOxygen()
    {
        selectedElement = oxygenPrefab;
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
                }
            }
        }
    }


    private void OnDestroy()
    {
        // Disable the InputAction when the object is destroyed
        clickAction.Disable();
    }
}