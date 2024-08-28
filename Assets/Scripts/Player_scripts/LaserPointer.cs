using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class LaserPointer : NetworkBehaviour
{
    public Transform handTransform;  // Reference to the hand (cube) transform
    public float laserDistance = 100f;  // Maximum distance for the laser

    private LineRenderer lineRenderer;
    private InputAction clickAction;

    void Start()
    {
        // Initialize the Line Renderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component is missing.");
            return;
        }

        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;

        // Initialize the InputAction for detecting mouse clicks
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();
    }

    void Update()
    {
        if (handTransform == null)
        {
            Debug.LogError("HandTransform is not assigned.");
            return;
        }

        // Cast a ray from the hand's position in the forward direction
        Ray ray = new Ray(handTransform.position, handTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            // Set the LineRenderer positions
            lineRenderer.SetPosition(0, handTransform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // If no hit, extend the line to max distance
            lineRenderer.SetPosition(0, handTransform.position);
            lineRenderer.SetPosition(1, handTransform.position + handTransform.forward * laserDistance);
        }
    }

    private void OnClick()
    {
        // Check if the click is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  // Exit the method to prevent any action
        }

        // Ensure lineRenderer is not null
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            return;
        }

        // Optional: Change the color of the laser on click
        lineRenderer.material.color = Color.green;
    }

    private void OnDestroy()
    {
        clickAction.Disable();
    }
}