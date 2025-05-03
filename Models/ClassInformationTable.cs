// Models/ClassInformationTable.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class ClassInformationTable
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        [Range(0, 100)]
        public int Grade { get; set; }

        [Required]
        public string Department { get; set; } = string.Empty;
    }
}
