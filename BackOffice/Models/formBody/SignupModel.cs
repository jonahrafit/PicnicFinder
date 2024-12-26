public class SignupModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; } // Par exemple : "USER" ou "ADMIN" ou "OWNER"
    public required string Name { get; set; }
    public required string Phone { get; set; } 
}
