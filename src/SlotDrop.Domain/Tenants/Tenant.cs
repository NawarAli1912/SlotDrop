namespace SlotDrop.Domain.Tenants;

// A barber shop or clinic. Only the id and name exist until an episode needs more (the slug arrives in E1).
public sealed class Tenant
{
    public Tenant(Guid id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }
}
