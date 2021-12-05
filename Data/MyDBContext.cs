using Microsoft.EntityFrameworkCore;
using Empty_MVC.Models;

namespace Empty_MVC.Data
{
    public class MyDataContext : DbContext
    {
        public MyDataContext(DbContextOptions<MyDataContext> options) : base(options)
        {
        }
        public DbSet<UsersModel> UsersModels { get; set; }
    }
}
