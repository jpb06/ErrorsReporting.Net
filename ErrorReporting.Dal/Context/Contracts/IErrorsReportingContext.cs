using ErrorReporting.Dal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Context.Contracts
{
    public interface IErrorsReportingContext
    {
        IDbSet<ErrorReportApplication> Applications { get; set; }
        IDbSet<ErrorReportException> Exceptions { get; set; }

        Database Database { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry Entry(object entity);
        int SaveChanges();

        void Dispose();
    }
}
