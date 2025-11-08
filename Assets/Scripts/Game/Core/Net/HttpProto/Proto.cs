using Newtonsoft.Json;

namespace Game.Core.Net.HttpProto
{
    public class ApiResponse<T>
    {
        [JsonProperty("success")] public bool Success { get; set; }

        [JsonProperty("errorCode")] public int ErrorCode { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("data")] public T Data { get; set; }
    }

    public class ServerInfo
    {
        [JsonIgnore] [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("ip")] public string Ip { get; set; }

        [JsonProperty("port")] public string Port { get; set; }

        [JsonProperty("serverId")] public int ServerId { get; set; }

        [JsonProperty("channel")] public string Channel { get; set; }

        [JsonProperty("plat")] public string Plat { get; set; }

        [JsonProperty("state")] public int State { get; set; }

        [JsonProperty("openTime")] public string OpenTime { get; set; }

        [JsonProperty("isNew")] public int IsNew { get; set; }

        [JsonProperty("serverVersion")] public string ServerVersion { get; set; }
    }

    public class UserLoginResponse
    {
        [JsonProperty("openId")] public string OpenId { get; set; }

        [JsonProperty("loginToken")] public string LoginToken { get; set; }

        [JsonProperty("refreshToken")] public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        [JsonProperty("loginToken")] public string LoginToken { get; set; }

        [JsonProperty("refreshToken")] public string RefreshToken { get; set; }
    }

    public class UserInfoResponse
    {
        [JsonProperty("userId")] public string UserId { get; set; }

        [JsonProperty("userToken")] public string UserToken { get; set; }
    }
}