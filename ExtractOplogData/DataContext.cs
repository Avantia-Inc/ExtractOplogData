using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractOplogData
{
    public class DataContext : DbContext
    {
        public DbSet<OplogItem> OplogItems { get; set; }

        public DataContext() : base("DefaultConnection") { }

        public static DataContext Create()
        {
            return new DataContext();
        }
    }
}
