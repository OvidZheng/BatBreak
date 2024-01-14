using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;




public class  NetworkManagerUI : MonoBehaviour
{
    public delegate void ColorChangeAction();
    
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button localserverButton;
    [SerializeField] private Button localclientButton;
    [SerializeField] private Button changeColorButton;
    public string localServerIPAddress ="127.0.0.1";
    
    public static event ColorChangeAction OnColorChangeRequested;
    private void Awake()
    {
        serverButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartServer();
            GameManager.Instance.ServerInitGame();
        }));
        hostButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartHost();
            GameManager.Instance.ServerInitGame();
        }));
        clientButton.onClick.AddListener((() =>
        {
            // GameManager.Instance.ClientInitGame();
            NetworkManager.Singleton.StartClient();
        }));
        localserverButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localServerIPAddress, 7777);
            NetworkManager.Singleton.StartServer();
            GameManager.Instance.ServerInitGame();
        }));
        localclientButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localServerIPAddress, 7777);
            // GameManager.Instance.ClientInitGame();
            NetworkManager.Singleton.StartClient();
        }));
        changeColorButton.onClick.AddListener(((() =>
        {
            OnColorChangeRequested.Invoke();
        })));
    }
}

