using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConverterWPF;
internal static class FileWriter
{
    /// <summary>
    /// Сохраняет данные в формате JSON
    /// </summary>
    /// <param name="data"></param>
    public static void GenerateJsonFile(IEnumerable<dynamic> data, string outputPath)
    {
        var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        var filePath = Path.Combine(outputPath, "report.json");
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Сохраняет файлы в хранилище
    /// </summary>
    /// <param name="filesCollection"></param>
    /// <param name="storagePath"></param>
    public static void SaveFiles(IEnumerable<string> filesCollection, string storagePath)
    {
        foreach (string file in filesCollection)
        {
            var name = Path.GetFileName(file);

            storagePath = Path.Combine(storagePath, name);
            File.Copy(file, storagePath, true);
        }

    }

    public static void SaveFiles(string filePath, string storagePath)
    {
        var name = Path.GetFileName(filePath);

        storagePath = Path.Combine(storagePath, name);
        File.Copy(filePath, storagePath, true);
    }
}
