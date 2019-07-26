using System;
using System.Data.SQLite;
using System.Data;

namespace TelegramBot
{
    class SQLiteHelper
    {
        public static void RegisterUser(long userId)
        {

            using (SQLiteConnection sqlCon = new SQLiteConnection(String.Format("Data Source={0};Version=3;FailIfMissing=True;",
                                                    GlobalData.BuildDir + GlobalData.UserDbFilename)))
            {
                InsertNewUserCommand(sqlCon, userId);
            }
        }

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
            catch (SQLiteException)
            {
                Console.WriteLine("Couldn't insert new user to DB.");
            }
        }
    }
}
