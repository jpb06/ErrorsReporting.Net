using ErrorReporting.Dal.Context;
using ErrorReporting.Dal.Context.Contracts;
using ErrorReporting.Dal.Manipulation.Repositories;
using ErrorReporting.Dal.Models;
using ErrorReporting.Shared.Tests.Data.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Tests.UnitTests.Manipulation.Repositories
{
    [TestFixture]
    public class DBaseGenericRepositoryTest
    {
        private IErrorsReportingContext context;
        private GenericRepository<ErrorReportApplication> repository;
        private PersistentErrorsReportingDataSet dataSet;
        private ErrorReportApplication addApplication;

        public DBaseGenericRepositoryTest()
        {
            this.dataSet = new PersistentErrorsReportingDataSet();
        }

        [OneTimeSetUp]
        public void Init()
        {
            this.context = new ErrorsReportingContext();
            this.repository = new GenericRepository<ErrorReportApplication>(this.context);
            this.dataSet.Initialize();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            this.dataSet.Destroy();
            this.dataSet.Dispose();
            this.context.Dispose();
        }

        [Test, Order(1)]
        public void Db_Repository_AddApplication()
        {
            this.addApplication = new ErrorReportApplication
            {
                 Name = "TestApplication", 
                 Version = "1.0.0.0",
                 FirstRunDate = DateTime.Now
            };

            repository.Insert(this.addApplication);
            int result = this.context.SaveChanges();

            Assert.AreEqual(1, result);
        }

        [Test, Order(2)]
        public void Db_Repository_UpdateApplication()
        {
            this.addApplication.Name = "TestApplication updated";

            this.repository.Update(this.addApplication);
            int result = this.context.SaveChanges();

            Assert.AreEqual(1, result);

        }

        [Test, Order(3)]
        public void Db_Repository_GetApplicationById()
        {
            ErrorReportApplication application = this.repository.GetByID(this.addApplication.Id);

            Assert.IsNotNull(application);
            Assert.AreEqual(this.addApplication.Name, application.Name);
            Assert.AreEqual(this.addApplication.FirstRunDate, application.FirstRunDate);
            Assert.AreEqual(this.addApplication.RowVersion, application.RowVersion);
        }

        [Test]
        public void Db_Repository_GetApplicationById_DoesntExist()
        {
            ErrorReportApplication application = this.repository.GetByID(0);

            Assert.AreEqual(null, application);
        }

        [Test, Order(4)]
        public void Db_Repository_DeleteApplication()
        {
            this.repository.Delete(this.addApplication);
            int result = this.context.SaveChanges();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void Db_Repository_DeleteById() 
        {
            ErrorReportApplication application = new ErrorReportApplication
            {
                Name = "TestApplicationForDeletion", 
                Version = "1.0.0.0", 
                FirstRunDate = DateTime.Now
            };

            this.repository.Insert(application);
            this.context.SaveChanges();

            this.repository.Delete(application.Id);
            this.context.SaveChanges();

            ErrorReportApplication deletedApplication = this.repository.GetByID(application.Id);

            Assert.IsNull(deletedApplication);
        }

        [Test]
        public void Db_Repository_GetApplications_VersionFiltered()
        {
            var applications = this.repository.Get(app => app.Version == "a.a.a.b");

            Assert.AreEqual(3, applications.Count());
        }

        [Test]
        public void Db_Repository_GetApplications_Ordered()
        {
            var applications = this.repository.Get(orderBy: q => q.OrderByDescending(a => a.FirstRunDate));

            Assert.GreaterOrEqual(applications.Count(), 5);
            Assert.AreEqual("TestApplicationForVersion", applications.First().Name);
            Assert.AreEqual("TestApplicationWithVersion3", applications.Last().Name);
            
        }

        [Test]
        public void Db_Repository_GetApplications_FilteredAndOrdered()
        {
            var applications = this.repository.Get(filter: app => app.FirstRunDate.Year < 2012,
                                                   orderBy: q => q.OrderByDescending(a => a.FirstRunDate));

            Assert.AreEqual(3, applications.Count());
            Assert.AreEqual("TestApplicationWithVersion1", applications.First().Name);
            Assert.AreEqual("TestApplicationWithVersion3", applications.Last().Name);    
        }

        [Test]
        public void Db_Repository_GetWithRawSql()
        {
            var param = new SqlParameter("version", "a.a.a.a");
            var applications = this.repository.GetWithRawSql("SELECT * FROM [dbo].[Applications] WHERE [Applications].[Version] = @version;", param);

            Assert.AreEqual(2, applications.Count());
        }
    }
}
