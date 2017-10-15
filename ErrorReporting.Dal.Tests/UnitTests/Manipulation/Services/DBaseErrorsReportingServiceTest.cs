using ErrorReporting.Dal.Context;
using ErrorReporting.Dal.Context.Contracts;
using ErrorReporting.Dal.Exceptions;
using ErrorReporting.Dal.Exceptions.CustomTypes;
using ErrorReporting.Dal.Manipulation.Repositories;
using ErrorReporting.Dal.Manipulation.Repositories.Contracts;
using ErrorReporting.Dal.Manipulation.Services;
using ErrorReporting.Dal.Manipulation.Services.Contracts;
using ErrorReporting.Dal.Models;
using ErrorReporting.Shared.Tests.Data.Database;
using ErrorReporting.Shared.Tests.Data.Database.PrimitiveHelpers;
using ErrorReporting.Shared.Tests.Exceptions;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Tests.UnitTests.Manipulation.Services
{
    [TestFixture]
    public class DBaseErrorsReportingServiceTest
    {
        private UnityContainer container;
        private PersistentErrorsReportingDataSet dataSet;
        private SqlConnection connection;
        private ExceptionsSqlHelper exceptionsSqlHelper;

        public DBaseErrorsReportingServiceTest()
        {
            this.container = new UnityContainer();

            this.container.RegisterType<IErrorsReportingContext, ErrorsReportingContext>(new HierarchicalLifetimeManager());
            this.container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            this.container.RegisterType<IErrorsReportingService, ErrorsReportingService>();

            this.dataSet = new PersistentErrorsReportingDataSet();

            this.connection = new SqlConnection(DatabaseConfiguration.ErrorsReportingConnectionString);
            this.exceptionsSqlHelper = new ExceptionsSqlHelper(this.connection);
        }

        [OneTimeSetUp]
        public void Init()
        {
            this.dataSet.Initialize();
            this.connection.Open();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            this.connection.Close();
            this.dataSet.Destroy();
            this.dataSet.Dispose();
        }

        [Test]
        public void Db_ErrorsReportingService_CreateApplication()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();
                ErrorReportApplication result = service.CreateApplication("TestApplication", "1.0.0.0");

                Assert.IsNotNull(result);
                Assert.Greater(result.Id, 0);

                this.dataSet.ApplicationsIds.Add(result.Id);
            }
        }

        [Test]
        public void Db_ErrorsReportingService_CreateApplication_AlreadyExists()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();

                DalException ex = Assert.Throws<DalException>(() =>
                {
                    service.CreateApplication("TestApplicationAlreadyExisting", "a.a.a.a");
                });
                Assert.That(ex.errorType, Is.EqualTo(DalErrorType.SqlUniqueConstraintViolation));
            }
        }

        [Test]
        public void Db_ErrorsReportingService_GetApplication()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();
                ErrorReportApplication application = service.GetApplication("TestApplicationAlreadyExisting", "a.a.a.a");

                Assert.Greater(application.Id, 0);
                Assert.AreEqual(new DateTime(2000, 1, 1), application.FirstRunDate);
            }
        }

        [Test]
        public void Db_ErrorsReportingService_GetApplication_NotExisting()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();
                ErrorReportApplication application = null;
                Assert.That(() =>
                {
                    application = service.GetApplication("TestApplicationAlreadyExisting", "1.1.0.0");
                }, Throws.Nothing);

                Assert.IsNull(application);
            }
        }

        [Test]
        public void Db_ErrorsReportingService_LogException()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();

                try
                {
                    ExceptionGenerator.ThrowsOne();
                }
                catch (Exception exception)
                {
                    int? id = null;
                    Assert.That(() =>
                    {
                        id = service.LogException(this.dataSet.ApplicationsIds.ElementAt(0), exception, "ErrorType.Specific");
                    }, Throws.Nothing);

                    Assert.IsNotNull(id);

                    ErrorReportException ex = this.exceptionsSqlHelper.Get(id.Value);

                    Assert.AreEqual("One", ex.Message);
                }
            }
        }

        [Test]
        public void Db_ErrorsReportingService_LogException_WithInner()
        {
            using (IUnityContainer childContainer = this.container.CreateChildContainer())
            {
                IErrorsReportingService service = childContainer.Resolve<IErrorsReportingService>();

                try
                {
                    ExceptionGenerator.ThrowsTwo();
                }
                catch (Exception exception)
                {
                    int? id = null;
                    Assert.That(() =>
                    {
                        id = service.LogException(this.dataSet.ApplicationsIds.ElementAt(0), exception, "ErrorType.Specific");
                    }, Throws.Nothing);

                    Assert.IsNotNull(id);

                    ErrorReportException ex = this.exceptionsSqlHelper.Get(id.Value);
                    ErrorReportException innerEx = this.exceptionsSqlHelper.Get(ex.IdInnerException.Value);

                    Assert.AreEqual("Two", ex.Message);
                    Assert.AreEqual("One", innerEx.Message);
                }
            }
        }
    }
}
