namespace MyApp.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
        public string Name { get; set; } 

        public int StudentCount { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
