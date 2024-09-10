namespace Defence.In.Depth.Domain.Exceptions;

public class DomainPrimitiveArgumentException<T> : ArgumentException
{
    public DomainPrimitiveArgumentException()
    {
    }

    public DomainPrimitiveArgumentException(T value) : base($"The value {value} is not valid.")
    {
    }
        
    public DomainPrimitiveArgumentException(string message, T value) : base(message)
    {
        Value = value;
    }
        
    public T? Value { get; }
}