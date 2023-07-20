using SQLite;
using SSHDirectClient.Database.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SSHDirectClient.Database
{
    public class DatabaseHandler
    {
        private string path = "configs";
        private SQLiteConnection _db;

        public DatabaseHandler()
        {
            if (!File.Exists(path))
            {
                _db = new SQLiteConnection(path);
                _db.CreateTable<SSHConfigEntity>();
            }
            else
            {
                _db = new SQLiteConnection(path);
            }
        }

        public void Insert(SSHConfigEntity config)
        {
            _db.Insert(config);
        }

        public void Delete(SSHConfigEntity config)
        {
            _db.Delete(config);
        }

        public void Update(SSHConfigEntity config)
        {
            _db.Update(config);
        }

        public List<SSHConfigEntity> GetAll()
        {
             return _db.Query<SSHConfigEntity>("SELECT * FROM SSHConfigs");
        }
    }
}
