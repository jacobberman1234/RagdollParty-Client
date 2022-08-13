using InexperiencedDeveloper.Network.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerInfo : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();

    private bool nameSet = false;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
            playerName.Value = $"Player {OwnerClientId}";
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TMP_Text>();
        localPlayerOverlay.SetText(playerName.Value);
        nameSet = true;
    }

    private void Update()
    {
        if (!nameSet && !string.IsNullOrEmpty(playerName.Value))
            SetOverlay();
    }
}


