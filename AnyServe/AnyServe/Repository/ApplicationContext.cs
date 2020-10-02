using AnyServe.Providers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnyServe.Storage
{
    public class ApplicationContext : IdentityDbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        { 
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //add all generic models to context
            var assembliesTypes = GenericTypesProvider.GetModelTypes();

            foreach (var assemblyEntry in assembliesTypes)
            {
                var candidates = assemblyEntry.Value;
                foreach (var candidate in candidates)
                {
                    modelBuilder.Entity(candidate);
                }
            }
        }
    }
}
