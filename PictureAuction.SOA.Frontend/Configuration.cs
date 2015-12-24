namespace PictureAuction.SOA.Frontend
{
    public static class Configuration
    {
        static Configuration()
        {
            EncryptionKey = "SuperSecretPass";
            HmacKey = "UberSuperSecret";
            PicturesBackendUri = "http://localhost:1333/pictures";
            ArtistsBackendUri = "http://localhost:1330/artists";
            ImagesBackendUri = "http://localhost:1331/img";
            SessionBackendUri = "http://localhost:1334/user";
        }

        public static string ArtistsBackendUri { get; }
        public static string EncryptionKey { get; }
        public static string HmacKey { get; }

        public static string ImagesBackendUri { get; }
        public static string PicturesBackendUri { get; }
        public static string SessionBackendUri { get; }
    }
}