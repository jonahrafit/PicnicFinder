public class TokenModel
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public DateTime Expiration { get; set; }

    public override string ToString()
    {
        return $"TokenModel [UserId={UserId}, Name={Name}, Role={Role}, Expiration={Expiration}]";
    }
}
