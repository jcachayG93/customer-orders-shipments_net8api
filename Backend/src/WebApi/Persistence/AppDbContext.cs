using Microsoft.EntityFrameworkCore;

namespace WebApi.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options);