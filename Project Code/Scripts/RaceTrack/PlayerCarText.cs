//script that displays the player name over the car
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCarText : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    public static PlayerCarText Instance => instance;
    private static PlayerCarText instance;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        PlayerData? playerData = ServerGameNetPortal.Instance.GetPlayerData(OwnerClientId);

        if (playerData.HasValue)
        {
            playerName.Value = playerData.Value.PlayerName;
            Debug.Log("Player_Name "+playerName.Value);
        }
       
    }

    private void OnEnable()
    {
        playerName.OnValueChanged += HandleDisplayNameChanged;
    }

    private void OnDisable()
    {
        playerName.OnValueChanged -= HandleDisplayNameChanged;
    }

    private void HandleDisplayNameChanged(FixedString32Bytes oldDisplayName, FixedString32Bytes newDisplayName)
    {
        displayNameText.text = newDisplayName.ToString();
    }

   
}

