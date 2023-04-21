using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Domain.Entities;

public sealed class UserEntity
{
    [Key]
    public int Id { get; set; }

    [Required] 
    [MaxLength(20)] 
    public string Nickname { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = null!;
        
    [Required]
    [MaxLength(50)]
    public string PasswordHash { get; set; } = null!;
        
    [Required]
    [MaxLength(50)]
    public string PasswordSalt { get; set; } = null!;
    
    public string SearchValue => Nickname.ToLower();
}