using System;
using System.Data;
using System.Data.SqlClient;

namespace LibraryTweeter
{
    public class DatabaseSQLServerManager
    {
        private string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionStringDB"];

        public void InsertOrDelete(string sqlInsertOrDelete)
        {
            SqlConnection m_dbConnection = null;

            try
            {
                m_dbConnection = new SqlConnection(connectionString);
                m_dbConnection.Open();

                SqlCommand command = new SqlCommand(sqlInsertOrDelete, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (m_dbConnection != null)
                    {
                        m_dbConnection.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public DataTable Select(string sqlSelect)
        {
            SqlConnection m_dbConnection = null;

            try
            {
                m_dbConnection = new SqlConnection(connectionString);
                m_dbConnection.Open();

                SqlCommand command = new SqlCommand(sqlSelect, m_dbConnection);
                SqlDataReader dataReader = command.ExecuteReader();

                DataTable tableToReturn = new DataTable();
                tableToReturn.Load(dataReader);

                return tableToReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    if (m_dbConnection != null)
                    {
                        m_dbConnection.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
