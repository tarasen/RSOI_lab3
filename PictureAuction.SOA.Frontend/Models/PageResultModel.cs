using PictureAuction.SOA.Shared.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace PictureAuction.SOA.Frontend.Models
{
    public class PageResultModel<T>
    {
        public PageResultModel(PageResult<T> page)
        {
            Items = page.Items.ToList();
            PrevPage = page.CurrentPage == 1 ? (long?) null : page.CurrentPage - 1;
            NextPage = page.CurrentPage == page.PageCount.Value ? (long?) null : page.CurrentPage + 1;
        }

        public bool Authorized { get; set; }
        public List<T> Items { get; set; }
        public long? NextPage { get; set; }
        public long? PrevPage { get; set; }
    }
}