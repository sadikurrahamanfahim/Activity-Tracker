using Postgrest.Attributes;
using Postgrest.Models;

namespace Activity_Tracker
{
    public class ActivityStatus : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("last_active")]
        public DateTime LastActive { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }
    }
}
