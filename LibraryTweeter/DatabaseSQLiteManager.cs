using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter
{
    public class DatabaseSQLiteManager
    {
        private string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionStringDB"];

        public void InsertOrDelete(string sqlInsertOrDelete)
        {
            SQLiteConnection m_dbConnection = null;

            try
            {
                m_dbConnection = new SQLiteConnection(connectionString);
                m_dbConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlInsertOrDelete, m_dbConnection);
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
            SQLiteConnection m_dbConnection = null;

            try
            {
                m_dbConnection = new SQLiteConnection(connectionString);
                m_dbConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlSelect, m_dbConnection);
                SQLiteDataReader dataReader = command.ExecuteReader();

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
