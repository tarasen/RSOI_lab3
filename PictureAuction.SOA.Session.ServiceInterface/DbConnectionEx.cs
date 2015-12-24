using ServiceStack.OrmLite;
using System.Data;
using System.Linq;

namespace PictureAuction.SOA.Session.ServiceInterface
{
    public static class DbConnectionEx
    {
        public static T FirstOrDefaultById<T>(this IDbConnection db, object id) where T : new()
        {
            var key = OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName);
            var value = OrmLiteConfig.DialectProvider.GetQuotedValue(id, id.GetType());
            return db.Select<T>(x => x.Limit(1).Where($"{key} = {value}")).FirstOrDefault();
        }
    }
}