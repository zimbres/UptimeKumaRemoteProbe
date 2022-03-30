namespace UptimeKumaRemoteProbe.Data;

public class ApplicationDbContext : DbContext
{
    private readonly Endpoint _endpoint;

    public ApplicationDbContext(Endpoint endpoint)
    {
        _endpoint = endpoint;
    }

    public DbSet<DbVersion>? DbVersion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_endpoint.Brand)
        {
            case "MSSQL":
                optionsBuilder.UseSqlServer(_endpoint.ConnectionString);
                break;
            case "MYSQL":
                optionsBuilder.UseMySQL(_endpoint.ConnectionString);
                break;
            default:
                break;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
