using ErrorReporting.Dal.Context.Contracts;
using ErrorReporting.Dal.Manipulation.Repositories;
using ErrorReporting.Dal.Manipulation.Repositories.Contracts;
using ErrorReporting.Dal.Models;
using ErrorReporting.Shared.Tests.Data.Mocked;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Tests.UnitTests.Manipulation.Repositories
{
    [TestFixture]
    public class MockGenericRepositoryTest
    {
        private GenericRepository<ErrorReportApplication> repository;

        public MockGenericRepositoryTest() { }

        [Test]
        public void AddApplication()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Insert(It.IsAny<ErrorReportApplication>()))
                                     .Callback<ErrorReportApplication>((app) =>
                                     {
                                         app.Id = 5;
                                         app.RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 };
                                         store.Applications.Add(app);
                                     }).Verifiable();
            this.repository = mockApplicationRepository.Object;

            ErrorReportApplication application = new ErrorReportApplication
            {
                Name = "Application 3",
                Version = "1.0.0.0", 
                FirstRunDate = new DateTime(2017, 1, 1)
            };

            this.repository.Insert(application);

            mockApplicationRepository.Verify(r => r.Insert(It.IsAny<ErrorReportApplication>()), Times.Once());
            Assert.AreEqual(5, store.Applications.Count);
            Assert.AreEqual(5, store.Applications.Last().Id);
        }

        [Test]
        public void UpdateApplication()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Update(It.IsAny<ErrorReportApplication>()))
                                     .Callback<ErrorReportApplication>((app) =>
                                     {
                                         var index = store.Applications.FindIndex(el => el.Id == app.Id);
                                         store.Applications[index] = app;
                                     }).Verifiable();
            this.repository = mockApplicationRepository.Object;

            ErrorReportApplication application = store.Applications.ElementAt(2);
            string newName = application.Name = "Application 4";
            string newVersion = application.Version = "1.0.0.0";

            this.repository.Update(application);

            mockApplicationRepository.Verify(r => r.Update(It.IsAny<ErrorReportApplication>()), Times.Once());
            Assert.AreEqual(newName, store.Applications.ElementAt(2).Name);
            Assert.AreEqual(newVersion, store.Applications.ElementAt(2).Version);

        }

        [Test]
        public void DeleteApplication()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Delete(It.IsAny<ErrorReportApplication>()))
                                     .Callback((ErrorReportApplication app) =>
                                     {
                                         store.Applications.Remove(app);
                                     }).Verifiable();
            this.repository = mockApplicationRepository.Object;

            ErrorReportApplication application = store.Applications.ElementAt(3);

            this.repository.Delete(application);

            mockApplicationRepository.Verify(r => r.Delete(It.IsAny<ErrorReportApplication>()), Times.Once());
            Assert.AreEqual(3, store.Applications.Count);
        }

        [Test]
        public void DeleteApplicationById()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Delete(It.IsAny<int>()))
                                     .Callback((object id) =>
                                     {
                                         var index = store.Applications.FindIndex(el => el.Id == (int)id);
                                         store.Applications.RemoveAt(index);
                                     }).Verifiable();
            this.repository = mockApplicationRepository.Object;

            this.repository.Delete(1);

            mockApplicationRepository.Verify(r => r.Delete(It.IsAny<int>()), Times.Once());
            Assert.AreEqual(3, store.Applications.Count);
        }

        [Test]
        public void GetApplicationById()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.GetByID(It.IsInRange<int>(1, 4, Range.Inclusive)))
                                     .Returns<int>(id => store.Applications.Find(el => el.Id == id));
            this.repository = mockApplicationRepository.Object;

            ErrorReportApplication application = this.repository.GetByID(1);
            ErrorReportApplication storedApplication = store.Applications.Single(el => el.Id == 1);

            Assert.AreEqual(application.Name, storedApplication.Name);
            Assert.AreEqual(application.Version, storedApplication.Version);
            Assert.AreEqual(application.FirstRunDate, storedApplication.FirstRunDate);
        }

        [Test]
        public void GetApplicationById_DoesntExist()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.GetByID(It.IsNotIn<int>(1, 2, 3, 4)))
                                     .Returns<ErrorReportApplication>(null);
            this.repository = mockApplicationRepository.Object;

            ErrorReportApplication application = this.repository.GetByID(10);

            Assert.AreEqual(null, application);
        }

        [Test]
        public void GetApplications_VersionFiltered()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Get(It.IsAny<Expression<Func<ErrorReportApplication, bool>>>(), null, string.Empty))
                                     .Returns((Expression<Func<ErrorReportApplication, bool>> filter,
                                               Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>> orderBy,
                                               string includeProperties) => store.Applications.Where(filter.Compile()));
            this.repository = mockApplicationRepository.Object;

            var result = this.repository.Get(app => app.Version == "1.0.0.0");

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void GetApplications_MinimumIdFiltered()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Get(It.IsAny<Expression<Func<ErrorReportApplication, bool>>>(), null, string.Empty))
                                     .Returns((Expression<Func<ErrorReportApplication, bool>> filter,
                                               Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>> orderBy,
                                               string includeProperties) => store.Applications.Where(filter.Compile()));
            this.repository = mockApplicationRepository.Object;

            var result = this.repository.Get(filter: app => app.Id > 2);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetApplications_Ordered()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Get(null, It.IsAny<Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>>>(), string.Empty))
                                     .Returns((Expression<Func<ErrorReportApplication, bool>> filter,
                                               Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>> orderBy,
                                               string includeProperties) => orderBy.Invoke(store.Applications.AsQueryable()));
            this.repository = mockApplicationRepository.Object;

            var result = this.repository.Get(orderBy: q => q.OrderByDescending(a => a.Id));

            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(1, result.Last().Id);
            Assert.AreEqual(4, result.First().Id);
        }

        [Test]
        public void GetApplications_IdFilteredAndOrdered()
        {
            VolatileDataset store = new VolatileDataset();
            Mock<IErrorsReportingContext> context = new Mock<IErrorsReportingContext>();
            Mock<GenericRepository<ErrorReportApplication>> mockApplicationRepository = new Mock<GenericRepository<ErrorReportApplication>>(context.Object);

            mockApplicationRepository.Setup(r => r.Get(It.IsAny<Expression<Func<ErrorReportApplication, bool>>>(),
                                                   It.IsAny<Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>>>(),
                                                   string.Empty))
                                 .Returns((Expression<Func<ErrorReportApplication, bool>> filter,
                                           Func<IQueryable<ErrorReportApplication>, IOrderedQueryable<ErrorReportApplication>> orderBy,
                                           string includeProperties) => orderBy.Invoke(store.Applications.Where(filter.Compile()).AsQueryable()));
            this.repository = mockApplicationRepository.Object;

            var result = this.repository.Get(filter: a => a.Id > 2,
                                             orderBy: q => q.OrderByDescending(a => a.Id));

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.Last().Id);
            Assert.AreEqual(4, result.First().Id);
        }
    }
}
