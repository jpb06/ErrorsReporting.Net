﻿using ErrorReporting.Dal.Tests.Data.Database.PrimitiveHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Tests.Data.Database
{
    public class PersistentErrorsReportingDataSet : IDisposable
    {
        private SqlConnection connection;
        private ApplicationsSqlHelper applications;
        private ExceptionsSqlHelper exceptions;

        public List<int> ApplicationsIds { get; private set; }
        public List<int> ExceptionsIds { get; private set; }

        public PersistentErrorsReportingDataSet()
        {
            this.ApplicationsIds = new List<int>();
            this.ExceptionsIds = new List<int>();

            this.connection = new SqlConnection(DatabaseConfiguration.ErrorsReportingConnectionString);
            this.connection.Open();

            this.applications = new ApplicationsSqlHelper(this.connection);
            this.exceptions = new ExceptionsSqlHelper(this.connection);
        }

        public void Initialize()
        {
            this.ApplicationsIds.Add(this.applications.Create("TestApplicationAlreadyExisting", "1.0.0.0", new DateTime(2000, 1, 1)));
            this.ApplicationsIds.Add(this.applications.Create("TestApplicationForVersion", "1.0.0.0", new DateTime(2100, 1, 1)));
            this.ApplicationsIds.Add(this.applications.Create("TestApplicationWithVersion1", "1.1.1.1", new DateTime(2011, 1, 1)));
            this.ApplicationsIds.Add(this.applications.Create("TestApplicationWithVersion2", "1.1.1.1", new DateTime(2012, 1, 1)));
            this.ApplicationsIds.Add(this.applications.Create("TestApplicationWithVersion3", "1.1.1.1", new DateTime(1990, 1, 1)));
        }

        public void Destroy()
        {
            foreach (var id in this.ExceptionsIds)
                this.exceptions.Delete(id);

            foreach (var id in this.ApplicationsIds)
                this.applications.Delete(id);
        }

        public void Dispose()
        {
            this.connection.Close();
            this.connection.Dispose();
        }
    }

}
