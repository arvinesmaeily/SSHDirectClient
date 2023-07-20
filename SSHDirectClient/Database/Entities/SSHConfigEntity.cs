using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSHDirectClient.Database.Entities
{
    [Table("SSHConfigs")]
    public class SSHConfigEntity
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("ServerAddress")]
        public string ServerAddress { get; set; }

        [Column("ServerPort")]
        public uint ServerPort { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Password")]
        public string Password { get; set; }



    }
}
