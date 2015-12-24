using System;

namespace PictureAuction.SOA.Shared.DTOs
{
    public static class AuthDTO
    {
        public class AuthResponse
        {
            public Guid ClientId { get; set; }
            public string RedirectUri { get; set; }
            public string State { get; set; }
        }

        public class TokenResponse
        {
            public Guid AccessToken { get; set; }
            public long ExpiresIn { get; set; }
            public string TokenType { get; set; } = "Bearer";
        }
    }
}