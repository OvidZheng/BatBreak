using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button localserverButton;
    [SerializeField] private Button localclientButton;
    public string localServerIPAddress ="127.0.0.1";

    private void Awake()
    {
        serverButton.onClick.AddListener((() => NetworkManager.Singleton.StartServer()));
        hostButton.onClick.AddListener((() => NetworkManager.Singleton.StartHost()));
        clientButton.onClick.AddListener((() => NetworkManager.Singleton.StartClient()));
        localserverButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localServerIPAddress, 7777);
            NetworkManager.Singleton.StartServer();
        }));
        localclientButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localServerIPAddress, 7777);
            NetworkManager.Singleton.StartClient();
        }));
    }
}
