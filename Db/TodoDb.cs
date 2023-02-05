using Microsoft.EntityFrameworkCore;
using skyOne.Models;


namespace skyOne.Db
{
    public class TodoDb:DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options):base(options)
        {
            
        }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}