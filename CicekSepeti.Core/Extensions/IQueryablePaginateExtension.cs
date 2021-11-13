using CicekSepeti.Core.Concrete;
using CicekSepeti.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CicekSepeti.Core.Extensions
{
    public static class IQueryablePaginateExtension
    {
        public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int page, int pageSize,
    int from = 0, CancellationToken cancellationToken = default)
        {

            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip(page * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            var list = new Paginate<T>
            {
                Page = page,
                PageSize = pageSize,
                From = from,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)pageSize)
            };

            return list;
        }
    }
}
