namespace PictureAuction.SOA.Frontend.Authentication
{
    public class PasswordIdentity : UserIdentity
    {
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
    }
}