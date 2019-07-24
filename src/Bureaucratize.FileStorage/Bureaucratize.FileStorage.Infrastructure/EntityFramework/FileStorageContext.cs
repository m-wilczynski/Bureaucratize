using Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.FileStorage.Infrastructure.EntityFramework
{
    public class FileStorageContext : DbContext
    {
        public FileStorageContext(DbContextOptions<FileStorageContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        internal DbSet<DocumentToProcessBitmap> DocumentBitmaps { get; set; }
        internal DbSet<PageCanvasBitmap> PageCanvasBitmaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentToProcessBitmap>()
                .HasIndex(d => d.DocumentToProcessId);

            modelBuilder.Entity<PageCanvasBitmap>()
                .HasIndex(d => d.TemplateId);
            modelBuilder.Entity<PageCanvasBitmap>()
                .HasIndex(d => d.TemplatePageId)
                .IsUnique();
        }
    }
}
