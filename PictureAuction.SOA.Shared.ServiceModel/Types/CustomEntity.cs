using ServiceStack.DesignPatterns.Model;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    public class CustomEntity : IHasIntId
    {
        public int Id { get; set; }
    }
}