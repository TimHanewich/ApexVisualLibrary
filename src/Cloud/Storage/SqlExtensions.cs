using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Codemasters.F1_2020;
using ApexVisual.Cloud.DeveloperMessaging;
using ApexVisual.Cloud.User;
using ApexVisual.Cloud.ActivityLogging;
using ApexVisual.SessionManagement;

namespace ApexVisual.Cloud.Storage
{
    public static class SqlExtensions
    {

        #region "User operations"

        public static async Task<bool> UserAccountExistsAsync(this ApexVisualManager avm, string username)
        {
            string cmd = "select Username from UserAccount where Username='" + username + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            bool ToReturn = dr.HasRows;

            sqlcon.Close();

            return ToReturn;
        }

        public static async Task<ApexVisualUserAccount> DownloadUserAccountAsync(this ApexVisualManager avm, string username)
        {
            string cmd = "select Id, Password, Email, AccountCreatedAt, PhotoBlobId from UserAccount where Username='" + username + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            if (dr.HasRows == false)
            {
                throw new Exception("Unable to find user account record with username '" + username + "'");
            }

            await dr.ReadAsync();

            //Prepare the return assets
            ApexVisualUserAccount ToReturn = new ApexVisualUserAccount();

            //Id
            if (dr.IsDBNull(0) == false)
            {
                ToReturn.Id = dr.GetGuid(0);
            }

            //Usernmae
            ToReturn.Username = username;

            //Password
            if (dr.IsDBNull(1) == false)
            {
                ToReturn.Password = dr.GetString(1);
            }

            //Email
            if (dr.IsDBNull(2) == false)
            {
                ToReturn.Email = dr.GetString(2);
            }

            //AccountCreatedAt
            if (dr.IsDBNull(3) == false)
            {
                ToReturn.AccountCreatedAt = dr.GetDateTime(3);
            }

            //Photoblobid
            if (dr.IsDBNull(4) == false)
            {
                ToReturn.PhotoBlobId = dr.GetString(4);
            }

            return ToReturn;
        }

        public static async Task UploadUserAccountAsync(this ApexVisualManager avm, ApexVisualUserAccount useraccount)
        {
            //Error check
            if (useraccount.Username == null || useraccount.Username == "")
            {
                throw new Exception("Unable to upload user account: Username was null or blank.");
            }
            if (useraccount.Password == null || useraccount.Password == "")
            {
                throw new Exception("Unable to upload user account: password was null or blank.");
            }
            if (useraccount.Email == null || useraccount.Email == "")
            {
                throw new Exception("Unable to upload user account: email was null or blank.");
            }
            
            //Prepare the KVP's for this record insert/update
            List<KeyValuePair<string, string>> ColumnValuePairs = new List<KeyValuePair<string, string>>();
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Id", "'" + useraccount.Id + "'"));
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Username", "'" + useraccount.Username + "'"));
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Password", "'" + useraccount.Password + "'"));
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Email", "'" + useraccount.Email + "'"));
            ColumnValuePairs.Add(new KeyValuePair<string, string>("AccountCreatedAt", "'" + useraccount.AccountCreatedAt.Year.ToString("0000") + "-" + useraccount.AccountCreatedAt.Month.ToString("00") + "-" + useraccount.AccountCreatedAt.Day.ToString("00") + " " + useraccount.AccountCreatedAt.Hour.ToString("00") + ":" + useraccount.AccountCreatedAt.Minute.ToString("00") + "." + useraccount.AccountCreatedAt.Second.ToString() + "'"));
            if (useraccount.PhotoBlobId != null && useraccount.PhotoBlobId != "")
            {
                ColumnValuePairs.Add(new KeyValuePair<string, string>("PhotoBlobId", "'" + useraccount.PhotoBlobId + "'"));
            }

            //Get the appropriate cmd to send
            string cmd = "";
            bool AlreadyExists = await avm.UserAccountExistsAsync(useraccount.Username);
            if (AlreadyExists == false) //It is a new account
            {
                //Prepare the command string
                string Component_Columns = "";
                string Component_Values = "";
                foreach (KeyValuePair<string, string> kvp in ColumnValuePairs)
                {
                    Component_Columns = Component_Columns + kvp.Key + ",";
                    Component_Values = Component_Values + kvp.Value + ",";
                }
                Component_Columns = Component_Columns.Substring(0, Component_Columns.Length-1); //Remove the last comma
                Component_Values = Component_Values.Substring(0, Component_Values.Length - 1);//Remove the last comma
                cmd = "insert into UserAccount (" + Component_Columns + ") values (" + Component_Values + ")"; 
            }
            else
            {
                string setter_portion = "";
                foreach (KeyValuePair<string, string> kvp in ColumnValuePairs)
                {
                    setter_portion = setter_portion + kvp.Key + " = " + kvp.Value + ",";
                }
                setter_portion = setter_portion.Substring(0, setter_portion.Length - 1);
                cmd = "update UserAccount set " + setter_portion + " where " + "Username='" + useraccount.Username + "'";
            }

            //Send the command
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            await sqlcmd.ExecuteNonQueryAsync();
            sqlcon.Close();
        }

        public static async Task<string[]> ListOwnedSessionsAsync(this ApexVisualManager avm, string username)
        {
            string cmd = "select SessionId from Session where Owner='" + username + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            
            List<string> ToReturn = new List<string>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ToReturn.Add(dr.GetString(0));
                }
            }

            sqlcon.Close();

            return ToReturn.ToArray();
        }

        public static async Task<int> CountNewlyRegisteredUsersAsync(this ApexVisualManager avm, DateTime date)
        {
            string cmd = "select count(Username) from UserAccount where " + GetTimeStampDayFilter("AccountCreatedAt", date);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            dr.Read();
            int ToReturn = dr.GetInt32(0);
            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<string[]> ListNewlyRegisteredUsersAsync(this ApexVisualManager avm, DateTime date)
        {
            string cmd = "select username from UserAccount where " + GetTimeStampDayFilter("AccountCreatedAt", date);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<string> ToReturn = new List<string>();
            while (dr.Read())
            {
                if (dr.IsDBNull(0) == false)
                {
                    ToReturn.Add(dr.GetString(0));
                }
            }
            return ToReturn.ToArray();
        }

        #endregion

        #region "Activity log operations"

        public async static Task<Guid> UploadActivityLogAsync(this ApexVisualManager avm, ActivityLog log)
        {
            Guid ToReturn = Guid.NewGuid();

            List<KeyValuePair<string, string>> ColumnValuePairs = new List<KeyValuePair<string, string>>();

            //This unique id (primary key)
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Id", "'" + ToReturn.ToString() + "'"));

            //Session id
            if (log.SessionId == null)
            {
                log.SessionId = new Guid(); //Blank (000000, etc.)
            }
            ColumnValuePairs.Add(new KeyValuePair<string, string>("SessionId", "'" + log.SessionId.ToString() + "'"));
            
            //Username
            if (log.Username != null & log.Username != "")
            {
                ColumnValuePairs.Add(new KeyValuePair<string, string>("Username", "'" + log.Username + "'"));
            }

            //TimeStamp
            if (log.TimeStamp == null)
            {
                log.TimeStamp = DateTimeOffset.Now;
            }
            ColumnValuePairs.Add(new KeyValuePair<string, string>("TimeStamp", "'" + log.TimeStamp.Year.ToString("0000") + "-" + log.TimeStamp.Month.ToString("00") + "-" + log.TimeStamp.Day.ToString("00") + " " + log.TimeStamp.Hour.ToString("00") + ":" + log.TimeStamp.Minute.ToString("00") + ":" + log.TimeStamp.Second.ToString("00") + "." + log.TimeStamp.Millisecond.ToString() + "'"));

            //ApplicationId
            ColumnValuePairs.Add(new KeyValuePair<string, string>("ApplicationId", Convert.ToInt32(log.ApplicationId).ToString()));

            //ActivityId
            ColumnValuePairs.Add(new KeyValuePair<string, string>("ActivityId", Convert.ToInt32(log.ActivityId).ToString()));

            //Package versions
            if (log.PackageVersion != null)
            {
                ColumnValuePairs.Add(new KeyValuePair<string, string>("PackageVersionMajor", log.PackageVersion.Major.ToString()));
                ColumnValuePairs.Add(new KeyValuePair<string, string>("PackageVersionMinor", log.PackageVersion.Minor.ToString()));
                ColumnValuePairs.Add(new KeyValuePair<string, string>("PackageVersionBuild", log.PackageVersion.Build.ToString()));
                ColumnValuePairs.Add(new KeyValuePair<string, string>("PackageVersionRevision", log.PackageVersion.Revision.ToString()));
            }

            //Note
            if (log.Note != null && log.Note != "")
            {
                string NoteToWrite = "";
                if (log.Note.Length > 255)
                {
                    NoteToWrite = log.Note.Substring(0, 255);
                }
                else
                {
                    NoteToWrite = log.Note;
                }
                ColumnValuePairs.Add(new KeyValuePair<string, string>("Note", "'" + log.Note + "'"));
            }

            //Prepare the command string
            string Component_Columns = "";
            string Component_Values = "";
            foreach (KeyValuePair<string, string> kvp in ColumnValuePairs)
            {
                Component_Columns = Component_Columns + kvp.Key + ",";
                Component_Values = Component_Values + kvp.Value + ",";
            }
            Component_Columns = Component_Columns.Substring(0, Component_Columns.Length-1); //Remove the last comma
            Component_Values = Component_Values.Substring(0, Component_Values.Length - 1);//Remove the last comma

            string cmd = "insert into ActivityLog (" + Component_Columns + ") values (" + Component_Values + ")";
            SqlConnection sqlcon = GetSqlConnection(avm);
            await sqlcon.OpenAsync();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            await sqlcmd.ExecuteNonQueryAsync();
            sqlcon.Close();

            return ToReturn;
        }

        public async static Task<ActivityLog> DownloadActivityLogAsync(this ApexVisualManager avm, Guid id)
        {
            string cmd = "select SessionId,Username,TimeStamp,ApplicationId,ActivityId,PackageVersionMajor,PackageVersionMinor,PackageVersionBuild,PackageVersionRevision,Note from ActivityLog where Id='" + id.ToString() + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            if (dr.HasRows == false)
            {
                throw new Exception("Activity log with ID '" + id.ToString() + "' does not exist.");
            }

            dr.Read();

            ActivityLog ToReturn = new ActivityLog();
            
            //SessionId
            if (dr.IsDBNull(0) == false)
            {
                ToReturn.SessionId = dr.GetGuid(0);
            }

            //Username
            if (dr.IsDBNull(1) == false)
            {
                ToReturn.Username = dr.GetString(1);
            }

            //Timestamp
            if (dr.IsDBNull(2) == false)
            {
                ToReturn.TimeStamp = dr.GetDateTime(2);
            }
            
            //ApplicationId
            if (dr.IsDBNull(3) == false)
            {
                ToReturn.ApplicationId = (ApplicationType)dr.GetByte(3);
            }

            //ActivityId
            if (dr.IsDBNull(4) == false)
            {
                ToReturn.ActivityId = (ActivityType)dr.GetInt32(4);
            }

            //Major
            if (dr.IsDBNull(5) == false)
            {
                if (ToReturn.PackageVersion == null)
                {
                    ToReturn.PackageVersion = new PackageVersion();
                }
                ToReturn.PackageVersion.Major = (int)dr.GetInt16(5);
            }

            //Minor
            if (dr.IsDBNull(6) == false)
            {
                if (ToReturn.PackageVersion == null)
                {
                    ToReturn.PackageVersion = new PackageVersion();
                }
                ToReturn.PackageVersion.Minor = (int)dr.GetInt16(6);
            }

            //Build
            if (dr.IsDBNull(7) == false)
            {
                if (ToReturn.PackageVersion == null)
                {
                    ToReturn.PackageVersion = new PackageVersion();
                }
                ToReturn.PackageVersion.Build = (int)dr.GetInt16(7);
            }

            //Revision
            if (dr.IsDBNull(8) == false)
            {
                if (ToReturn.PackageVersion == null)
                {
                    ToReturn.PackageVersion = new PackageVersion();
                }
                ToReturn.PackageVersion.Revision = (int)dr.GetInt16(8);
            }

            //Note
            if (dr.IsDBNull(9) == false)
            {
                ToReturn.Note = dr.GetString(9);
            }

            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<ActivityLog[]> DownloadActivityLogsBySessionAsync(this ApexVisualManager avm, Guid session_id)
        {
            string cmd = "select Username, TimeStamp, ApplicationId, ActivityId, PackageVersionMajor, PackageVersionMinor, PackageVersionBuild, PackageVersionRevision, Note from ActivityLog where SessionId='" + session_id + "' order by TimeStamp asc";
            SqlConnection sqlcon = GetSqlConnection(avm);
            await sqlcon.OpenAsync();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            List<ActivityLog> ToReturn = new List<ActivityLog>();
            while (dr.Read())
            {
                ActivityLog al = new ActivityLog();

                //If there is a username
                if (dr.IsDBNull(0) == false)
                {
                    al.Username = dr.GetString(0);
                }

                //if There is a timestamp
                if (dr.IsDBNull(1) == false)
                {
                    al.TimeStamp = dr.GetDateTime(1);
                }

                //ApplicationId
                al.ApplicationId = (ApplicationType)dr.GetByte(2);

                //ActivityId
                al.ActivityId = (ActivityType)dr.GetInt32(3);

                //Package major version
                if (dr.IsDBNull(4) == false)
                {
                    if (al.PackageVersion == null)
                    {
                        al.PackageVersion = new PackageVersion();
                    }
                    al.PackageVersion.Major = (int)dr.GetInt16(4);
                }

                //Package minor version
                if (dr.IsDBNull(5) == false)
                {
                    if (al.PackageVersion == null)
                    {
                        al.PackageVersion = new PackageVersion();
                    }
                    al.PackageVersion.Minor = (int)dr.GetInt16(5);
                }

                //Package build version
                if (dr.IsDBNull(6) == false)
                {
                    if (al.PackageVersion == null)
                    {
                        al.PackageVersion = new PackageVersion();
                    }
                    al.PackageVersion.Build = (int)dr.GetInt16(6);
                }

                //Package revision version
                if (dr.IsDBNull(7) == false)
                {
                    if (al.PackageVersion == null)
                    {
                        al.PackageVersion = new PackageVersion();
                    }
                    al.PackageVersion.Revision = (int)dr.GetInt16(7);
                }

                //Note
                if (dr.IsDBNull(8) == false)
                {
                    al.Note = dr.GetString(8);
                }

                ToReturn.Add(al);
            }

            sqlcon.Close();

            return ToReturn.ToArray();
        }

        public static async Task<int> CountActivityLogsAsync(this ApexVisualManager avm, DateTime date)
        {  
            string cmd = "select count(Id) from ActivityLog where " + GetTimeStampDayFilter("TimeStamp", date);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            int ToReturn = 0;
            while (dr.Read())
            {
                ToReturn = dr.GetInt32(0);
            }
            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<int> CountActivityLogsOfTypeAsync(this ApexVisualManager avm, DateTime date, ActivityType activity_type)
        {
            string cmd = "select count(Id) from ActivityLog where " + GetTimeStampDayFilter("TimeStamp", date) + " and ActivityId = " + Convert.ToInt32(activity_type).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            await dr.ReadAsync();
            int ToReturn = dr.GetInt32(0);
            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<int> CountDistinctActivitySessionsAsync(this ApexVisualManager avm, DateTime date, ApplicationType? app_type = null)
        {
            //Date
            string datetime_filter = GetTimeStampDayFilter("TimeStamp", date);

            //Application type filter
            string app_type_filter = "";
            if (app_type.HasValue)
            {
                app_type_filter = " and ApplicationId = " + Convert.ToString((int)app_type.Value);
            }

            string cmd = "select count(distinct SessionId) from ActivityLog where " + datetime_filter + app_type_filter;
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            int ToReturn = 0;
            while (dr.Read())
            {
                ToReturn = dr.GetInt32(0);
            }
            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<Guid[]> GetUniqueActivitySessionIdsAsync(this ApexVisualManager avm, DateTime date, ApplicationType? app_type = null)
        {
            //Date
            string datetime_filter = GetTimeStampDayFilter("TimeStamp", date);

            //Application type filter
            string app_type_filter = "";
            if (app_type.HasValue)
            {
                app_type_filter = " and ApplicationId = " + Convert.ToString((int)app_type.Value);
            }

            string cmd = "select distinct SessionId from ActivityLog where " + datetime_filter + app_type_filter;
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            
            List<Guid> ToReturn = new List<Guid>();
            while (dr.Read())
            {
                ToReturn.Add(dr.GetGuid(0));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }

        
        #endregion

        #region "Message Submission operations"

        public static async Task<Guid> UploadMessageSubmissionAsync(this ApexVisualManager avm, MessageSubmission msg, Guid? as_id = null)
        {
            //If an ID is supplied, use it. If not, just use a random one.
            Guid ToReturn = Guid.NewGuid();
            if (as_id != null)
            {
                ToReturn = as_id.Value;
            }

            List<KeyValuePair<string, string>> ColumnValuePairs = new List<KeyValuePair<string, string>>();

            //The ID
            ColumnValuePairs.Add(new KeyValuePair<string, string>("Id", "'" + ToReturn.ToString() + "'"));

            //Username
            if (msg.Username != null)
            {
                if (msg.Username != "")
                {
                    ColumnValuePairs.Add(new KeyValuePair<string, string>("Username", "'" + msg.Username + "'"));
                }
            }

            //Email
            if (msg.Email != null)
            {
                if (msg.Email != "")
                {
                    ColumnValuePairs.Add(new KeyValuePair<string, string>("Email", "'" + msg.Email + "'"));
                }
            }

            //Message Type
            ColumnValuePairs.Add(new KeyValuePair<string, string>("MessageType", Convert.ToString((int)msg.MessageType)));

            //Created At
            string cas = msg.CreatedAt.Year.ToString("0000") + "-" + msg.CreatedAt.Month.ToString("00") + "-" + msg.CreatedAt.Day.ToString("00") + " " + msg.CreatedAt.Hour.ToString("00") + ":" + msg.CreatedAt.Minute.ToString("00") + ":" + msg.CreatedAt.Second.ToString("00") + "." + msg.CreatedAt.Millisecond.ToString();
            ColumnValuePairs.Add(new KeyValuePair<string, string>("CreatedAt", "'" + cas + "'"));

            //Prepare the command string
            string Component_Columns = "";
            string Component_Values = "";
            foreach (KeyValuePair<string, string> kvp in ColumnValuePairs)
            {
                Component_Columns = Component_Columns + kvp.Key + ",";
                Component_Values = Component_Values + kvp.Value + ",";
            }
            Component_Columns = Component_Columns.Substring(0, Component_Columns.Length-1); //Remove the last comma
            Component_Values = Component_Values.Substring(0, Component_Values.Length - 1);//Remove the last comma

            //Make the command
            string cmd = "insert into MessageSubmission (" + Component_Columns + ") values (" + Component_Values + ")";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            await sqlcmd.ExecuteNonQueryAsync();
            sqlcon.Close();
            
            return ToReturn;
        }

        public static async Task<MessageSubmission> DownloadMessageSubmissionAsync(this ApexVisualManager avm, Guid id)
        {
            string cmd = "select Username, Email, MessageType, CreatedAt from MessageSubmission where Id='" + id.ToString() + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            if (dr.HasRows == false)
            {
                throw new Exception("Message submission with ID '" + id.ToString() + "' does not exist in SQL storage.");
            }

            await dr.ReadAsync();

            MessageSubmission ToReturn = new MessageSubmission();

            //Username
            if (dr.IsDBNull(0) == false)
            {
                ToReturn.Username = dr.GetString(0);
            }

            //Email
            if (dr.IsDBNull(1) == false)
            {
                ToReturn.Email = dr.GetString(1);
            }

            //MessageType
            if (dr.IsDBNull(2) == false)
            {
                ToReturn.MessageType = (MessageType)dr.GetByte(2);
            }

            //CreatedAt
            if (dr.IsDBNull(3) == false)
            {
                ToReturn.CreatedAt = dr.GetDateTime(3);
            }

            sqlcon.Close();

            return ToReturn;
        }

        public static async Task<int> CountMessageSubmissionsFromDateAsync(this ApexVisualManager avm, DateTime date)
        {
            string cmd = "select count(Id) from MessageSubmission where " + GetTimeStampDayFilter("CreatedAt", date);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            await dr.ReadAsync();
            int ToReturn = dr.GetInt32(0);
            sqlcon.Close();
            return ToReturn;
        }

        public static async Task<Guid[]> ListMessageSubmissionIdsFromDateAsync(this ApexVisualManager avm, DateTime date)
        {
            string cmd = "select Id from MessageSubmission where " + GetTimeStampDayFilter("CreatedAt", date);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();

            List<Guid> ToReturn = new List<Guid>();
            while (dr.Read())
            {
                ToReturn.Add(dr.GetGuid(0));
            }

            sqlcon.Close();
            return ToReturn.ToArray();
        }

        #endregion
  
        #region "Helper functions"

        private static SqlConnection GetSqlConnection(this ApexVisualManager avm)
        {
            SqlConnection con = new SqlConnection(avm.AzureSqlDbConnectionString);
            return con;
        }

        private static string GetTimeStampDayFilter(string column_name, DateTime date)
        {
            string date_START = date.Year.ToString("0000") + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00");
            string date_END = (date.AddDays(1)).Year.ToString("0000") + "-" + (date.AddDays(1)).Month.ToString("00") + "-" + (date.AddDays(1)).Day.ToString("00");
            string ToReturn = column_name + " >= '" + date_START + "' and " + column_name + " < '" + date_END + "'";
            return ToReturn;
        }

        #endregion
    
    }
}