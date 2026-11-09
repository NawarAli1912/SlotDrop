namespace SlotDrop.Infrastructure.Persistence;

// HasData compares the seed with the last migration's snapshot, so every value must be a constant.
public static class SeedData
{
    public static readonly Guid FadeStudioId = new("01a1c14b-fcfd-780d-9a99-eba805d438c7");

    public const string FadeStudioName = "Fade Studio";
}
