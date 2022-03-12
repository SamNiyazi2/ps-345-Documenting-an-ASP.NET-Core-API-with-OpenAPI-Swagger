using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.API.Entities
{

    // 03/12/2022 01:26 am - SSN - [20220311-2302] - [001] - M03-09 - Ignoring warnings where appropriate
#pragma warning disable CS1591


    [Table("Authors")]
    public class Author
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
    
    
    // 03/12/2022 01:26 am - SSN - [20220311-2302] - [001] - M03-09 - Ignoring warnings where appropriate
#pragma warning restore CS1591


}
