//script for updating the state of every lobby player
//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/UI/LobbyPlayerCard.cs

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{   
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Button statusBtn;
    [SerializeField] private TMP_Text statusTxt;

   
    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {      
        playerPanel.SetActive(true); 
        playerNameText.text = lobbyPlayerState.PlayerName.ToString();
        statusTxt.text = lobbyPlayerState.isReady ? "Ready" : "Not Ready";
        statusBtn.GetComponent<Image>().color = lobbyPlayerState.isReady ? Color.green : Color.red;
    }

    public void DisableDisplay()
    {
        playerPanel.SetActive(false);
    }

}
