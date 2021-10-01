using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Codemasters.F1_2020;
using ApexVisual.Cloud.DeveloperMessaging;
using ApexVisual.Cloud.User;
using ApexVisual.Cloud.ActivityLogging;
using ApexVisual.SessionManagement;
using ApexVisual.SessionDocumentation;
using TimHanewich.SqlHelper;

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
                ToReturn.PhotoBlobId = dr.GetGuid(4);
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
            if (useraccount.PhotoBlobId != Guid.Empty)
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
            ColumnValuePairs.Add(new KeyValuePair<string, string>("SessionId", "'" + log.SessionId.ToString() + "'"));

            //User Id
            ColumnValuePairs.Add(new KeyValuePair<string, string>("ByUser", "'" + log.ByUser.ToString() + "'"));

            //TimeStamp
            if (log.TimeStamp.Year < 1900)
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
            string cmd = "select SessionId,ByUser,TimeStamp,ApplicationId,ActivityId,PackageVersionMajor,PackageVersionMinor,PackageVersionBuild,PackageVersionRevision,Note from ActivityLog where Id='" + id.ToString() + "'";
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

            //ByUser
            try
            {
                ToReturn.ByUser = dr.GetGuid(dr.GetOrdinal("ByUser"));
            }
            catch
            {
                
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
            string cmd = "select ByUser, TimeStamp, ApplicationId, ActivityId, PackageVersionMajor, PackageVersionMinor, PackageVersionBuild, PackageVersionRevision, Note from ActivityLog where SessionId='" + session_id + "' order by TimeStamp asc";
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
                    al.ByUser = dr.GetGuid(0);
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
  
        #region "Session v2"

        public static async Task UploadSessionAsync(this ApexVisualManager avm, Session s)
        {
            InsertHelper ih = new InsertHelper("Session");
            ih.Add("SessionId", ApexVisualToolkit.ULongToLong(s.SessionId).ToString()); //be sure to encode the session id from unsigned to signed.
            ih.Add("Owner", s.Owner.ToString(), true);
            ih.Add("Game", Convert.ToInt32(s.Game).ToString());
            ih.Add("Track", Convert.ToInt32(s.Track).ToString());
            ih.Add("Mode", Convert.ToInt32(s.Mode).ToString());
            ih.Add("Team", Convert.ToInt32(s.Team).ToString());
            ih.Add("Driver", Convert.ToInt32(s.Driver).ToString());
            ih.Add("CreatedAtUtc", s.CreatedAtUtc.ToString(), true);
            string cmd = ih.ToString();

            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            await sqlcmd.ExecuteNonQueryAsync();
            sqlcon.Close();
        }

        public static async Task<Session> DownloadSessionAsync(this ApexVisualManager avm, ulong id)
        {
            long ToSearchFor = ApexVisualToolkit.ULongToLong(id);
            string cmd = "select SessionId, Owner, Game, Track, Mode, Team, Driver, CreatedAtUtc from Session where SessionId = " + ToSearchFor.ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                await dr.ReadAsync();
                Session ToReturn = ExtractSessionFromSqlDataReader(dr);
                sqlcon.Close();
                return ToReturn;
            }
            else
            {
                sqlcon.Close();
                throw new Exception("Unable to find session with session id '" + id.ToString() + "'");
            }
        }

        private static Session ExtractSessionFromSqlDataReader(SqlDataReader dr)
        {
            Session ToReturn = new Session();

            //SessionId
            try
            {
                long SID = dr.GetInt64(dr.GetOrdinal("SessionId"));
                ToReturn.SessionId = ApexVisualToolkit.LongToUlong(SID);
            }
            catch
            {

            }

            //Owner
            try
            {
                ToReturn.Owner = dr.GetGuid(dr.GetOrdinal("Owner"));
            }
            catch
            {

            }

            //Game
            try
            {
                ToReturn.Game = (CodemastersF1Game)dr.GetByte(dr.GetOrdinal("Game"));
            }
            catch
            {

            }

            //Track
            try
            {
                ToReturn.Track = (ApexVisual.SessionManagement.Track)dr.GetByte(dr.GetOrdinal("Track"));
            }
            catch
            {

            }

            //Mode
            try
            {
                ToReturn.Mode = (SessionType)dr.GetByte(dr.GetOrdinal("Mode"));
            }
            catch
            {

            }

            //Team
            try
            {
                ToReturn.Team = (ApexVisual.SessionManagement.Team)dr.GetByte(dr.GetOrdinal("Team"));
            }
            catch
            {

            }

            //Driver
            try
            {
                ToReturn.Driver = (ApexVisual.SessionManagement.Driver)dr.GetByte(dr.GetOrdinal("Driver"));
            }
            catch
            {

            }

            //CreatedAtUtc
            try
            {
                ToReturn.CreatedAtUtc = dr.GetDateTime(dr.GetOrdinal("CreatedAtUtc"));
            }
            catch
            {

            }

            return ToReturn;
        }

        public static async Task<bool> SessionExistsAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select count(SessionId) from Session where SessionId = " + ApexVisualToolkit.ULongToLong(session_id).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            await dr.ReadAsync();
            int val = dr.GetInt32(0);
            sqlcon.Close();
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task DeleteSessionAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "delete from Session where SessionId = " + ApexVisualToolkit.ULongToLong(session_id);
            await ExecuteNonQueryAsync(avm, cmd);
        }



        public static async Task UploadOriginalCaptureAsync(this ApexVisualManager avm, OriginalCapture oc)
        {
            InsertHelper ih = new InsertHelper("OriginalCapture");
            ih.Add("SessionId", ApexVisualToolkit.ULongToLong(oc.SessionId).ToString());
            ih.Add("CapturedAtUtc", oc.CapturedAtUtc.ToString(), true);
            string cmd = ih.ToString();
            await ExecuteNonQueryAsync(avm, cmd);
        }

        public static async Task<bool> OriginalCaptureExistsAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select count(SessionId) from OriginalCapture where SessionId = " + ApexVisualToolkit.ULongToLong(session_id).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            await dr.ReadAsync();
            int val = dr.GetInt32(0);
            sqlcon.Close();
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<OriginalCapture> DownloadOriginalCaptureAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select CapturedAtUtc from OriginalCapture where SessionId = " + ApexVisualToolkit.ULongToLong(session_id);
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                await dr.ReadAsync();
                DateTime val = dr.GetDateTime(0);
                sqlcon.Close();
                OriginalCapture ToReturn = new OriginalCapture();
                ToReturn.SessionId = session_id;
                ToReturn.CapturedAtUtc = val;
                return ToReturn;
            }
            else
            {
                sqlcon.Close();
                throw new Exception("Unable to find OriginalCapture record with SessionId '" + session_id.ToString() + "'");
            }
        }



        public static async Task UploadLapAsync(this ApexVisualManager avm, Lap l)
        {
            InsertHelper ih = new InsertHelper("Lap");
            ih.Add("Id", l.Id.ToString(), true);
            ih.Add("FromSession", ApexVisualToolkit.ULongToLong(l.FromSession).ToString(), true);
            ih.Add("LapNumber", l.LapNumber.ToString());
            if (l.Sector1Time.HasValue)
            {
                ih.Add("Sector1Time", l.Sector1Time.ToString());
            }
            if (l.Sector2Time.HasValue)
            {
                ih.Add("Sector2Time", l.Sector2Time.ToString());
            }
            if (l.Sector3Time.HasValue)
            {
                ih.Add("Sector3Time", l.Sector3Time.ToString());
            }
            ih.Add("EndingFuel", l.EndingFuel.ToString());
            ih.Add("PercentOnThrottle", l.PercentOnThrottle.ToString());
            ih.Add("PercentOnBrake", l.PercentOnBrake.ToString());
            ih.Add("PercentCoasting", l.PercentCoasting.ToString());
            ih.Add("PercentOnMaxThrottle", l.PercentOnMaxThrottle.ToString());
            ih.Add("PercentOnMaxBrake", l.PercentOnMaxBrake.ToString());
            ih.Add("EndingErs", l.EndingErs.ToString());
            ih.Add("GearChanges", l.GearChanges.ToString());
            ih.Add("EquippedTyreCompound", Convert.ToInt32(l.EquippedTyreCompound).ToString());
            ih.Add("EndingTyreWear", l.EndingTyreWear.ToString(), true);
            await ExecuteNonQueryAsync(avm, ih.ToString());
        }

        public static async Task<Lap[]> DownloadLapsFromSessionAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select Id, FromSession, LapNumber, Sector1Time, Sector2Time, Sector3Time, EndingFuel, PercentOnThrottle, PercentOnBrake, PercentCoasting, PercentOnMaxThrottle, PercentOnMaxBrake, EndingErs, GearChanges, EquippedTyreCompound, EndingTyreWear from Lap where FromSession = " + ApexVisualToolkit.ULongToLong(session_id).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<Lap> ToReturn = new List<Lap>();
            while (dr.Read())
            {
                ToReturn.Add(ExtractLapFromSqlDataReader(dr));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }

        public static async Task<Lap> DownloadLapAsync(this ApexVisualManager avm, ulong session_id, byte lap_number)
        {
            string cmd = "select Id, FromSession, LapNumber, Sector1Time, Sector2Time, Sector3Time, EndingFuel, PercentOnThrottle, PercentOnBrake, PercentCoasting, PercentOnMaxThrottle, PercentOnMaxBrake, EndingErs, GearChanges, EquippedTyreCompound, EndingTyreWear from Lap where FromSession = " + ApexVisualToolkit.ULongToLong(session_id).ToString() + " and LapNumber = " + lap_number.ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            if (dr.HasRows == false)
            {
                sqlcon.Close();
                throw new Exception("Unable to find lap " + lap_number.ToString() + " for session " + session_id.ToString());
            }
            await dr.ReadAsync();
            Lap ToReturn = ExtractLapFromSqlDataReader(dr);
            return ToReturn;
        }

        public static async Task<byte[]> AvailableLapsAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select LapNumber from Lap where FromSession = " + ApexVisualToolkit.ULongToLong(session_id).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<byte> ToReturn = new List<byte>();
            while (dr.Read())
            {
                ToReturn.Add(dr.GetByte(0));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }

        public static async Task DeleteLapsAsync(this ApexVisualManager avm, ulong from_session_id)
        {
            await ExecuteNonQueryAsync(avm, "delete from Lap where FromSession = " + ApexVisualToolkit.ULongToLong(from_session_id).ToString());
        }

        private static Lap ExtractLapFromSqlDataReader(SqlDataReader dr)
        {
            Lap ToReturn = new Lap();

            //Id
            try
            {
                ToReturn.Id = dr.GetGuid(dr.GetOrdinal("Id"));
            }
            catch
            {
                
            }

            //FromSession
            try
            {
                ToReturn.FromSession = ApexVisualToolkit.LongToUlong(dr.GetInt64(dr.GetOrdinal("FromSession")));
            }
            catch
            {

            }

            //LapNumber
            try
            {
                ToReturn.LapNumber = dr.GetByte(dr.GetOrdinal("LapNumber"));
            }
            catch
            {

            }

            //Sector1Time
            try
            {
                ToReturn.Sector1Time = dr.GetFloat(dr.GetOrdinal("Sector1Time"));
            }
            catch
            {

            }

            //Sector2Time
            try
            {
                ToReturn.Sector2Time = dr.GetFloat(dr.GetOrdinal("Sector2Time"));
            }
            catch
            {

            }

            //Sector3Time
            try
            {
                ToReturn.Sector3Time = dr.GetFloat(dr.GetOrdinal("Sector3Time"));
            }
            catch
            {

            }

            //Ending Fuel
            try
            {
                ToReturn.EndingFuel = dr.GetFloat(dr.GetOrdinal("EndingFuel"));
            }
            catch
            {

            }

            //PercentOnThrottle
            try
            {
                ToReturn.PercentOnThrottle = dr.GetByte(dr.GetOrdinal("PercentOnThrottle"));
            }
            catch
            {

            }

            //PercentOnBrake
            try
            {
                ToReturn.PercentOnBrake = dr.GetByte(dr.GetOrdinal("PercentOnBrake"));
            }
            catch
            {

            }

            //PercentCoasting
            try
            {
                ToReturn.PercentCoasting = dr.GetByte(dr.GetOrdinal("PercentCoasting"));
            }
            catch
            {

            }

            //Percent on max throttle
            try
            {
                ToReturn.PercentOnMaxThrottle = dr.GetByte(dr.GetOrdinal("PercentOnMaxThrottle"));
            }
            catch
            {

            }

            //Percent on max brake
            try
            {
                ToReturn.PercentOnMaxBrake = dr.GetByte(dr.GetOrdinal("PercentOnMaxBrake"));
            }
            catch
            {

            }

            //Ending Ers
            try
            {
                ToReturn.EndingErs = dr.GetFloat(dr.GetOrdinal("EndingErs"));
            }
            catch
            {

            }

            //GearChanges
            try
            {
                ToReturn.GearChanges = dr.GetInt16(dr.GetOrdinal("GearChanges"));
            }
            catch
            {

            }

            //Equipped tyre compound
            try
            {
                ToReturn.EquippedTyreCompound = (ApexVisual.SessionManagement.TyreCompound)dr.GetByte(dr.GetOrdinal("EquippedTyreCompound"));
            }
            catch
            {

            }

            //Ending tyre wear
            try
            {
                ToReturn.EndingTyreWear = dr.GetGuid(dr.GetOrdinal("EndingTyreWear"));
            }
            catch
            {

            }

            return ToReturn;
        }



        public static async Task UploadTelemetrySnapshotAsync(this ApexVisualManager avm, TelemetrySnapshot ts)
        {
            InsertHelper ih = new InsertHelper("TelemetrySnapshot");
            ih.Add("Id", ts.Id.ToString(), true);
            ih.Add("FromLap", ts.FromLap.ToString(), true);
            ih.Add("SessionTime", ts.SessionTime.ToString());
            ih.Add("LocationType", Convert.ToInt32(ts.LocationType).ToString());
            ih.Add("LocationNumber", ts.LocationNumber.ToString());
            ih.Add("PositionX", ts.PositionX.ToString());
            ih.Add("PositionY", ts.PositionY.ToString());
            ih.Add("PositionZ", ts.PositionZ.ToString());
            ih.Add("CurrentLapTime", ts.CurrentLapTime.ToString());
            ih.Add("CarPosition", ts.CarPosition.ToString());
            ih.Add("LapInvalid", Convert.ToInt32(ts.LapInvalid).ToString());
            ih.Add("SpeedKph", ts.SpeedKph.ToString());
            ih.Add("Throttle", ts.Throttle.ToString());
            ih.Add("Steer", ts.Steer.ToString());
            ih.Add("Brake", ts.Brake.ToString());
            ih.Add("Gear", Convert.ToInt32(ts.Gear).ToString());
            ih.Add("DrsActive", Convert.ToInt32(ts.DrsActive).ToString());
            ih.Add("TyreWearPercent", ts.TyreWearPercent.ToString(), true);
            ih.Add("TyreDamagePercent", ts.TyreDamagePercent.ToString(), true);
            ih.Add("StoredErs", ts.StoredErs.ToString());
            await ExecuteNonQueryAsync(avm, ih.ToString());
        }

        public static async Task<TelemetrySnapshot[]> DownloadTelemetrySnapshotsAsync(this ApexVisualManager avm, Guid lap_id)
        {
            string cmd = "select Id, FromLap, SessionTime, LocationType, LocationNumber, PositionX, PositionY, PositionZ, CurrentLapTime, CarPosition, LapInvalid, SpeedKph, Throttle, Steer, Brake, Gear, DrsActive, TyreWearPercent, TyreDamagePercent, StoredErs from TelemetrySnapshot where FromLap = '" + lap_id.ToString() + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<TelemetrySnapshot> ToReturn = new List<TelemetrySnapshot>();
            while (dr.Read())
            {
                ToReturn.Add(ExtractTelemetrySnapshotFromSqlDataReader(dr));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }
        
        public static async Task<TelemetrySnapshot[]> DownloadTelemetrySnapshotsAsync(this ApexVisualManager avm, ulong session_id)
        {
            string cmd = "select TelemetrySnapshot.Id as Id, FromLap, SessionTime, LocationType, LocationNumber, PositionX, PositionY, PositionZ, CurrentLapTime, CarPosition, LapInvalid, SpeedKph, Throttle, Steer, Brake, Gear, DrsActive, TyreWearPercent, TyreDamagePercent, StoredErs from TelemetrySnapshot inner join Lap on TelemetrySnapshot.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(session_id).ToString();
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<TelemetrySnapshot> ToReturn = new List<TelemetrySnapshot>();
            while (dr.Read())
            {
                ToReturn.Add(ExtractTelemetrySnapshotFromSqlDataReader(dr));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }

        public static async Task DeleteTelemetrySnapshotsAsync(this ApexVisualManager avm, ulong from_session_id)
        {
            string cmd = "delete ts from TelemetrySnapshot as ts inner join Lap on ts.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session_id).ToString();
            await ExecuteNonQueryAsync(avm, cmd);
        }

        private static TelemetrySnapshot ExtractTelemetrySnapshotFromSqlDataReader(SqlDataReader dr)
        {
            TelemetrySnapshot ToReturn = new TelemetrySnapshot();

            //Id
            try
            {
                ToReturn.Id = dr.GetGuid(dr.GetOrdinal("Id"));
            }
            catch
            {
                
            }

            //From Lap
            try
            {
                ToReturn.FromLap = dr.GetGuid(dr.GetOrdinal("FromLap"));
            }
            catch
            {

            }

            //Session time
            try
            {
                ToReturn.SessionTime = dr.GetFloat(dr.GetOrdinal("SessionTime"));
            }
            catch
            {

            }

            //LocationType
            try
            {
                ToReturn.LocationType = (TrackLocationType)dr.GetByte(dr.GetOrdinal("LocationType"));
            }
            catch
            {

            }

            //Location number
            try
            {
                ToReturn.LocationNumber = dr.GetByte(dr.GetOrdinal("LocationNumber"));
            }
            catch
            {

            }
            
            //PositionX
            try
            {
                ToReturn.PositionX = dr.GetFloat(dr.GetOrdinal("PositionX"));
            }
            catch
            {

            }

            //PositionY
            try
            {
                ToReturn.PositionY = dr.GetFloat(dr.GetOrdinal("PositionY"));
            }
            catch
            {

            }

            //PositionZ
            try
            {
                ToReturn.PositionZ = dr.GetFloat(dr.GetOrdinal("PositionZ"));
            }
            catch
            {

            }

            //CurrentLapTime
            try
            {
                ToReturn.CurrentLapTime = dr.GetFloat(dr.GetOrdinal("CurrentLapTime"));
            }
            catch
            {

            }

            //CarPosition
            try
            {
                ToReturn.CarPosition = dr.GetByte(dr.GetOrdinal("CarPosition"));
            }
            catch
            {

            }

            //Lap Invalid
            try
            {
                ToReturn.LapInvalid = dr.GetBoolean(dr.GetOrdinal("LapInvalid"));
            }
            catch
            {

            }
            
            //SpeedKph
            try
            {
                ToReturn.SpeedKph = dr.GetInt16(dr.GetOrdinal("SpeedKph"));
            }
            catch
            {

            }

            //Throttle
            try
            {
                ToReturn.Throttle = dr.GetByte(dr.GetOrdinal("Throttle"));
            }
            catch
            {

            }

            //Steer
            try
            {
                ToReturn.Steer = dr.GetInt16(dr.GetOrdinal("Steer"));
            }
            catch
            {

            }

            //Brake
            try
            {
                ToReturn.Brake = dr.GetByte(dr.GetOrdinal("Brake"));
            }
            catch
            {

            }

            //Gear
            try
            {
                ToReturn.Gear = (Gear)dr.GetByte(dr.GetOrdinal("Gear"));
            }
            catch
            {

            }

            //DrsActive
            try
            {
                ToReturn.DrsActive = dr.GetBoolean(dr.GetOrdinal("DrsActive"));
            }
            catch
            {

            }

            //TyreWearPercent
            try
            {
                ToReturn.TyreWearPercent = dr.GetGuid(dr.GetOrdinal("TyreWearPercent"));
            }
            catch
            {

            }

            //Tyre damage percent
            try
            {
                ToReturn.TyreDamagePercent = dr.GetGuid(dr.GetOrdinal("TyreDamagePercent"));
            }
            catch
            {

            }

            //StoredErs
            try
            {
                ToReturn.StoredErs = dr.GetFloat(dr.GetOrdinal("StoredErs"));
            }
            catch
            {
                
            }

            return ToReturn;
        }




        public static async Task UploadWheelDataArrayAsync(this ApexVisualManager avm, ApexVisual.SessionDocumentation.WheelDataArray wda)
        {
            InsertHelper ih = new InsertHelper("WheelDataArray");
            ih.Add("Id", wda.Id.ToString(), true);
            ih.Add("RearLeft", wda.RearLeft.ToString());
            ih.Add("RearRight", wda.RearRight.ToString());
            ih.Add("FrontLeft", wda.FrontLeft.ToString());
            ih.Add("FrontRight", wda.FrontRight.ToString());
            await ExecuteNonQueryAsync(avm, ih.ToString());
        }

        public static async Task<ApexVisual.SessionDocumentation.WheelDataArray> DownloadWheelDataArrayAsync(this ApexVisualManager avm, Guid id)
        {
            string cmd = "select RearLeft, RearRight, FrontLeft, FrontRight from WheelDataArray where Id = '" + id.ToString() + "'";
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                await dr.ReadAsync();
                ApexVisual.SessionDocumentation.WheelDataArray ToReturn = ExtractWheelDataArrayFromSqlDataReader(dr);
                ToReturn.Id = id;
                sqlcon.Close();
                return ToReturn;
            }
            else
            {
                sqlcon.Close();
                throw new Exception("Unable to find WheelDataArray with Id '" + id.ToString() + "'");
            }
        }

        public static async Task<ApexVisual.SessionDocumentation.WheelDataArray[]> DownloadWheelDataArraysAsync(this ApexVisualManager avm, ulong from_session)
        {
            //Going to have to get it in two commands

            string cmd1 = "select WheelDataArray.Id as Id, RearLeft, RearRight, FrontLeft, FrontRight from WheelDataArray inner join Lap on WheelDataArray.Id = Lap.EndingTyreWear where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session).ToString();
            ApexVisual.SessionDocumentation.WheelDataArray[] batch1 = await DownloadWheelDataArraysFromCommandAsync(avm, cmd1);

            string cmd2 = "select WheelDataArray.Id as Id, RearLeft, RearRight, FrontLeft, FrontRight from WheelDataArray inner join TelemetrySnapshot on WheelDataArray.Id = TelemetrySnapshot.TyreWearPercent inner join Lap on TelemetrySnapshot.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session).ToString();
            ApexVisual.SessionDocumentation.WheelDataArray[] batch2 = await DownloadWheelDataArraysFromCommandAsync(avm, cmd2);

            string cmd3 = "select WheelDataArray.Id as Id, RearLeft, RearRight, FrontLeft, FrontRight from WheelDataArray inner join TelemetrySnapshot on WheelDataArray.Id = TelemetrySnapshot.TyreDamagePercent inner join Lap on TelemetrySnapshot.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session).ToString();
            ApexVisual.SessionDocumentation.WheelDataArray[] batch3 = await DownloadWheelDataArraysFromCommandAsync(avm, cmd3);

            //Assemble and return
            List<ApexVisual.SessionDocumentation.WheelDataArray> ToReturn = new List<SessionDocumentation.WheelDataArray>();
            ToReturn.AddRange(batch1);
            ToReturn.AddRange(batch2);
            ToReturn.AddRange(batch3);
            return ToReturn.ToArray();
        }

        public static async Task DeleteWheelDataArraysAsync(this ApexVisualManager avm, ulong from_session_id)
        {
            //Delete where TelemetrySnapshots reference it via TyreWearPercent
            await ExecuteNonQueryAsync(avm, "delete wda from WheelDataArray as wda inner join TelemetrySnapshot on wda.Id = TelemetrySnapshot.TyreWearPercent inner join Lap on TelemetrySnapshot.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session_id).ToString());
            
            //Delete where a TelemetrySnapshots references it via TyreDamagePercent
            await ExecuteNonQueryAsync(avm, "delete wda from WheelDataArray as wda inner join TelemetrySnapshot on wda.Id = TelemetrySnapshot.TyreDamagePercent inner join Lap on TelemetrySnapshot.FromLap = Lap.Id where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session_id).ToString());
        
            //Delete where a Lap references it 
            await ExecuteNonQueryAsync(avm, "delete wda from WheelDataArray as wda inner join Lap on wda.Id = Lap.EndingTyreWear where Lap.FromSession = " + ApexVisualToolkit.ULongToLong(from_session_id).ToString());
        }

        private static async Task<ApexVisual.SessionDocumentation.WheelDataArray[]> DownloadWheelDataArraysFromCommandAsync(ApexVisualManager avm, string cmd)
        {
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            SqlDataReader dr = await sqlcmd.ExecuteReaderAsync();
            List<ApexVisual.SessionDocumentation.WheelDataArray> ToReturn = new List<SessionDocumentation.WheelDataArray>();
            while (dr.Read())
            {
                ToReturn.Add(ExtractWheelDataArrayFromSqlDataReader(dr));
            }
            sqlcon.Close();
            return ToReturn.ToArray();
        }

        private static ApexVisual.SessionDocumentation.WheelDataArray ExtractWheelDataArrayFromSqlDataReader(SqlDataReader dr)
        {
            ApexVisual.SessionDocumentation.WheelDataArray ToReturn = new SessionDocumentation.WheelDataArray();

            //Id
            try
            {
                ToReturn.Id = dr.GetGuid(dr.GetOrdinal("Id"));
            }
            catch
            {

            }

            //Rear left
            try
            {
                ToReturn.RearLeft = dr.GetByte(dr.GetOrdinal("RearLeft"));
            }
            catch
            {

            }

            //Rear right
            try
            {
                ToReturn.RearRight = dr.GetByte(dr.GetOrdinal("RearRight"));
            }
            catch
            {

            }

            //Front left
            try
            {
                ToReturn.FrontLeft = dr.GetByte(dr.GetOrdinal("FrontLeft"));
            }
            catch
            {

            }

            //Front right
            try
            {
                ToReturn.FrontRight = dr.GetByte(dr.GetOrdinal("FrontRight"));
            }
            catch
            {

            }

            return ToReturn;
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

        private static async Task ExecuteNonQueryAsync(this ApexVisualManager avm, string cmd)
        {
            SqlConnection sqlcon = GetSqlConnection(avm);
            sqlcon.Open();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
            await sqlcmd.ExecuteNonQueryAsync();
            sqlcon.Close();
        }

        #endregion
    
    }
}