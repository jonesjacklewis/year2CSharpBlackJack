using OOPA1.Helpers;
using OOPA1.Enums;

namespace OOPA1_Test
{
    public class EnumHelperTests
    {


        /// <summary>
        /// This test is to show that when an enum is passed in, the expected values are returned.
        /// </summary>
        [Test]
        public void ShouldReturnEnumValues()
        {
            List<string> enumValues = EnumHelper.GetStringListFromEnum<RoleTypes>();
            List<string> expectedValues = new(){
                "Admin",
                "Player"
            };

            Assert.That(enumValues, Is.EquivalentTo(expectedValues));
        }

    }
}