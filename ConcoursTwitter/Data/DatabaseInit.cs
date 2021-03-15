using ConcoursTwitter.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcoursTwitter.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseInit
    {
        /// <summary>
        /// Creates the tables.
        /// </summary>
        /// <param name="needDeleteBefore">for reset mode, need delete before</param>
        public static void CreateTables(bool needDeleteBefore)
        {
            if (needDeleteBefore)
            {
                DeleteTable();
            }

            DatabaseSQLServerManagerTools dbManager = new DatabaseSQLServerManagerTools();

            string queryCreateDatabase = "if not exists (select * from sysobjects where name='T_Citations' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_Citations](" +
                    "	[IdCitation] [int] IDENTITY(1,1) NOT NULL," +
                    "	[Text] [nvarchar](max) NULL," +
                    "	[IdTwitterFriend] [bigint] NULL," +
                    "	[Date] [date] NULL," +
                    "	[IdTweet] [bigint] NULL," +
                    " CONSTRAINT [PK_Citations] PRIMARY KEY CLUSTERED " +
                    "(" +
                    "	[IdCitation] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]" +
                    "" +
                    "if not exists (select * from sysobjects where name='T_Friends' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_Friends](" +
                    "	[IdFriend] [int] IDENTITY(1,1) NOT NULL," +
                    "	[DateAdded] [date] NULL," +
                    "	[IdTwitterFriend] [bigint] NULL," +
                    "	[Name] [nvarchar](50) NULL," +
                    " CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED " +
                    "(" +
                    "	[IdFriend] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY]" +
                    "" +
                    "if not exists (select * from sysobjects where name='T_FriendsAdded' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_FriendsAdded](" +
                    "	[IdFriendsAdded] [int] IDENTITY(1,1) NOT NULL," +
                    "	[DateAdded] [date] NULL," +
                    "	[IdTwitterFriend] [bigint] NULL," +
                    " CONSTRAINT [PK_FriendsAdded] PRIMARY KEY CLUSTERED " +
                    "(" +
                    "	[IdFriendsAdded] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY]" +
                    "" +
                    "if not exists (select * from sysobjects where name='T_LastIdSeen' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_LastIdSeen](" +
                    "	[IdOfLastTweetSeen] [bigint] NOT NULL," +
                    "	[IdTwitterFriend] [bigint] NOT NULL" +
                    ") ON [PRIMARY]" +
                    "" +
                    "if not exists (select * from sysobjects where name='T_Messages' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_Messages](" +
                    "	[IdMessage] [int] IDENTITY(1,1) NOT NULL," +
                    "	[IdMessageTweeter] [bigint] NOT NULL," +
                    "	[Text] [nvarchar](max) NULL," +
                    "	[IdTwitterFriend] [bigint] NOT NULL," +
                    "	[Date] [date] NULL," +
                    "	[Response] [nvarchar](max) NULL," +
                    " CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED " +
                    "(" +
                    "	[IdMessage] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]" +
                    "" +
                    "if not exists (select * from sysobjects where name='T_Retweets' and xtype='U')" +
                    "CREATE TABLE [dbo].[T_Retweets](" +
                    "	[IdTweet] [bigint] NULL," +
                    "	[DateInserted] [date] NULL," +
                    "	[IdTwitterFriend] [bigint] NULL" +
                    ") ON [PRIMARY]";

            dbManager.InsertOrDelete(queryCreateDatabase);
        }

        private static void DeleteTable()
        {
            DatabaseSQLServerManagerTools dbManager = new DatabaseSQLServerManagerTools();

            string queryDropTable = "if exists (select * from sysobjects where name='T_Citations' and xtype='U')" +
                "DROP TABLE T_Citations; " +
                "" +
                "if exists (select * from sysobjects where name='T_Friends' and xtype='U')" +
                "DROP TABLE T_Friends; " +
                "" +
                "if exists (select * from sysobjects where name='T_FriendsAdded' and xtype='U')" +
                "DROP TABLE T_FriendsAdded; " +
                "" +
                "if exists (select * from sysobjects where name='T_LastIdSeen' and xtype='U')" +
                "DROP TABLE T_LastIdSeen; " +
                "" +
                "if exists (select * from sysobjects where name='T_Messages' and xtype='U')" +
                "DROP TABLE T_Messages; " +
                "" +
                "if exists (select * from sysobjects where name='T_Retweets' and xtype='U')" +
                "DROP TABLE T_Retweets; ";

            dbManager.InsertOrDelete(queryDropTable);
        }


    }
}
