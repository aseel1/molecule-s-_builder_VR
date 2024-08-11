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
            return;  // Exit the method to prevent spawning an element
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Set the distance from the camera where the object will be spawned
        float distance = 2f;  // Adjust this distance as needed

        // Calculate the spawn position
        Vector3 spawnPosition = ray.GetPoint(distance);

        // Place the selected element at the calculated position
        SpawnElement(spawnPosition);
    }


    private void OnDestroy()
    {
        // Disable the InputAction when the object is destroyed
        clickAction.Disable();
    }
}