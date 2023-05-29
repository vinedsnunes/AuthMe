using System.Text.Json.Serialization;

namespace AuthMe.Application.DTOs.Response
{
    public class UserSignInResponse
    {
        public bool Success => Errors.Count == 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefreshToken { get; private set; }

        public List<string> Errors { get; private set; }

        public UserSignInResponse() =>
            Errors = new List<string>();

        public UserSignInResponse(bool success, string accessToken, string refreshToken) : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public void AddError(string error) =>
            Errors.Add(error);

        public void AddErrors(IEnumerable<string> errors) =>
            Errors.AddRange(errors);
    }
}
