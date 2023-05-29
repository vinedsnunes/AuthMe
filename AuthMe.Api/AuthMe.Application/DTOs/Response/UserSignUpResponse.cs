namespace AuthMe.Application.DTOs.Response
{
    public class UserSignUpResponse
    {
        public bool Success { get; private set; }
        public List<string> Errors { get; private set; }

        public UserSignUpResponse() => 
            Errors = new List<string>();
            
            public UserSignUpResponse(bool success = true) : this() =>
            Success = success;
            
            public void AddErrors(IEnumerable<string> errors) =>
            Errors.AddRange(errors);
    }
}
