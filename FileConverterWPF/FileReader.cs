using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using CsvHelper;

namespace FileConverterWPF;
internal static class FileReader
{
    public static IEnumerable<dynamic> ReadFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return extension switch
        {
            ".csv" => ReadCsvFile(filePath),
            ".xml" => ReadXmlFile(filePath),
            _ => throw new NotSupportedException($"Формат {extension} не поддерживается")
        };
    }
    
    /// <summary>
    /// Чтение CSV файла
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static IEnumerable<dynamic> ReadCsvFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<dynamic>().ToList();
    }

    /// <summary>
    /// Чтение XML файла
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static IEnumerable<dynamic> ReadXmlFile(string filePath)
    {
        var xmlDocument = XDocument.Load(filePath);
        var records = new List<dynamic>();

        // Получаем корневой элемент и читаем все его дочерние элементы
        var root = xmlDocument.Root;
        if (root == null) return records;

        foreach (var childElement in root.Elements())
        {
            dynamic record = new ExpandoObject();
            var recordDict = (IDictionary<string, object>)record;

            // Добавляем атрибуты дочернего элемента
            foreach (var attribute in childElement.Attributes())
            {
                recordDict[attribute.Name.LocalName] = attribute.Value;
            }

            // Добавляем вложенные элементы
            foreach (var element in childElement.Elements())
            {
                recordDict[element.Name.LocalName] = element.Value;
            }

            records.Add(record);
        }

        return records;
    }

    /// <summary>
    /// Чтение всех файлов из хранилища
    /// </summary>
    /// <param name="storagePath"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<dynamic>> LoadFilesFromStorage(string storagePath)
    {
        var keys = new List<IEnumerable<dynamic>>();
        var files = Directory.GetFiles(storagePath);
        foreach (var file in files)
        {
            var fileKeys = FileReader.ReadFile(file);
            keys.Add(fileKeys);
        }
        return keys;
    }
}
