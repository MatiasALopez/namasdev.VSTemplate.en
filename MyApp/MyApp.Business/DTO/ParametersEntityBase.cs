using System;

namespace MyApp.Business.DTO
{
    public class ParametersEntityBase<TId> : ParametersWithUserBase
        where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
    }
}
