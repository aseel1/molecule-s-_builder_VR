using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            Debug.Log("Starting Server...");
            NetworkManager.Singleton.StartServer();
        });

        clientBtn.onClick.AddListener(() =>
        {
            Debug.Log("Starting Client...");
            NetworkManager.Singleton.StartClient();
        });

        hostBtn.onClick.AddListener(() =>
        {
            Debug.Log("Starting Host...");
            NetworkManager.Singleton.StartHost();
        });
    }
}