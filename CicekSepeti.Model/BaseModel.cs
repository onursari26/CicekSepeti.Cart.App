using System;

namespace CicekSepeti.Model
{
    public class BaseModel<T> : IBaseModel<T> where T : IEquatable<T>
    {
        public T Id { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifyDate { get; set; }
    }

    public interface IBaseModel
    {

    }

    public interface IBaseModel<T> : IBaseModel where T : IEquatable<T>
    {
        T Id { get; set; }

        int? CreatedBy { get; set; }

        int? ModifiedBy { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? ModifyDate { get; set; }
    }
}
