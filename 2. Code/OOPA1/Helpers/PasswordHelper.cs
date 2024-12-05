using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Helpers
{
    /// <summary>
    /// This class is used for password operations
    /// </summary>
    public class PasswordHelper
    {
        /// <summary>
        /// This method hashes a password for security.
        /// </summary>
        /// <param name="rawText">The raw text to be hashed.</param>
        /// <param name="salt">The salt to be used, defaults to null.</param>
        /// <returns>The hashedPassword, and the updated salt.</returns>
        // Based on https://stackoverflow.com/questions/12416249/hashing-a-string-with-sha256
        public static (string hashedPassword, string salt) HashPassword(string rawText, string? salt = null)
        {
            // ensure valid salt
            if (string.IsNullOrEmpty(salt))
            {
                byte[] saltBytes = RandomNumberGenerator.GetBytes(16); // generates a random byte array of length 16

                salt = Convert.ToBase64String(saltBytes); // converts the saltBytes into a string
            }

            byte[] rawTextBytes = Encoding.UTF8.GetBytes(rawText + salt);
            byte[] hashedRawTextBytes = SHA256.HashData(rawTextBytes);
            string hashedRawText = Convert.ToBase64String(hashedRawTextBytes);

            return (hashedRawText, salt); // tuple return
        }

        /// <summary>
        /// Checks whether the user's entered password is correct
        /// </summary>
        /// <param name="rawPassword">The password entered by the user</param>
        /// <param name="salt">The salt used</param>
        /// <param name="saltedPassword">The orignal salted password</param>
        /// <returns>True if new password hashes to the same result</returns>
        public static bool ValidateUser(string rawPassword, string salt, string saltedPassword)
        {
            (string newHashedPassword, _) = HashPassword(rawPassword, salt); // use of underscore, as at this point, we do not check against the salt
            return newHashedPassword == saltedPassword;
        }

        /// <summary>
        /// Checks a password is valid (not null, length >= 6, alphanumeric, multicase, contains special characters)
        /// </summary>
        /// <param name="password">The password to test</param>
        /// <returns>Tuple containg a boolean flag for whether the password is valid, and a string reason message.</returns>
        public static (bool isValid, string reason) PasswordIsValid(string password)
        {
            // must be 6 or more characters
            if(password == null) { return (false, "Password cannot be null."); }
            if(password.Length < 6) { return (false, "Password should be at least 6 characters long."); }

            // must be mixed case
            if(
                password == password.ToLower() || 
                password == password.ToUpper()
            )
            { 
                return (false, "Password should be mixed case"); 
            }

            // must contain at least one digit
            if (!(password.Any(char.IsDigit)))
            {
                return (false, "Password should contain at least 1 digit");
            }

            // must not contain a space
            if(password.Contains(' '))
            {
                return (false, "Password should not contain a space");
            }

            // should contain at least one special character
            char[] specialChars = { 
                '!',
                '£',
                '#',
                '$',
                '%',
                '^',
                '&',
                '*',
                '(',
                ')',
                '{',
                '}',
                '[',
                ']',
                ':',
                ';',
                '@',
                '~',
                ',',
                '.',
                '<',
                '>',
                '/',
                '?',
                '-',
                '_',
                '+',
                '=',
            };


            if (!(password.Any(specialChars.Contains))){
                return (false, "Password should contain at least 1 special character.");
            }

            return (true, "");

        }
    }
}
