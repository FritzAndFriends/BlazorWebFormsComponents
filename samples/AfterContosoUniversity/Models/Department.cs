using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentID { get; set; }

        [Required]
        [MaxLength(20)]
        public string DepartmentName { get; set; }

        public int BuildingNumber { get; set; }

        public int ManagingInstructorID { get; set; }

        public virtual ICollection<Cours> Courses { get; set; }

    }
}

