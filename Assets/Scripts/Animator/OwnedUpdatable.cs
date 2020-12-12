public abstract class OwnedUpdatable : IUpdateable
{
    object _owner;

    public OwnedUpdatable SetOwner(object owner)
    {
        _owner = owner;
        return this;
    }

    public bool IsOwnedBy(object owner)
    {
        return _owner == owner;
    }

    public abstract void Update();
    public abstract bool IsDone();
}