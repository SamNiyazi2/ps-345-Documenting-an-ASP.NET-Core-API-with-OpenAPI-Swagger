using System;

namespace Library.API.Models
{
    // 03/11/2022 08:48 pm - SSN - [20220311-2045] - [001] - M03-06 - Demo - Incorporating XML comments on model classes
    /// <summary>
    /// An author with id, FirstName and LastName fields
    /// </summary>
    public class Author
    {
        /// <summary>
        /// The author's Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The author's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The author's last name
        /// </summary>
        public string LastName { get; set; }
    }
}
