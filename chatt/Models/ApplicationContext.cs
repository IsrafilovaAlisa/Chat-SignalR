using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace chatt.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<Groups> Groups { get; set; }
    public DbSet<GroupUsers> GroupUsers { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {

    }

}
