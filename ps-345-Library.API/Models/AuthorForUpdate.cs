using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{

    // 03/11/2022 09:33 pm - SSN - [20220311-2130] - [001] - M03-07 - Demo - Improving documentation with data annotations

    public class AuthorForUpdate
    {  

        /// <summary>The firt name of the author</summary>
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the author
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }
    }
}
