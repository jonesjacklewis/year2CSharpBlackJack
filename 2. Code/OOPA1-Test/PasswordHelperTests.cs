using OOPA1.Helpers;

namespace OOPA1_Test
{
    public class PasswordHelperTests
    {


        /// <summary>
        /// This test is to show that when null salt is provided, it returns a none null resultant salt.
        /// </summary>
        [Test]
        public void ShouldReturnNonNullSalt()
        {
            string rawText = "password123";
            string? initialSalt = null;

            // underscore for the hashedpassword as is not needed
            (string _, string salt) = PasswordHelper.HashPassword(rawText, initialSalt);

            Assert.That(salt, Is.Not.Null);
        }


        /// <summary>
        /// This test is to show that when non-null salt is provided, it returns the same salt.
        /// </summary>
        [Test]
        public void ShouldReturnSameSalt()
        {
            string rawText = "password123";
            string? initialSalt = "lIComNN+JcxFKV4sgYx1rg==";

            // underscore for the hashedpassword as is not needed
            (string _, string salt) = PasswordHelper.HashPassword(rawText, initialSalt);

            Assert.That(initialSalt, Is.EqualTo(salt));
        }


        /// <summary>
        /// This test is to show that a hashed password is generated based on the salt.
        /// </summary>
        [Test]
        public void ShouldReturnHashedPassword()
        {
            string rawText = "password123";
            string? initialSalt = "lIComNN+JcxFKV4sgYx1rg==";

            // underscore for the salt as is not needed
            (string hashedPassword, string _) = PasswordHelper.HashPassword(rawText, initialSalt);

            Assert.That(initialSalt, Is.Not.EqualTo(hashedPassword));
        }

        /// <summary>
        /// This test is to show that a hashed password is generated based on the salt, and is consistent.
        /// </summary>
        [Test]
        public void ShouldReturnConsistentHashedPassword()
        {
            string rawText = "password123!";
            string? initialSalt = "lIComNN+JcxFKV4sgYx1rg==";

            List<string> hashedPasswords = new();
            int numberOfTimesToHash = 100;

            // hash the same raw text, with the same salt 100 times
            while(hashedPasswords.Count != numberOfTimesToHash)
            {
                // underscore for the salt as is not needed
                (string hp, string _) = PasswordHelper.HashPassword(rawText, initialSalt);
                hashedPasswords.Add(hp);
            }

            // for comparison
            (string hashedPassword, string _) = PasswordHelper.HashPassword(rawText, initialSalt);

            // check they are all equal
            Assert.That(hashedPasswords, Is.All.EqualTo(hashedPassword));
        }

        /// <summary>
        /// Tests a null password
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenPasswordNull()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid(null);
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.False);
                Assert.That(reason, Is.EqualTo("Password cannot be null."));
            });
        }

        /// <summary>
        /// Test too short (<6 character) password
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenPasswordTooShort()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid("abc");
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.False);
                Assert.That(reason, Is.EqualTo("Password should be at least 6 characters long."));
            });
        }

        /// <summary>
        /// Test for when for all lowercase or all uppercase
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenPasswordOneCase()
        {
            (bool isValidLower, string reasonLower) = PasswordHelper.PasswordIsValid("abcdef");
            (bool isValidUpper, string reasonUpper) = PasswordHelper.PasswordIsValid("ABCDEF");
            Assert.Multiple(() =>
            {
                Assert.That(isValidLower, Is.False);
                Assert.That(reasonLower, Is.EqualTo("Password should be mixed case"));
                Assert.That(isValidUpper, Is.False);
                Assert.That(reasonUpper, Is.EqualTo("Password should be mixed case"));
            });
        }

        /// <summary>
        /// Test all alphabetic
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenNoDigit()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid("abcDEF");
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.False);
                Assert.That(reason, Is.EqualTo("Password should contain at least 1 digit"));
            });
        }

        /// <summary>
        /// Test for a space
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenIncludesSpace()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid("abc DEF1");
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.False);
                Assert.That(reason, Is.EqualTo("Password should not contain a space"));
            });
        }

        /// <summary>
        /// Test for special characters
        /// </summary>
        [Test]
        public void ShouldReturnFalseWhenNoSpecialCharacters()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid("abcDEF1");
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.False);
                Assert.That(reason, Is.EqualTo("Password should contain at least 1 special character."));
            });
        }

        /// <summary>
        /// Test for valid password
        /// </summary>
        [Test]
        public void ShouldReturnTrueWhenPasswordValid()
        {
            (bool isValid, string reason) = PasswordHelper.PasswordIsValid("abcDEF1!");
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.True);
                Assert.That(reason, Is.EqualTo(""));
            });
        }
    }
}