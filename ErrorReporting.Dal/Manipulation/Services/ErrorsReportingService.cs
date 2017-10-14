using ErrorReporting.Dal.Context.Contracts;
using ErrorReporting.Dal.Manipulation.Repositories.Contracts;
using ErrorReporting.Dal.Manipulation.Services.Contracts;
using ErrorReporting.Dal.Models;
using ErrorReporting.Dal.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Manipulation.Services
{
    public class ErrorsReportingService : IErrorsReportingService
    {
        private IErrorsReportingContext context;

        private IGenericRepository<ErrorReportApplication> applicationsRepository;
        private IGenericRepository<ErrorReportException> exceptionRepository;

        public ErrorsReportingService(IErrorsReportingContext context,
                                      IGenericRepository<ErrorReportApplication> applicationsRespository,
                                      IGenericRepository<ErrorReportException> exceptionsRespository)
        {
            this.context = context;
            this.applicationsRepository = applicationsRespository;
            this.exceptionRepository = exceptionsRespository;
        }

        private int Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    int result = this.context.SaveChanges();
                    return result;
                }
                catch (DbUpdateConcurrencyException exception)
                {
                    saveFailed = true;

                    DbEntityEntry entry = exception.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
                catch (Exception exception)
                {
                    exception.HandleException();
                }

            } while (saveFailed);

            return 0;
        }

        public ErrorReportApplication CreateApplication(string name, string version)
        {
            ErrorReportApplication application = new ErrorReportApplication
            {
                Name = name,
                Version = version,
                FirstRunDate = DateTime.Now
            };

            this.applicationsRepository.Insert(application);
            int result = this.Save();

            return result == 1 ? application : null;
        }

        public ErrorReportApplication GetApplication(string name, string version)
        {
            ErrorReportApplication application = this.applicationsRepository
                                                     .Get(el => el.Name == name && el.Version == version)
                                                     .SingleOrDefault();
            return application;
        }

        public int? LogException(int idApplication, Exception exception, string errorCodeFullyQualifiedName)
        {
            if (exception == null) return null;

            var exceptionModel = new ErrorReportException();
            exceptionModel.IdApplication = idApplication;
            exceptionModel.Type = exception.GetType().ToString();
            exceptionModel.Message = exception.Message;
            exceptionModel.Source = exception.Source;
            if (exception.TargetSite != null && exception.TargetSite.Module != null)
                exceptionModel.SiteModule = exception.TargetSite.Module.Name;
            exceptionModel.SiteName = exception.TargetSite.Name;
            exceptionModel.StackTrace = exception.StackTrace;
            exceptionModel.HelpLink = exception.HelpLink;
            exceptionModel.Date = DateTime.Now;
            exceptionModel.IdInnerException = this.LogException(idApplication, exception.InnerException, errorCodeFullyQualifiedName);

            exceptionModel.CustomErrorType = errorCodeFullyQualifiedName;

            this.exceptionRepository.Insert(exceptionModel);

            int result = this.Save();

            return result == 1 ? exceptionModel.Id : (int?)null;
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
