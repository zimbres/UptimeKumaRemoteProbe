namespace UptimeKumaRemoteProbe.Data;

public class ApplicationDbContext(Endpoint endpoint) : DbContext
{
    public DbSet<DbVersion> DbVersion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (endpoint.Brand)
        {
            case "MSSQL":
                optionsBuilder.UseSqlServer(endpoint.ConnectionString);
                break;
            case "MYSQL":
                optionsBuilder.UseMySQL(endpoint.ConnectionString);
                break;
            case "PGSQL":
                optionsBuilder.UseNpgsql(endpoint.ConnectionString);
                break;
            default:
                break;
        }
    }
}
