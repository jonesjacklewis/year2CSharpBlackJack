using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;

namespace OOPA1_Test
{
    public class DatabaseControllerTests
    {
        /// <summary>
        /// This test is to show that there is 6 tables created.
        /// </summary>
        [Test]
        public void ShouldReturnExpectedNumberOfTables()
        {
            DatabaseController databaseController = new();
            long numberOfTables = databaseController.GetNumberOfTables();
            long expectedNumberOfTables = 6; // expected number of tables from the SQL

            Assert.That(expectedNumberOfTables, Is.EqualTo(numberOfTables));
        }

        /// <summary>
        /// This test is to show that there are at leasat the same number of rows as items in the enum for role types.
        /// </summary>
        [Test]
        public void ShouldReturnSameNumberOfRoleTypesAsEnum()
        {
            DatabaseController databaseController = new();
            List<string> dbRoleTypes = databaseController.GetRoleTypes();
            int expectedRoleCount = EnumHelper.GetStringListFromEnum<RoleTypes>().Count;

            Assert.That(dbRoleTypes, Has.Count.GreaterThanOrEqualTo(expectedRoleCount));
        }

        /// <summary>
        /// This test is to show that there are at leasat the same number of rows as items in the enum for message states.
        /// </summary>
        [Test]
        public void ShouldReturnSameNumberOfMessageStatesAsEnum()
        {
            DatabaseController databaseController = new();
            List<string> dbRoleTypes = databaseController.GetMessageStates();
            int expectedRoleCount = EnumHelper.GetStringListFromEnum<MessageStates>().Count;

            Assert.That(dbRoleTypes, Has.Count.GreaterThanOrEqualTo(expectedRoleCount));
        }

        /// <summary>
        /// This test is to show that it successfully retrieves the correct ID
        /// </summary>
        [Test]
        public void ShouldReturnCorrectIdForRoleType()
        {
            DatabaseController databaseController = new();
            RoleTypes admin = RoleTypes.Admin;
            int expectedId = 1;

            int id = databaseController.GetRoleTypeIdForRoleType(admin);

            Assert.That(id, Is.EqualTo(expectedId));
        }

        /// <summary>
        /// This test is to show that the DefaultAdmin user exists
        /// </summary>
        [Test]
        public void ShouldReturnTrueIfDefaultAdminExists()
        {
            DatabaseController databaseController = new();
            string username = "DefaultAdmin";

            Assert.That(databaseController.CheckUserExists(username), Is.True);
        }

        /// <summary>
        /// Test to check that DefaultAdmin returns the correct role type
        /// </summary>
        [Test]
        public void ShouldReturnCorrectRoleTypeForUser()
        {
            DatabaseController databaseController = new();
            string username = "DefaultAdmin";
            RoleTypes expectedRoleType = RoleTypes.Admin;

            RoleTypes? returnedRoleType = databaseController.GetRoleTypeForUsername(username);

            Assert.That(returnedRoleType, Is.EqualTo(expectedRoleType));
        }

        /// <summary>
        /// Test that there is at least one user (DefaultAdmin)
        /// </summary>
        [Test]
        public void ShouldReturnListOfUsers()
        {
            DatabaseController dbController = new();
            List<string> users = dbController.GetUsernames();


            Assert.Multiple(() =>
            {
                Assert.That(users, Is.Not.Empty);
                Assert.That(users, Does.Contain("DefaultAdmin"));
            }
            );
        }

        /// <summary>
        /// Test GetRoleTypeId
        /// </summary>
        [Test]
        public void ShouldReturnIdForRoleType()
        {
            DatabaseController databaseController = new();
            RoleTypes roleType = RoleTypes.Admin;

            int roleTypeId = databaseController.GetRoleTypeIdForRoleType(roleType);

            Assert.That(roleTypeId, Is.GreaterThanOrEqualTo(0));
        }
    }
}