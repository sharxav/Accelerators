//structure for leaderboard information

public struct PlayerRank
{
    public string PlayerName { get; private set; }
    public int Rank { get; private set; }

    public string PlayerTime { get; private set; }
    public ulong ClientId { get; private set; }

   

    public PlayerRank(string playerName, int rank,string playerTime,ulong clientId)
    {
        PlayerName = playerName;
        Rank = rank;
        PlayerTime = playerTime;
        ClientId = clientId;
       


    }
}