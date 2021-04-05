namespace JewelleryStore.Models
{
    public class LoginResponseModel
    {
        public bool IsAuthorized { get; set; }

        public string ErrorMessage { get; set; }

        public string AccessToken { get; set; }
    }
}
