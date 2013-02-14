namespace HappyAuth.Libs.Interfaces
{
    interface IOAuthScope
    {
        /// <summary>
        /// The required scope to access a resource.
        /// </summary>
        string Scope { get; }
    }
}
