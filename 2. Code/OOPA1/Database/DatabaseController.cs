using Microsoft.Data.Sqlite;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Database
{
    /// <summary>
    /// This class holds methods related to database access
    /// </summary>
    public class DatabaseController
    {
        private static readonly string _connectionString = $"Data Source=Database/{Globals.DatabaseName}";

        /// <summary>
        /// This is the constructor for the DatabaseController. It sets up the necessary tables and data.
        /// </summary>
        public DatabaseController()
        {
           CreateTables();
           AddDefaultData();
        }


        /// <summary>
        /// This method creates the tables if they do not exist.
        /// </summary>
        private void CreateTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sqlFileContents = File.ReadAllText("Database/CreateTablesIfNotExists.sql"); // the create statements

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sqlFileContents;

            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Gets the number of user creates tables in the database.
        /// </summary>
        /// <returns>The number of user-craeted tables.</returns>
        public long GetNumberOfTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Based on https://stackoverflow.com/questions/5334882/how-to-get-list-of-all-the-tables-in-sqlite-programmatically
            string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;

            long numberOfTables = (long)(command.ExecuteScalar() ?? -1); // protect against possible null value

            connection.Close();

            return numberOfTables;
        }

        /// <summary>
        /// Ensures the database contains all default data.
        /// </summary>
        private void AddDefaultData()
        {
            CreateDefaultRoleTypes();
            CreateDefaultMessageStates();
            CreateDefaultUser();
        }

        /// <summary>
        /// Creates the default role types based on the enum.
        /// </summary>
        private void CreateDefaultRoleTypes()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            List<string> roleTypes = EnumHelper.GetStringListFromEnum<RoleTypes>();

            bool isFirstRun = true;

            command.CommandText = "INSERT OR IGNORE INTO RoleTypes (RoleName) VALUES ($RoleName);";

            foreach (string roleType in roleTypes)
            {
                if (!isFirstRun) // on subsequent runs, clear previous parameterisation
                {
                    command.Parameters.Clear();
                }
                else
                {
                    isFirstRun = false;
                }
                command.Parameters.AddWithValue("$RoleName", roleType);

                command.ExecuteNonQuery();

            }

            connection.Close();

        }

        /// <summary>
        /// Creates the default message states based on the enum.
        /// </summary>
        private void CreateDefaultMessageStates()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            List<string> messageStates = EnumHelper.GetStringListFromEnum<MessageStates>();

            bool isFirstRun = true;

            command.CommandText = "INSERT OR IGNORE INTO MessageStates (StateName) VALUES ($MessageState);";

            foreach (string messageState in messageStates)
            {
                if (!isFirstRun) // on subsequent runs, clear previous parameterisation
                {
                    command.Parameters.Clear();
                }
                else
                {
                    isFirstRun = false;
                }
                command.Parameters.AddWithValue("$MessageState", messageState);

                command.ExecuteNonQuery();

            }

            connection.Close();
        }
        

        /// <summary>
        /// Gets the id for a specific role type from the database
        /// </summary>
        /// <param name="roleType">The role type to get the ID of</param>
        /// <returns>The database id for that role type</returns>
        public int GetRoleTypeIdForRoleType(RoleTypes roleType)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sql = "SELECT Id from RoleTypes WHERE RoleName = $RoleName";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("$RoleName", roleType.ToString());

            int id = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return id;
        }

        /// <summary>
        /// Gets the Id for a given User Id
        /// </summary>
        /// <param name="username">The username for who we get the ID</param>
        /// <returns>The id for a specific user</returns>
        public int GetUserId(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT UsersId FROM Users WHERE UserName = $UserName;";
            command.Parameters.AddWithValue("$UserName", username);

            int id = Convert.ToInt32(command.ExecuteScalar() ?? -1); // null protection
            connection.Close();

            return id;
        }

        /// <summary>
        /// Gets all usernames in the database.
        /// </summary>
        /// <returns>A list of strings containg the usernames</returns>
        public List<string> GetUsernames()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT UserName FROM Users;";

            List<string> usernames = new();

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                usernames.Add(reader.GetValue(0)?.ToString() ?? ""); // null protection
            }

            usernames = usernames.Distinct().Where(username => username != "").ToList(); // filter out duplicates/empty strings from null protection

            return usernames;

        }

        /// <summary>
        /// Gets all usernames in the database for a specific role.
        /// </summary>
        /// <returns>A list of strings containg the usernames of a specific role</returns>
        public List<string> GetUsersForSpecificRole(RoleTypes roleType)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT Username FROM Users u JOIN RoleTypes r ON u.RoleTypesId = r.Id WHERE r.RoleName = $RoleType;";
            command.Parameters.AddWithValue("$RoleType", roleType.ToString());

            List<string> usernames = new();

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                usernames.Add(reader.GetValue(0)?.ToString() ?? ""); // null protection
            }

            usernames = usernames.Distinct().Where(username => username != "").ToList(); // filter out duplicates/empty strings from null protection

            return usernames;
        }

        /// <summary>
        /// Adds value to user balanace
        /// </summary>
        /// <param name="userId">The user the balance should belong to</param>
        /// <param name="balancePence">The balanace</param>
        private void AddUserBalance(int userId, int balancePence)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT OR IGNORE INTO UserBalances (UsersId, BalancePence) VALUES ($UserId, $BalancePence);";

            command.Parameters.AddWithValue("$UserId", userId);
            command.Parameters.AddWithValue("$BalancePence", balancePence);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Adds a user with a none hashed password
        /// </summary>
        /// <param name="user">The DbUser model, where the password is yet to be hashed</param>
        public void AddUserBeforeHash(DbUser user)
        {
            (string hashedPassword, string salt) = PasswordHelper.HashPassword(user.Password);

            UserHash userHash = new(user.Username, salt);

            AddUserHash(userHash);

            DbUser dbUser = new(user.Username, hashedPassword, user.RoleTypesId);

            AddDbUser(dbUser);

            int id = GetUserId(user.Username);

            int oneHundredPounds = (100) * 100; // the value of £100 in pence

            AddUserBalance(id, oneHundredPounds);

        }

        /// <summary>
        /// Wrapper for UpdateUserBalanace, but to work with string username
        /// </summary>
        /// <param name="username">The username to update</param>
        /// <param name="balancePence">The new balance</param>
        public void UpdateUserBalanace(string username, int balancePence)
        {
            int userId = GetUserId(username);
            UpdateUserBalanace(userId, balancePence);
        }

        /// <summary>
        /// Updates the balance for a user
        /// </summary>
        /// <param name="usersId">The user ID to updadte</param>
        /// <param name="balancePence">The updated balance</param>
        public void UpdateUserBalanace(int usersId, int balancePence)
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE UserBalances SET BalancePence = $BalancePence WHERE UsersId = $UsersId";

            command.Parameters.AddWithValue("$BalancePence", balancePence);
            command.Parameters.AddWithValue("$UsersId", usersId);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Sets all User Balanaces to £100
        /// </summary>
        public void ResetAllBalance()
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE UserBalances SET BalancePence = $BalancePence";

            int oneHundredPounds = 100 * 100; // £100 in pence

            command.Parameters.AddWithValue("$BalancePence", oneHundredPounds);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Adds a user with a none hashed password, and sets a balanace
        /// </summary>
        /// <param name="user">The DbUser model, where the password is yet to be hashed, with a balance</param>
        public void AddUserBeforeHashWithBalanace(DbUserWithBalanace user)
        {
            AddUserBeforeHash(user);
            int userId = GetUserId(user.Username);
            UpdateUserBalanace(userId, user.BalanacePence);
        }

        /// <summary>
        /// Adds a UserHash object to the UserHashes table
        /// </summary>
        /// <param name="userHash">The user to add</param>
        private void AddUserHash(UserHash userHash)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sql = "INSERT OR IGNORE INTO UserHashes (Username, Hash) VALUES ($Username, $Hash)";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            
            command.Parameters.AddWithValue("$Username", userHash.Username);
            command.Parameters.AddWithValue("$Hash", userHash.Hash);

            command.ExecuteNonQuery();

            connection.Close();

        }

        /// <summary>
        /// Adds a DbUser object to the Users table
        /// </summary>
        /// <param name="dbUser">The DbUser to add</param>
        private void AddDbUser(DbUser dbUser)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sql = "INSERT OR IGNORE INTO Users (Username, HashedPassword, RoleTypesId) VALUES ($Username, $HashedPassword, $RoleTypesId)";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;

            command.Parameters.AddWithValue("$Username", dbUser.Username);
            command.Parameters.AddWithValue("$HashedPassword", dbUser.Password);
            command.Parameters.AddWithValue("$RoleTypesId", dbUser.RoleTypesId);

            command.ExecuteNonQuery();

            connection.Close();

        }

        /// <summary>
        /// Creates a default user admin.
        /// </summary>
        private void CreateDefaultUser()
        {
            string username = "DefaultAdmin";
            string password = "DefaultAdminPassword123!";

            (string hashedPassword, string salt) = PasswordHelper.HashPassword(password);

            UserHash userHash = new(username, salt);

            AddUserHash(userHash);

            int roleTypeId = GetRoleTypeIdForRoleType(RoleTypes.Admin);

            DbUser dbUser = new(username, hashedPassword, roleTypeId);

            AddDbUser(dbUser);
        }

        /// <summary>
        /// Checks whether a given username is in the UserHashes table
        /// </summary>
        /// <param name="username">A username to check</param>
        /// <returns>True if username in table, else false</returns>
        private bool UsernameInUserHashes(string username)
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Username FROM UserHashes WHERE Username = $Username";
            command.Parameters.AddWithValue("$Username", username);

            bool result = command.ExecuteScalar() != null;

            connection.Close();

            return result;

        }

        /// <summary>
        /// Gets the balance for a specific username
        /// </summary>
        /// <param name="username">The username to get the balanace of</param>
        /// <returns>The balance for that user</returns>
        public int GetBalanceForUser(string username)
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT ub.BalancePence FROM UserBalances ub JOIN Users u ON ub.UsersId = u.UsersId WHERE u.Username = $Username";

            command.Parameters.AddWithValue("$Username", username);

            int balancePence = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return balancePence;
        }

        /// <summary>
        /// Gets the Hash for a user.
        /// </summary>
        /// <param name="username">The user that the hash should be retrieved for.</param>
        /// <returns>the users's hash or null</returns>
        public string? GetHashForUser(string username)
        {
            if (!CheckUserExists(username))
            {
                return null;
            }

            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Hash FROM UserHashes WHERE Username = $Username";
            command.Parameters.AddWithValue("$Username", username);

            object? objResult = command.ExecuteScalar();

            connection.Close();

            if(objResult == null)
            {
                return null;
            }

            return (string)objResult;
        }

        /// <summary>
        /// Gets the Hashed Password for a user.
        /// </summary>
        /// <param name="username">The user that the hashed password should be retrieved for.</param>
        /// <returns>the users's hashed password or null</returns>
        public string? GetHashedPasswordForUser(string username)
        {
            if (!CheckUserExists(username))
            {
                return null;
            }

            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT HashedPassword FROM Users WHERE Username = $Username";
            command.Parameters.AddWithValue("$Username", username);

            object? objResult = command.ExecuteScalar();

            connection.Close();

            if (objResult == null)
            {
                return null;
            }

            return (string)objResult;
        }

        /// <summary>
        /// Checks whether a given username is in the Users table
        /// </summary>
        /// <param name="username">A username to check</param>
        /// <returns>True if username in table, else false</returns>
        private bool UsernameInUsers(string username)
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Username FROM Users WHERE Username = $Username";
            command.Parameters.AddWithValue("$Username", username);

            bool result = command.ExecuteScalar() != null;

            connection.Close();

            return result;

        }

        /// <summary>
        /// Checks whether a user exists
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is in UserHashes and Users, else false</returns>
        public bool CheckUserExists(string username)
        {
            bool InHashes = UsernameInUserHashes(username);
            bool InUsers = UsernameInUsers(username);
            return InHashes && InUsers;
        }

        /// <summary>
        /// Gets all role types in the database.
        /// </summary>
        /// <returns>A list of strings containg the role types</returns>
        public List<string> GetRoleTypes()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            List<string> roles = new();

            command.CommandText = "SELECT RoleName FROM RoleTypes";

            var reader = command.ExecuteReader();

            while (reader.Read()) // Read the data and store it in the list
            {
                roles.Add(reader.GetString(0));
            }

            return roles;

        }

        /// <summary>
        /// Gets all message states in the database.
        /// </summary>
        /// <returns>A list of strings containg the message states</returns>
        public List<string> GetMessageStates()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            List<string> states = new();

            command.CommandText = "SELECT StateName FROM MessageStates";

            var reader = command.ExecuteReader();

            while (reader.Read()) // Read the data and store it in the list
            {
                states.Add(reader.GetString(0));
            }

            return states;

        }

        /// <summary>
        /// Gets the role type id for a given username
        /// </summary>
        /// <param name="username">The username to get the ID for</param>
        /// <returns>The role type id for this user</returns>
        private int GetRoleTypeIdForUsername(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sql = "SELECT RoleTypesId from Users WHERE Username = $Username";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("$Username", username);

            int id = Convert.ToInt32(command.ExecuteScalar() ?? ((long)-1)); // null protection

            connection.Close();

            return id;
        }

        /// <summary>
        /// Gets the role type for a specfic role type id
        /// </summary>
        /// <param name="id">The ID to get the role type of</param>
        /// <returns>The Role Type as a enum</returns>
        private RoleTypes? GetRoleTypeById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string sql = "SELECT RoleName from RoleTypes WHERE Id = $Id";

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("$Id", id);

            object? roleTypesObj = command.ExecuteScalar();

            connection.Close();


            if (roleTypesObj == null)
            {
                return null;
            }

            string? roleTypeString = roleTypesObj?.ToString(); // extra proection against null

            RoleTypes? roleType = Enum.Parse<RoleTypes>(roleTypeString ?? ""); // extra proection against null

            return roleType;
        }

        /// <summary>
        /// Gets the Role Type enum value for a given user
        /// </summary>
        /// <param name="username">The username of the user for who we are getting the role type</param>
        /// <returns>The role type for the user, or null</returns>
        public RoleTypes? GetRoleTypeForUsername(string username)
        {
            int roleTypeId = GetRoleTypeIdForUsername(username);

            if(roleTypeId == -1)
            {
                return null;
            }

            RoleTypes? roleType = GetRoleTypeById(roleTypeId);

            return roleType;
        }

        /// <summary>
        /// Updates the UserHash for a specific user
        /// </summary>
        /// <param name="username">The user to update</param>
        /// <param name="newHash">The updated hash</param>
        private void UpdateUserHash(string username, string newHash)
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE UserHashes SET Hash = $NewHash WHERE UserName = $UserName";

            command.Parameters.AddWithValue("$NewHash", newHash);
            command.Parameters.AddWithValue("$UserName", username);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Updates the User's hashed password
        /// </summary>
        /// <param name="username">The user to update</param>
        /// <param name="hashedPassword">The updated hashed password</param>
        private void UpdateUserPassword(string username, string hashedPassword)
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET HashedPassword = $HashedPassword WHERE UserName = $UserName";

            command.Parameters.AddWithValue("$HashedPassword", hashedPassword);
            command.Parameters.AddWithValue("$UserName", username);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Resets the password for a user
        /// </summary>
        /// <param name="username">The user to update</param>
        /// <param name="newPassword">The plain text new password</param>
        public void ResetUserPassword(string username, string newPassword)
        {
            (string hashedPassword, string salt) = PasswordHelper.HashPassword(newPassword);

            UpdateUserHash(username, salt);
            UpdateUserPassword(username, hashedPassword);
        }

        /// <summary>
        /// Gets the ID for a specific role type
        /// </summary>
        /// <param name="roleType">The role type to get</param>
        /// <returns>The id for that role type</returns>
        public int GetRoleTypeIdByRole(RoleTypes roleType)
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM RoleTypes WHERE RoleName = $RoleName";

            command.Parameters.AddWithValue("$RoleName", roleType.ToString());

            int id = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return id;
        }

        /// <summary>
        /// Gets the ID for a specific message state
        /// </summary>
        /// <param name="messageState">The message state to get the ID of</param>
        /// <returns>The id of the message state</returns>
        public int GetMessageStateIdByMessageState(MessageStates messageState)
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM MessageStates WHERE StateName = $StateName";

            command.Parameters.AddWithValue("$StateName", messageState.ToString());

            int id = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return id;
        }

        public void AddMessage(Message message)
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT OR REPLACE INTO Messages (UsersId, MessageStatesId, DateTime) VALUES ($UsersId, $MessageStatesId, $DateTime)";

            int usersId = GetUserId(message.Username);
            int messageStateId = GetMessageStateIdByMessageState(message.MessageState);
            DateTime dateTime = message.DateTime;

            command.Parameters.AddWithValue("$UsersId", usersId);
            command.Parameters.AddWithValue("$MessageStatesId", messageStateId);
            command.Parameters.AddWithValue("$DateTime", dateTime);

            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Gets a list of messages
        /// </summary>
        /// <returns>A list of message objects</returns>
        public List<Message> GetMessages()
        {
            SqliteConnection connection = new(_connectionString);
            connection.Open();

            StringBuilder queryStringBuilder = new(); // StringBuilder used for easier formatting
            queryStringBuilder.Append("SELECT m.DateTime, ms.StateName, u.Username, ub.BalancePence ");
            queryStringBuilder.Append("FROM Messages m ");
            queryStringBuilder.Append("JOIN MessageStates ms ON m.MessageStatesId = ms.Id ");
            queryStringBuilder.Append("JOIN Users u ON u.UsersId = m.UsersId ");
            queryStringBuilder.Append("JOIN UserBalances ub ON ub.UsersId = u.UsersId ");
            queryStringBuilder.Append("WHERE ms.StateName = 'Unread';"); // gets unread messages
            

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = queryStringBuilder.ToString();

            var reader = command.ExecuteReader();

            List<Message> messages = new();

            while (reader.Read())
            {
                DateTime dateTime = reader.GetDateTime(0);
                String stateName = reader.GetString(1);
                String username = reader.GetString(2);
                int balancePence = reader.GetInt32(3);

                MessageStates messageState = Enum.Parse<MessageStates>(stateName);

                Message message = new(dateTime, username, balancePence, messageState);

                messages.Add(message);
            }

            connection.Close();
            return messages;
        }

        /// <summary>
        /// Gets the top n balances. Used for the scoreboard
        /// </summary>
        /// <param name="n">The number of users to get the balance of</param>
        /// <returns>A list of users and balances sorted high to low, including a rank</returns>
        public List<UserBalanceRank> GetTopNBalances(int n)
        {
            if(n <= 0)
            {
                n = 5; // sets the minimum to get at 5
            }

            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            // example of rank: https://www.sqlitetutorial.net/sqlite-window-functions/sqlite-rank/
            StringBuilder queryStringBuilder = new(); // StringBuilder used for easier formatting

            queryStringBuilder.Append(" SELECT u.UserName, ub.BalancePence, RANK() OVER (ORDER BY ub.BalancePence DESC) AS Rank ");
            queryStringBuilder.Append(" FROM Users u ");
            queryStringBuilder.Append(" JOIN UserBalances ub ");
            queryStringBuilder.Append(" ON u.UsersId = ub.UsersId ");
            queryStringBuilder.Append(" JOIN RoleTypes r ");
            queryStringBuilder.Append(" ON r.Id = u.RoleTypesId ");
            queryStringBuilder.Append(" WHERE r.RoleName <> 'Admin' ");
            queryStringBuilder.Append($" LIMIT {n} "); // interpolation has been used instead of standard parameterisation, as cannot use parameterisation in WHERE clause. As this method is directly called requiring an int, no security risk.

            command.CommandText = queryStringBuilder.ToString();

            var reader = command.ExecuteReader();

            List<UserBalanceRank> ranking = new();

            while (reader.Read())
            {
                string username = reader.GetString(0);
                int balancePence = reader.GetInt32(1);
                int rank = reader.GetInt32(2);

                UserBalanceRank userBalanceRank = new(username, balancePence, rank);

                ranking.Add(userBalanceRank);
            }


            connection.Close();

            return ranking;
        }
    }
}
