using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Database.Repositories;

namespace Softplan.TaskManager.Database.DependencyResolution;

public static class Ioc
{
    public static void AddDatabase(this IServiceCollection services)
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}