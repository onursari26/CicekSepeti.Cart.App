using CicekSepeti.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CicekSepeti.Core.Concrete
{
    public class Paginate<T> : IPaginate<T>
    {
        public Paginate(IEnumerable<T> source, int index, int size, int from)
        {
            var enumerable = source as T[] ?? source.ToArray();

            if (from > index)
                throw new ArgumentException($"indexFrom: {from} > pageIndex: {index}, must indexFrom <= pageIndex");

            if (source is IQueryable<T> querable)
            {
                Page = index;
                PageSize = size;
                From = from;
                Count = querable.Count();
                Pages = (int)Math.Ceiling(Count / (double)PageSize);
                Items = querable.Skip((Page - From) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                Page = index;
                PageSize = size;
                From = from;

                Count = enumerable.Count();
                Pages = (int)Math.Ceiling(Count / (double)PageSize);

                Items = enumerable.Skip((Page - From) * PageSize).Take(PageSize).ToList();
            }
        }

        internal Paginate()
        {
            Items = new T[0];
        }

        public int From { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }
        public IList<T> Items { get; set; }
        public bool HasPrevious => Page - From > 0;
        public bool HasNext => Page - From + 1 < Pages;
    }

    internal class Paginate<TSource, TResult> : IPaginate<TResult>
    {
        public Paginate(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int index, int size, int from)
        {
            var enumerable = source as TSource[] ?? source.ToArray();

            if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

            if (source is IQueryable<TSource> queryable)
            {
                Page = index;
                PageSize = size;
                From = from;
                Count = queryable.Count();
                Pages = (int)Math.Ceiling(Count / (double)PageSize);

                var items = queryable.Skip((Page - From) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
            else
            {
                Page = index;
                PageSize = size;
                From = from;
                Count = enumerable.Count();
                Pages = (int)Math.Ceiling(Count / (double)PageSize);

                var items = enumerable.Skip((Page - From) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
        }


        public Paginate(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            Page = source.Page;
            PageSize = source.PageSize;
            From = source.From;
            Count = source.Count;
            Pages = source.Pages;

            Items = new List<TResult>(converter(source.Items));
        }

        public int Page { get; }

        public int PageSize { get; }

        public int Count { get; }

        public int Pages { get; }

        public int From { get; }

        public IList<TResult> Items { get; }

        public bool HasPrevious => Page - From > 0;

        public bool HasNext => Page - From + 1 < Pages;
    }

    public static class Paginate
    {
        public static IPaginate<T> Empty<T>()
        {
            return new Paginate<T>();
        }

        public static IPaginate<TResult> From<TResult, TSource>(IPaginate<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            return new Paginate<TSource, TResult>(source, converter);
        }
    }
}
