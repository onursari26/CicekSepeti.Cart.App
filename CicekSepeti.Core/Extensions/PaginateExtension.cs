using CicekSepeti.Core.Concrete;
using CicekSepeti.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace CicekSepeti.Core.Extensions
{
    public static class PaginateExtension
    {
        public static IPaginate<T> ToPaginate<T>(this IEnumerable<T> source, int index, int size, int from = 0)
        {
            return new Paginate<T>(source, index, size, from);
        }

        public static IPaginate<TResult> ToPaginate<TSource, TResult>(this IEnumerable<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int index, int size, int from = 0)
        {
            return new Paginate<TSource, TResult>(source, converter, index, size, from);
        }
    }
}
