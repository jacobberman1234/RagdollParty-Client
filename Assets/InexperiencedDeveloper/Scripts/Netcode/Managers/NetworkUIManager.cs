using InexperiencedDeveloper.Core;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : Singleton<NetworkUIManager>
{
    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonStartHost;
    [SerializeField] private Button buttonStartClient;

    private void Start()
    {
        buttonStartHost.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartHost())
            {
                InexperiencedDeveloper.Utils.Log.DebugLogger.Log($"Host started...");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
                InexperiencedDeveloper.Utils.Log.DebugLogger.LogWarning($"Cannot start host...");
        });

        buttonStartClient.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                InexperiencedDeveloper.Utils.Log.DebugLogger.Log($"Client started...");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

            }
            else
                InexperiencedDeveloper.Utils.Log.DebugLogger.LogWarning($"Cannot start client...");
        });

        buttonStartServer.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
            {
                InexperiencedDeveloper.Utils.Log.DebugLogger.Log($"Server started...");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
                InexperiencedDeveloper.Utils.Log.DebugLogger.LogWarning($"Cannot start server...");
        });
    }
}
