using System.Text.Json;

namespace MyApp.Helpers
{
        

    public class Utils
    {
        private static Utils? _instance;
        private static readonly object _lock = new();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new Utils();  // İlk kez instance oluşturuluyor
                    return _instance!;
                }
            }
        }

        // Genel JSON export metodu
        public string ExportToJson<T>(List<T> data, List<string>? selectedColumns = null)
        {
            // Eğer selectedColumns null ise boş liste ata
            selectedColumns ??= new List<string>();

            if (selectedColumns.Count == 0)
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }

            // Sadece seçilen kolonlara göre dinamik obje oluştur
            var filtered = data.Select(item =>
            {
                var dict = new Dictionary<string, object>();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        dict[prop.Name] =   prop.GetValue(item) ?? string.Empty; // null değerleri boş string ile değiştir
                    }
                }
                return dict;
            }).ToList();

            return JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
