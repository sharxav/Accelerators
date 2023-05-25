//updates the leaderboard player panel
using UnityEngine;
using TMPro;

public class LeaderboardPlayerCard : MonoBehaviour
{

    [SerializeField] private TMP_Text playerDisplayNameText;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TMP_Text TimeDisplay;
    [SerializeField] private TMP_Text playerRankDisplay;

    public void Display(int rank, string playerName, string playerTime)
    {
        playerPanel.SetActive(true);
        playerDisplayNameText.text = playerName;
        TimeDisplay.text = playerTime;
        playerRankDisplay.text = rank.ToString();
    }
}
