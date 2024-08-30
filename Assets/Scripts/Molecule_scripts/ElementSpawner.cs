using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Import the new Input System package
using UnityEngine.EventSystems;
using Unity.Netcode;


public class ElementSpawner : NetworkBehaviour
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
        Debug.Log("ElementSpawner Awake");
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
        Debug.Log("Hydrogen selected");
        selectedElement = hydrogenPrefab;
        Debug.Log($"Selected element: {selectedElement}");

    }

    public void SelectOxygen()
    {
        Debug.Log("Oxygen selected");
        selectedElement = oxygenPrefab;
        Debug.Log($"Selected element: {selectedElement}");

    }

    public void SelectCarbon()
    {
        Debug.Log("Carbon selected");
        selectedElement = carbonPrefab;
        Debug.Log($"Selected element: {selectedElement}");

    }

    public void SpawnElement(Vector3 position)
    {
        if (selectedElement != null)
        {
            Debug.Log($"Spawning element request at position: {position}");
            string prefabName = selectedElement.name;
            SpawnElementServerRpc(prefabName, position);
        }
        else
        {
            Debug.LogError("No element selected to spawn.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnElementServerRpc(string prefabName, Vector3 position)
    {
        Debug.Log("SpawnElementServerRpc called");

        GameObject prefabToSpawn = null;

        switch (prefabName)
        {
            case "Hydrogen":
                prefabToSpawn = hydrogenPrefab;
                break;
            case "Oxygen":
                prefabToSpawn = oxygenPrefab;
                break;
            case "Carbon":
                prefabToSpawn = carbonPrefab;
                break;
            default:
                Debug.LogError("Invalid prefab name.");
                return;
        }

        if (prefabToSpawn != null)
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
            NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
            networkObject.Spawn();  // Ensure it is spawned across the network
            Debug.Log("Element instantiated and spawned on server");
        }
        else
        {
            Debug.LogError("Failed to find the prefab to spawn.");
        }
    }

    private void OnClick()
    {
        Debug.Log("Left click detected");
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click over UI element, ignoring");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        if (Mouse.current == null)
        {
            Debug.LogError("Mouse is not detected.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            DeleteMolecule(ray);
        }
        else
        {
            float distance = 2f;
            Vector3 spawnPosition = ray.GetPoint(distance);
            SpawnElement(spawnPosition);
        }
    }

    private void OnRightClick()
    {
        Debug.Log("Right click detected");
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Right click over UI element, ignoring");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        if (Mouse.current == null)
        {
            Debug.LogError("Mouse is not detected.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, 0.5f);

            foreach (Collider collider in colliders)
            {
                GameObject clickedObject = collider.gameObject;

                if (clickedObject.CompareTag("Molecule"))
                {
                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        draggedMolecule = clickedObject;
                        draggedGroup = draggedMolecule.GetComponentInParent<MoleculeGroup>();
                        Debug.Log("Dragging entire group");
                    }
                    else if (Keyboard.current.leftCtrlKey.isPressed)
                    {
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
                            Debug.Log("Dragging single molecule with connected bonds");
                        }
                    }
                }
            }
        }
    }

    private void OnRightClickRelease()
    {
        Debug.Log("Right click released");
        draggedMolecule = null;
        draggedGroup = null;
        moleculeToMove = null;
        connectedBonds.Clear();
    }

    private void Update()
    {
        if (!IsServer) return; // Ensure bond creation only runs on the server
        if (draggedMolecule != null && draggedGroup != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 targetPosition = ray.GetPoint(2f);
            Vector3 delta = targetPosition - draggedMolecule.transform.position;

            draggedGroup.transform.position += delta;
            Debug.Log("Moving entire group");
        }
        else if (moleculeToMove != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 targetPosition = ray.GetPoint(2f);
            Vector3 delta = targetPosition - moleculeToMove.transform.position;

            moleculeToMove.transform.position += delta;
            foreach (GameObject bond in connectedBonds)
            {
                UpdateBondPosition(bond);
            }
            Debug.Log("Moving single molecule and connected bonds");
        }
    }

    private bool IsBondConnectedToMolecule(GameObject bond, GameObject molecule)
    {
        return bond.GetComponent<Bond>().IsConnectedTo(molecule);
    }

    private void UpdateBondPosition(GameObject bond)
    {
        NetworkObject networkObject = bond.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            UpdateBondPositionServerRpc(networkObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateBondPositionServerRpc(NetworkObjectReference bondRef)
    {
        Debug.Log("UpdateBondPosition ServerRpcs");

        if (bondRef.TryGet(out NetworkObject bond))
        {

            Bond bondComponent = bond.GetComponent<Bond>();
            if (bondComponent != null)
            {
                Vector3 startPosition = bondComponent.molecule1.transform.position;
                Vector3 endPosition = bondComponent.molecule2.transform.position;
                bond.transform.position = (startPosition + endPosition) / 2;
                bond.transform.LookAt(endPosition);
                bond.transform.localScale = new Vector3(bond.transform.localScale.x, bond.transform.localScale.y, Vector3.Distance(startPosition, endPosition));

                // Propagate the update to all clients
                UpdateBondPositionClientRpc(bondRef);
            }
        }
    }
    [ClientRpc]
    private void UpdateBondPositionClientRpc(NetworkObjectReference bondRef)
    {
        if (bondRef.TryGet(out NetworkObject bond))
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
    }

    private void DeleteMolecule(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.CompareTag("Molecule"))
            {
                MoleculeGroup group = clickedObject.GetComponentInParent<MoleculeGroup>();
                if (group != null)
                {
                    group.RemoveMolecule(clickedObject);
                    if (group.molecules.Count == 0 && group.bonds.Count == 0)
                    {
                        Destroy(group.gameObject);
                    }
                }
                else
                {
                    Destroy(clickedObject);
                }
                Debug.Log("Molecule deleted");
            }
        }
    }

    private void OnDestroy()
    {
        clickAction.Disable();
        rightClickAction.Disable();
    }
}