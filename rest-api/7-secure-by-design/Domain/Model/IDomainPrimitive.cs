namespace Defence.In.Depth.Domain.Model
{
    public interface IDomainPrimitive<out T>
    {
        T Value { get; }
    }
}