using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace PictureAuction.SOA.Artists.ServiceInterface
{
    public static class DbConnectionEx
    {
        public static bool Any<T>(this IDbConnection db, Expression<Func<T, bool>> predicate) where T : new()
        {
            return db.Select<T>(q => q.Where(predicate).Limit(1)).Any();
        }
    }
}