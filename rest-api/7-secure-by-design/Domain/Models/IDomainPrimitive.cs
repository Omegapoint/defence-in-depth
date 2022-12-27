namespace Defence.In.Depth.Domain.Models;

public interface IDomainPrimitive<out T>
{
    T Value { get; }
}