namespace ArrayModifiers.Scripts
{
    public partial class ArrayModifier
    {
        public static class Fields
        {
            public const string Original = nameof(original);
            public const string Amount = nameof(amount);
            public const string BakeMeshes = nameof(bakeMeshes);
            public const string AddCollider = nameof(addCollider);
            public const string Initialized = nameof(initialized);
#if UNITY_EDITOR
            public const string StaticFlags = nameof(staticFlags);
            public const string GenerateLightmapUVs = nameof(generateLightmapUVs);
#endif
        }
    }
}