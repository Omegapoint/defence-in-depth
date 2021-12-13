using System;

namespace Defence.In.Depth.Domain.Services;

[Flags]
public enum AuthenticationMethods
{
    None = 0,
    
    Unknown = 0x1,
    
    Password = 0x2
}
