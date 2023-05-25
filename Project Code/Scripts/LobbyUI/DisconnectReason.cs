// Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/Networking/DisconnectReason.cs
public class ClientDisconnection
{
    public ConnectStatus Reason { get; private set; } = ConnectStatus.Undefined;

    public void SetClientDisconnection(ConnectStatus reason)
    {
        Reason = reason;
    }

    public void Clear()
    {
        Reason = ConnectStatus.Undefined;
    }

    public bool HasTransitionReason => Reason != ConnectStatus.Undefined;
}
