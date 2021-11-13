using CicekSepeti.Domain.Interface;
using System;

namespace CicekSepeti.Domain.Abstract
{
    public class BaseEntity<T> : IBaseEntity<T> where T : IEquatable<T>
    {
        public BaseEntity()
        {

        }

        public T Id { get; private set; }

        public int? CreatedBy { get; private set; }

        public int? ModifiedBy { get; private set; }

        public DateTime? CreatedDate { get; private set; }

        public DateTime? ModifyDate { get; private set; }

        public void SetId(T id)
        {
            Id = id;
        }

        public void SetCreator(int? userId)
        {
            CreatedBy = userId;
            CreatedDate = DateTime.Now;
        }

        public void SetModifier(int? userId)
        {
            ModifiedBy = userId;
            ModifyDate = DateTime.Now;
        }
    }
}
