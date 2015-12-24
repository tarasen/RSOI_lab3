using PictureAuction.SOA.Shared.ServiceModel;
using ServiceStack.Service;
using ServiceStack.Text;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Extentions
{
    internal static class JsonRestApi
    {
        public static async Task<T> GetItemAsync<T>(this IRestClientAsync client, string query)
        {
            var task = new TaskCompletionSource<T>();

            client.GetAsync<string>(query, s => task.SetResult(s.FromJson<T>()),
                (s, e) => task.SetResult(default(T)));
            return await task.Task;
        }

        internal static async Task<bool> DeleteItemAsync(this IRestClientAsync client, int id)
        {
            var task = new TaskCompletionSource<HttpWebResponse>();

            client.DeleteAsync<HttpWebResponse>(id.ToString(), b => task.SetResult(b), (s, e) => task.SetResult(s));
            return (await task.Task).StatusCode == HttpStatusCode.NoContent;
        }

        internal static async Task<string> GetImageAsync(this IRestClientAsync client, string fileName)
        {
            var task = new TaskCompletionSource<string>();

            client.GetAsync<byte[]>(fileName, b => task.SetResult(Convert.ToBase64String(b)),
                (s, e) => task.SetResult(string.Empty));
            return await task.Task;
        }

        internal static async Task<PageResult<T>> GetPageAsync<T>(this IRestClientAsync client, int? page, int? pageSize)
        {
            var query = new StringBuilder();

            if (page.HasValue)
                query.Append($"?page={page.Value}");
            if (pageSize.HasValue)
                query.Append(query.Length == 0 ? "?" : "&").Append($"page_size={pageSize.Value}");

            return await client.GetItemAsync<PageResult<T>>(query.ToString());
        }

        internal static async Task<T> PostItemAsync<T>(this IRestClientAsync client, T obj)
        {
            var task = new TaskCompletionSource<T>();

            client.PostAsync<T>("", obj, el => task.SetResult(el), (s, e) => task.SetResult(default(T)));
            return await task.Task;
        }

        internal static async Task<bool> PutItemAsync<T>(this IRestClientAsync client, T obj, int id)
        {
            var task = new TaskCompletionSource<HttpWebResponse>();

            client.PutAsync<HttpWebResponse>(id.ToString(), obj, b => task.SetResult(b), (s, e) => task.SetResult(s));
            return (await task.Task).StatusCode == HttpStatusCode.OK;
        }
    }
}