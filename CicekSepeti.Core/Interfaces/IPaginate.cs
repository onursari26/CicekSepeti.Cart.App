using System.Collections.Generic;

namespace CicekSepeti.Core.Interfaces
{
    public interface IPaginate<T>
    {
        int From { get; }

        int Page { get; }

        int PageSize { get; }

        int Count { get; }

        int Pages { get; }

        IList<T> Items { get; }

        bool HasPrevious { get; }

        bool HasNext { get; }
    }
}
