using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;

namespace TelegramBot
{
    class SQLiteHelper
    {
        private static SQLiteConnection sqlCon = new SQLiteConnection(String.Format("Data Source={0};Version=3;FailIfMissing=True;",
                                                    GlobalData.BuildDir + GlobalData.UserDbFilename));

        // public method to get currently registered users
        public static Dictionary<long,string> GetRegisteredUsers()
        {
            Dictionary<long, string> usersDict = new Dictionary<long, string>();
            DataSet usersDataSet = new DataSet();

            /*using (SQLiteConnection sQLiteCon = new SQLiteConnection(String.Format("Data Source={0};Version=3;FailIfMissing=True;",
                                                    GlobalData.BuildDir + GlobalData.UserDbFilename)))*/
            using (SQLiteConnection sQLiteCon = new SQLiteConnection("Data Source=C:/Users/step/source/repos/TelegramBotExamples/users.db;Version=3;FailIfMissing=True;"))
            {
                try
                {
                    string commandText = "SELECT * FROM User";
                    SQLiteDataAdapter SQLiteAdap = new SQLiteDataAdapter(commandText, sQLiteCon);
                    SQLiteAdap.Fill(usersDataSet, "User");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error " + ex.ErrorCode + ": " + ex.Message);
                }
            }

            if (usersDataSet.Tables["User"].Rows != null)
            {
                foreach (DataRow dataRow in usersDataSet.Tables["User"].Rows)
                {
                    try
                    {
                        usersDict.Add(long.Parse(dataRow["userId"].ToString()), dataRow["timeStamp"].ToString());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Couldn't convert ID to desired format.");
                    }
                }
            }            

            return usersDict;
        }

        // public method to register new users
        public static void RegisterUser(long userId)
        {

            using (SQLiteConnection sqlCon = new SQLiteConnection(String.Format("Data Source={0};Version=3;FailIfMissing=True;",
                                                    GlobalData.BuildDir + GlobalData.UserDbFilename)))
            {
                InsertNewUserCommand(sqlCon, userId);
            }
        }

        // private method to insert users in DB
        private static void InsertNewUserCommand(SQLiteConnection sQLiteCon, long userId)
        {
            DateTime currentDateTime = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));

            SQLiteParameter idParam = new SQLiteParameter("@idParam", DbType.Double),
                        timestampParam = new SQLiteParameter("@timestampParam", DbType.String);

            SQLiteCommand insertCmd = new SQLiteCommand("INSERT INTO User (userId, timeStamp) " +
                "VALUES (@idParam,@timestampParam)", sQLiteCon);

            idParam.Value = userId;
            timestampParam.Value = currentDateTime;

            insertCmd.Parameters.Add(idParam);
            insertCmd.Parameters.Add(timestampParam);

            try
            {
                if (sQLiteCon.State == ConnectionState.Closed)
                {
                    sQLiteCon.Open();
                }
                insertCmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Couldn't insert new user to DB.");
                Console.WriteLine("Error " + ex.ErrorCode + ": " + ex.Message);
            }
        }
    }
}
