using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using static Unity.Netcode.Transports.UTP.UnityTransport;

public class NetworkIPPortHandler : MonoBehaviour
{
    NetworkManager networkManagers;
    UnityTransport unityTransportManager;
    NetworkIPPortHandler instance;

    [AddComponentMenu("Address Management")]
    private string defaultIP;
    private string defaultPort;

    public string IP = "127.0.0.1";
    public ushort Port = 7777;

    ConnectionAddressData ConData;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if(instance != null)
            Destroy(this);

        unityTransportManager = GetComponent<UnityTransport>();
        networkManagers = GetComponent<NetworkManager>();

        if(networkManagers)
        {
            ConData = unityTransportManager.ConnectionData;
        }
    }

    public void changeIPandPort(string IP, ushort Port)
    {
        unityTransportManager.SetConnectionData(IP, Port);
        ConData.Address = IP;
        ConData.Port = Port;
    }

    public ConnectionAddressData getIPandPort()
    {
        return ConData;
    }

    #region Functionality Button
    // Cuman boleh dipanggil kalau di Main menu aja
    public void HostServer()
    {
        if (!networkManagers.IsConnectedClient)
            networkManagers.StartHost();
    }

    public void StopHost()
    {
        if (networkManagers.IsHost)
        {
            networkManagers.Shutdown();
        }
    }

    public void Connect()
    {
        networkManagers.StartClient();
    }

    public void Disconnect()
    {
        if (networkManagers.IsHost)
        {
            // Immediate Shutdown

            networkManagers.Shutdown(true);
        } else if (networkManagers.IsClient)
        {
            ulong localID = networkManagers.LocalClientId;
            networkManagers.DisconnectClient(localID);
        }
    }

    #endregion
}
