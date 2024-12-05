using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Helpers
{
    /// <summary>
    /// This class is used for operations on Enums
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Gets a list of string values for a given enum.
        /// </summary>
        /// <typeparam name="T">The Enum to get the values of</typeparam>
        /// <returns>A string list containg the values.</returns>
        // Loosley based on https://stackoverflow.com/questions/25126006/converting-enum-values-into-an-string-array
        public static List<string> GetStringListFromEnum<T>() where T : Enum
        {
            List<string> list = new();

            var enumValues = Enum.GetValues(typeof(T));

            foreach(var value in enumValues)
            {
                if(value == null)
                {
                    continue; // don't add null values
                }

                list.Add(value.ToString() ?? ""); // aditional protection against null
            }

            return list;
        }
    }
}
