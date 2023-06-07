using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataAccess;
public class DatabaseContext: DbContext
{
	public DatabaseContext(DbContextOptions options):base(options)
	{
		//options contains DB connection string
	}

	//dbsets are used to query & save instances of entities to database
	public DbSet<PostEntity> Posts { get; set; } //table name: Post (singular)
	public DbSet<CommentEntity> Comments { get; set; }

	
}
