namespace HappyAuth.Domain.Interfaces
{
    public interface IOAuthScope
    {
        /// <summary>
        /// The required scope to access a resource.
        /// </summary>
        string Scope { get; }
    }
}
