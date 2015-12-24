using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PictureAuction.SOA.Shared.ServiceModel
{
    [DataContract]
    public class PageResult<T>
    {
        private long? _pageCount;

        public PageResult()
        {
        }

        public PageResult(IReadOnlyCollection<T> items, long currentPage, long pageSize, long count)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            PageCount = (long) Math.Ceiling((double) count/pageSize);
            CurrentPage = currentPage;
            Items = items;
        }

        [DataMember]
        public long CurrentPage { get; set; }

        [DataMember]
        public IReadOnlyCollection<T> Items { get; set; }

        [DataMember]
        public long? PageCount
        {
            get { return _pageCount; }
            private set
            {
                if (value.HasValue && value.Value < 0L)
                    throw new ArgumentOutOfRangeException(nameof(value), value.Value, string.Empty);

                _pageCount = value;
            }
        }

        [DataMember]
        public long? PageSize => Items?.Count;
    }
}