namespace E_Commerce.Service.Exceptions;

public class CustomException : Exception
{
    public int Code { get; set; }
    public CustomException(string message, int code) : base(message)
    {
        Code = code;
    }
}
