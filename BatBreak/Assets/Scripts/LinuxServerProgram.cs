using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LinuxServerProgram : NetworkBehaviour
{
    void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-s")
            {
                Debug.Log("Running as server");
                NetworkManager.Singleton.StartServer();
                NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectMessage;
            }
        }
    }

    public void ClientConnectMessage(ulong connectionID)
    {
        Debug.Log(connectionID + " has connected");
    }
    
}
