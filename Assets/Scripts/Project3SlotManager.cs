using Unity.Netcode;
using UnityEngine;

public class Project3SlotManager : NetworkBehaviour
{
    public static Project3SlotManager Instance;

    public static int localClaimedSlot = -1;

    public NetworkVariable<int> matchConfig = new NetworkVariable<int>(2);

    public NetworkVariable<ulong> leftSlot1Owner = new NetworkVariable<ulong>(ulong.MaxValue);
    public NetworkVariable<ulong> leftSlot2Owner = new NetworkVariable<ulong>(ulong.MaxValue);
    public NetworkVariable<ulong> rightSlot1Owner = new NetworkVariable<ulong>(ulong.MaxValue);
    public NetworkVariable<ulong> rightSlot2Owner = new NetworkVariable<ulong>(ulong.MaxValue);

    private void Awake()
    {
        Instance = this;
    }

    public void SetMatchConfig(int playerCount)
    {
        Project3GameSettings.selectedPlayerCount = playerCount;

        if (NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
        {
            matchConfig.Value = playerCount;
        }

        Debug.Log("Match config selected: " + playerCount);
    }

    public void ClaimSlot(int slotIndex)
    {
        localClaimedSlot = slotIndex;
        Debug.Log("LOCAL PLAYER CLAIMED SLOT: " + slotIndex);

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Slot saved locally. Start Host or Client for networking.");
            return;
        }

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ClaimSlotServerRpc(slotIndex, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClaimSlotServerRpc(int slotIndex, ulong clientId)
    {
        if (!IsSlotAllowed(slotIndex))
        {
            Debug.Log("Slot not allowed in this match configuration.");
            return;
        }

        if (slotIndex == 0 && leftSlot1Owner.Value == ulong.MaxValue)
        {
            leftSlot1Owner.Value = clientId;
            Debug.Log("Client " + clientId + " claimed Left Slot 1.");
        }
        else if (slotIndex == 1 && leftSlot2Owner.Value == ulong.MaxValue)
        {
            leftSlot2Owner.Value = clientId;
            Debug.Log("Client " + clientId + " claimed Left Slot 2.");
        }
        else if (slotIndex == 2 && rightSlot1Owner.Value == ulong.MaxValue)
        {
            rightSlot1Owner.Value = clientId;
            Debug.Log("Client " + clientId + " claimed Right Slot 1.");
        }
        else if (slotIndex == 3 && rightSlot2Owner.Value == ulong.MaxValue)
        {
            rightSlot2Owner.Value = clientId;
            Debug.Log("Client " + clientId + " claimed Right Slot 2.");
        }
        else
        {
            Debug.Log("Slot already taken.");
        }
    }

    private bool IsSlotAllowed(int slotIndex)
    {
        int config = Project3GameSettings.selectedPlayerCount;

        if (config == 2)
        {
            return slotIndex == 0 || slotIndex == 2;
        }

        if (config == 3)
        {
            return slotIndex == 0 || slotIndex == 1 || slotIndex == 2;
        }

        if (config == 4)
        {
            return true;
        }

        return false;
    }

    public bool IsLocalPlayerSlot(int slotIndex)
    {
        return localClaimedSlot == slotIndex;
    }
}