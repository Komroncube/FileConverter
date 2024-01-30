using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace FileConverterWPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IConfiguration _configuration;
    private readonly ICollection<string> _filesCollection = new List<string>();
    private readonly ICollection<IEnumerable<dynamic>> _recordsToGeneration = new List<IEnumerable<dynamic>>();
    public MainWindow()
    {
        InitializeComponent();
        var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true, true);
        _configuration = builder.Build();
    }
    /// <summary>
    /// Генерация отчета
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if(!_filesCollection.Any()) return;
            FileWriter.SaveFiles(_filesCollection, _configuration["StoragePath"]);

            if(!_recordsToGeneration.Any()) return;
            FileWriter.GenerateJsonFile(_recordsToGeneration, _configuration["OutputPath"]);

            _recordsToGeneration.Clear();
            _filesCollection.Clear();
            lbFiles.Items.Clear();
            txtStatus.Text = "Отчет сгенерирован";

        }
        catch (Exception ex)
        {
            txtStatus.Text = "Ошибка: " + ex.Message;
        }
    }
    private string ProcessFiles()
    {
        var statusText = new StringBuilder();

        if (_filesCollection.Any() == false) return "Файлы не выбраны";

        foreach (string filePath in _filesCollection)
        {
            statusText.Append($"Обработка файла: {filePath}\n");

            IEnumerable<dynamic> fileData = FileReader.ReadFile(filePath).ToArray();
            if (fileData.Any() == false) continue;

            var dataFromFiles = FileReader.LoadFilesFromStorage(_configuration["StoragePath"]);
            foreach (var keys in dataFromFiles)
            {
                var matchingFieldName = DataMerger.FindMatchingFieldName(fileData, keys);
                if (string.IsNullOrEmpty(matchingFieldName)) continue;

                var mergingData = DataMerger.MergeData(fileData, keys, matchingFieldName).ToArray();
                statusText.Append($"Найдено совпадение с {mergingData.Count()} записями\n");
                _recordsToGeneration.Add(mergingData);
                break;
            }
        }
        ;
        return statusText.ToString();
    }



    private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFileDialog.ShowDialog() == true)
        {
            openFileDialog.FileNames.ToList().ForEach(filename =>
            {
                lbFiles.Items.Add(filename);
                _filesCollection.Add(filename);
            });
            txtStatus.Text = ProcessFiles();

        }
    }
}