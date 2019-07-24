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

using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Recognition;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.ImageProcessing.Infrastructure.EntityFramework
{
    public class ImageProcessingContext : DbContext
    {
        internal ImageProcessingContext(IImageProcessingPersistenceConfiguration configuration)
            : base(new DbContextOptionsBuilder<ImageProcessingContext>()
                    .UseSqlServer(configuration.DatabaseConnectionString).Options)
        {
            Database.EnsureCreated();
        }

        internal DbSet<DocumentToProcessPersistenceModel> DocumentsToProcess { get; set; }
        internal DbSet<ProcessedDocumentPage> ProcessedPages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentToProcessPersistenceModel>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<ProcessedDocumentPage>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<RecognizedChoicePart>()
                .Ignore(b => b.AreaName)
                .HasKey(b => b.Id);

            modelBuilder.Entity<RecognizedTextPart>()
                .Ignore(b => b.AreaName)
                .HasKey(b => b.Id);
        }
    }
}
