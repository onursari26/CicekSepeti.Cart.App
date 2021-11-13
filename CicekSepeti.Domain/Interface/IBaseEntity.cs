using System;

namespace CicekSepeti.Domain.Interface
{
    public interface IBaseEntity
    {

    }

    public interface IBaseEntity<T> : IBaseEntity where T : IEquatable<T>
    {
        T Id { get; }

        int? CreatedBy { get; }

        int? ModifiedBy { get; }

        DateTime? CreatedDate { get; }

        DateTime? ModifyDate { get; }
    }
}
