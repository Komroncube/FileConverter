using System.Dynamic;

namespace FileConverterWPF;
internal static class DataMerger
{
    /// <summary>
    /// Находит первое совпадающее поле в двух наборах данных
    /// </summary>
    /// <param name="csvData"></param>
    /// <param name="xmlData"></param>
    /// <returns></returns>
    public static string FindMatchingFieldName(IEnumerable<dynamic> firstCollection , IEnumerable<dynamic> secondCollection )
    {
        if (!firstCollection.Any() || !secondCollection.Any())
            return null;

        var csvFields = ((IDictionary<string, object>)firstCollection.First()).Keys;
        var xmlFields = ((IDictionary<string, object>)secondCollection.First()).Keys;

        return csvFields.Intersect(xmlFields).FirstOrDefault();
    }


    /// <summary>
    /// Объединяет два набора данных по совпадающему полю
    /// </summary>
    /// <param name="firstCollection"></param>
    /// <param name="secondCollection"></param>
    /// <returns></returns>
    public static IEnumerable<dynamic> MergeData(IEnumerable<dynamic> firstCollection, IEnumerable<dynamic> secondCollection, string matchingFieldName)
    {
        var matchedData = new List<dynamic>();

        if (string.IsNullOrEmpty(matchingFieldName))
            return matchedData; // Нет совпадающих полей

        foreach (var firstRecord in firstCollection)
        {
            var firstValue = ((IDictionary<string, object>)firstRecord)[matchingFieldName];
            var secondRecord = secondCollection.FirstOrDefault(x => ((IDictionary<string, object>)x)[matchingFieldName].Equals(firstValue));
            
            dynamic mergedRecord = new ExpandoObject();
            var mergedDict = (IDictionary<string, object>)mergedRecord;

            // Добавляем поля из первой записи
            foreach (var field in (IDictionary<string, object>)firstRecord)
            {
                mergedDict[field.Key] = field.Value;
            }

            // Добавляем поля из второй записи
            foreach (var field in (IDictionary<string, object>)secondRecord)
            {
                // Проверяем, чтобы не добавить дубликат общего поля
                if (!mergedDict.ContainsKey(field.Key))
                {
                    mergedDict[field.Key] = field.Value;
                }
            }

            matchedData.Add(mergedRecord);
        }

        return matchedData;
    }
}
