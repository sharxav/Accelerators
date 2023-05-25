//script to save the player name
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ChangeScene :MonoBehaviour
{
    [SerializeField] private TMP_InputField inp;

    public void setName()
    {
        PlayerPrefs.SetString("PlayerName",inp.text);
        SceneManager.LoadScene("LobbyUI",LoadSceneMode.Single);
    }
}
