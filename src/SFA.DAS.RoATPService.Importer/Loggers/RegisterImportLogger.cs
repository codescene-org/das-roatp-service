namespace SFA.DAS.RoATPService.Importer.Loggers
{
    using System;
    using System.IO;
    using SFA.DAS.RoATPService.Importer.Models;

    public class RegisterImportLogger 
    {
        private static string _logFileName;
        private static FileStream _logFileStream;
        private static StreamWriter _logWriter;
        private static RegisterImportLogger instance = null;
        private static readonly object lockObject = new object();

        RegisterImportLogger()
        {
        }

        public static RegisterImportLogger Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new RegisterImportLogger();
                    }
                    return instance;
                }
            }
        }

        public string LogFileName
        {
            get { return _logFileName; }
            set
            {
                _logFileName = value;
                File.Delete(_logFileName);
                _logFileStream = File.Create(_logFileName);
                _logWriter = new StreamWriter(_logFileStream);
                _logWriter.AutoFlush = true;
            }
        }

        public bool LogEnabled
        {
            get { return !String.IsNullOrWhiteSpace(LogFileName); }
        }

        public void LogStatement(string sql)
        {
            if (!LogEnabled)
            {
                return;
            }

            _logWriter.WriteLine($"{sql}\n");
        }

        public void LogInsertStatement(string sql, RegisterEntry registerEntry, Guid organisationId, DateTime createdAt, string createdBy, string statusId, DateTime statusDate, string organisationData)
        {
            if (!LogEnabled)
            {
                return;
            }

            registerEntry.LegalName = registerEntry.LegalName.Replace("'", "''");
            registerEntry.TradingName = registerEntry.TradingName.Replace("'", "''");
            
            string formattedSql = $"exec sp_executesql N'{sql}',N'@organisationId uniqueidentifier,@createdAt datetime,@createdBy nvarchar(4000),@statusId nvarchar(4000),@providerTypeId int,@OrganisationTypeId int,@UKPRN bigint,@LegalName nvarchar(4000),@TradingName nvarchar(4000),@statusDate datetime,@organisationData nvarchar(4000)',@organisationId='{organisationId}',@createdAt='{createdAt.ToString("yyyyMMdd HH:mm:ss")}',@createdBy=N'{createdBy}',@statusId=N'{statusId}',@ProviderTypeId={registerEntry.ProviderTypeId},@OrganisationTypeId={registerEntry.OrganisationTypeId},@UKPRN={registerEntry.UKPRN},@LegalName=N'{registerEntry.LegalName}',@TradingName=N'{registerEntry.TradingName}',@statusDate='{statusDate.ToString("yyyyMMdd HH:mm:ss")}',@organisationData=N'{organisationData}'";

            _logWriter.WriteLine($"{formattedSql}\n");
        }

        public void Close()
        {
           
            _logFileStream.Flush();
            _logFileStream.Close();
        }
    }
}
