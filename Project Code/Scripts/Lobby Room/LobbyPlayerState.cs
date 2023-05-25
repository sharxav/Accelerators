//structure for the lobby player panels
//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/UI/LobbyPlayerState.cs
using System;
using Unity.Collections;
using Unity.Netcode;


public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public bool isReady;
  

    public LobbyPlayerState(ulong clientId, FixedString32Bytes playerName,bool IsReady)
    {
        ClientId = clientId;
        PlayerName = playerName;
        isReady = IsReady;
      
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref isReady);

    }

    public bool Equals(LobbyPlayerState other)
    {
        return ClientId == other.ClientId &&
            PlayerName.Equals(other.PlayerName) &&
            isReady == other.isReady;
           
    }
}
