/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Linq;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels.BaseModel;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework
{
    internal class TemplatingContext : DbContext
    {
        public TemplatingContext(ITemplatingPersistenceConfiguration configuration)
            : base(new DbContextOptionsBuilder<TemplatingContext>()
                .UseSqlServer(configuration.DatabaseConnectionString).Options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<TemplatePersistenceModel> Templates { get; set; }
        public DbSet<TemplatePagePersistenceModel> Pages { get; set; }
        public DbSet<TemplatePageCanvasPersistenceModel> ReferenceCanvases { get; set; }
        public DbSet<TemplatePageAreaPersistenceModel> PageAreas { get; set; }
        public DbSet<TemplatePageAreaPartPersistenceModel> PageAreaParts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemplatePersistenceModel>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<TemplatePersistenceModel>()
                .HasMany(t => t.DefinedPages)
                .WithOne();

            modelBuilder.Entity<TemplatePagePersistenceModel>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<TemplatePagePersistenceModel>()
                .HasMany(t => t.DefinedAreas)
                .WithOne();
            modelBuilder.Entity<TemplatePagePersistenceModel>()
                .HasOne(t => t.ReferenceCanvas)
                .WithOne();

            modelBuilder.Entity<TemplatePageAreaPersistenceModel>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TemplatePageAreaPartPersistenceModel>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TemplatePageCanvasPersistenceModel>()
                .HasKey(t => t.Id);
        }

        public void ReplaceLocallyAttachedWith<T>(T entityToAttach)
            where T : PersistenceModel
        {
            var entityToDetach = Set<T>().Local
                .SingleOrDefault(entry => entry.Id.Equals(entityToAttach.Id));

            if (entityToDetach != null)
            {
                Entry(entityToDetach).State = EntityState.Detached;
            }

            Entry(entityToAttach).State = EntityState.Modified;
        }
    }
}
