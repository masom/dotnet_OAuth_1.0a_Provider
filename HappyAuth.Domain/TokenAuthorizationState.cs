namespace HappyAuth.Domain
{
    /// <summary>
    /// Possible token authorization state values.
    /// </summary>
    public enum TokenAuthorizationState : int
    {
        UnauthorizedRequestToken = 0,
        AuthorizedRequestToken = 1,
        AccessToken = 2
    }
}