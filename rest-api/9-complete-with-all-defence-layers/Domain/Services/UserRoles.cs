namespace Defence.In.Depth.Domain.Services;

[Flags]
public enum UserRoles
{
    None = 0,
    
    Unknown = 0x1,
    
    TeamMember = 0x2,

    ProductManager = 0x4
}
