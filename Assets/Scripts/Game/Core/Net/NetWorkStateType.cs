namespace Game.Core.Net
{
    public enum NetworkStateType
    {
        Connected,
        ConnectFail,
        Reconnected,
        ReconnectFail,
        Exception,
        Disconnect
    }
}