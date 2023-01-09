using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TRMDataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess : IDisposable
    {
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                List<T> rows = cnn.Query<T>(storedProcedure,parameters,
                    commandType: CommandType.StoredProcedure ).ToList();
                return rows;
            }
        }
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                 cnn.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
        
        // Open Connection/ start transation 
        private IDbConnection _cnn;
        private IDbTransaction _tranaction;
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _cnn = new SqlConnection(connectionString);
            _cnn.Open();
            _tranaction = _cnn.BeginTransaction();

        }

        //Load using transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            
                List<T> rows = _cnn.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _tranaction).ToList();
                return rows;
            
        }


        //Save using transaction
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
           
                _cnn.Execute(storedProcedure, parameters,
                   commandType: CommandType.StoredProcedure, transaction: _tranaction);
           
        }
        //Close Connection / stop transation
        public void CommitTransaction()
        {
            _tranaction?.Commit();
            _cnn?.Close();
        }

        public void RollbackTransaction() 
        {
            _tranaction?.Rollback();
            _cnn?.Close();
        }

        public void Dispose()
        {
            CommitTransaction();
        }
        //Dispose
    }
}
