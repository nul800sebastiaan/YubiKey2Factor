using Umbraco.Core.Persistence;

namespace TwoFactorAuthentication.Models
{
    [TableName(Constants.ProductName)]
    [PrimaryKey("userId", autoIncrement = false)]
    public class TwoFactor
    {
        [Column("userId")]
        public int UserId { get; set; }

        [Column("key")]
        public string Key { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("confirmed")]
        public bool Confirmed { get; set; }
    }
}