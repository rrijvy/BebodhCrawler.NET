namespace Core.Models
{
    public class SignInResponseModel
    {
        public SignInResponseModel()
        {
            Message = string.Empty;
            ExpiresAt = DateTime.MinValue;
            IsSuccess = false;
        }

        public string Message { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsSuccess { get; set; }
    }
}
