namespace ManagedDoom.Config;

public readonly record struct Arg(bool Present);

public readonly struct Arg<T>
{
    public bool Present { get; }

    public T Value { get; }

    public Arg()
    {
        this.Present = false;
        this.Value = default;
    }

    public Arg(T value)
    {
        this.Present = value is not null;
        this.Value = value;
    }
}