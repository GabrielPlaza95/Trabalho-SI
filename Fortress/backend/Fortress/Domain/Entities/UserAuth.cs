using Fortress.Domain.Enums;

namespace Fortress.Domain.Entities
{
    public class UserAuth
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public AuthFactorEnum AuthFactorId { get; set; }
        public DateTime LastAuthTimeUtc { get; set; }
    }
}
