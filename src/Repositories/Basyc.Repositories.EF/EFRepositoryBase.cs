using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF
{
    public abstract class EFRepositoryBase<TEntity, TModel> where TEntity : class
    {
        private static bool isDbContextValidated = false;
        protected readonly DbContext dbContext;
        protected readonly ILogger<EFRepositoryBase<TEntity, TModel>> logger;

        public EFRepositoryBase(DbContext dbContext, ILogger<EFRepositoryBase<TEntity, TModel>> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            if (isDbContextValidated is false)
            {
                ValidateDbContext(dbContext);
                isDbContextValidated = true;
            }
        }

        /// <summary>
        /// Checks if generic DbContext contains a requiered Set<<see cref="TEntity"/>>
        /// </summary>
        /// <param name="dbContext"></param>
        private void ValidateDbContext(DbContext dbContext)
        {
            logger.LogInformation($"Validating dbContext: {dbContext.GetType().Name}");
            dbContext.Set<TEntity>();
        }

        protected abstract TEntity ToEntity(TModel model);

        protected abstract TModel ToModel(TEntity entity);
    }
}