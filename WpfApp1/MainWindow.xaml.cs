using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Security.Cryptography;
using System.IO;
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class files
        {
            public static string filename = "blank.csv";
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".csv"; // Default file extension
            dialog.Filter = "Book Database Files (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                files.filename = dialog.FileName;
                progressbar.Value = 50;
                BookAccelerator.Main1();
                progressbar.Value= 100;


            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void progressbar_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
    }
        public class BookAccelerator
        {

            public static void Main1()
            {

                string csv_file_path = files.filename;
                // gets filepath from the files function filename
                DataTable csvData = CSVreader.GetDataTableFromCSVFile(csv_file_path);
                Console.WriteLine($"Read {csvData.Rows.Count} records");

                List<Book> Books = new List<Book>();

                //for every row in the csv file it appends a new row to the json that also includes the hash
                foreach (DataRow row in csvData.Rows)
                {
                    Books.Add(new Book(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString()));
                }
                Console.WriteLine($"Sample - Books 50 was {Books[50]} ");

                var dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.FileName = "Document"; // Default file name
                dialog.DefaultExt = ".json"; // Default file extension
                dialog.Filter = "JSON FILE (.json)|*.json"; // Filter files by extension

                // Show save file dialog box
                bool? result = dialog.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
            // Save document
            
                    string filename = dialog.FileName;
                    int stuff = File.ReadLines(files.filename).Count();
                    SerializeToFile(Books, filename);
                    stuff = stuff - 1;
                    string stuff1 = stuff.ToString();


                    MessageBox.Show(stuff1 + " Records Processed");






        }



    }

            public static async Task SerializeToFile(object data, string filename)

            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                using var stream = File.Create(filename);
                await JsonSerializer.SerializeAsync(stream, data, options);
                await stream.DisposeAsync();
            }
        }



        static class CSVreader
        {
            public static DataTable GetDataTableFromCSVFile(string csv_file_path)
            {
                DataTable csvData = new DataTable();


                try
                {
                    using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                    {
                        csvReader.SetDelimiters(new string[] { "," });
                        csvReader.HasFieldsEnclosedInQuotes = true;
                        string[] colFields = csvReader.ReadFields();

                        foreach (string column in colFields)
                        {
                            DataColumn datecolumn = new DataColumn(column);
                            datecolumn.AllowDBNull = true;
                            csvData.Columns.Add(datecolumn);
                        }

                        while (!csvReader.EndOfData)
                        {
                            string[] fieldData = csvReader.ReadFields();
                            //Making empty value as null
                            for (int i = 0; i < fieldData.Length; i++)
                            {
                                if (fieldData[i] == "")
                                {
                                    fieldData[i] = null;
                                }
                            }

                            csvData.Rows.Add(fieldData);
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                return csvData;
            }
        }

        public readonly record struct Book
        {
            //This rec assigns each variable to parts of the csv
            public Book(string name, string title, string publishedIn, string publisher, string date)
            {
                
                Name = name;
                Title = title;
                PublishedIn = publishedIn;
                Publisher = publisher;
                Date = date;
                Cat = GetCatFor(name, title, publishedIn, publisher, date);
            }

            public string Cat { get; }
            public string Name { get; }
            public string Title { get; }
            public string PublishedIn { get; }
            public string Publisher { get; }
            public string Date { get; }


            private bool PrintMembers(StringBuilder stringBuilder)
            {
                stringBuilder.Append($"Catalogue = {Cat}, Author = {Name}, Title = {Title}, Published in {PublishedIn} by {Publisher}, {Date}");
                return true;
            }

            private static string GetCatFor(string name, string title, string publishedIn, string publisher, string date)
            {
                string source = name + title + publishedIn + publisher + date;
                using (MD5 md5 = MD5.Create()) // Using MD5 function it combines all parts of the book into one variable called source and then hashes it using the MD5 alogrithim, this is done to ensure duplicate books have the same ID
                {
                    string hash = GetHash(md5, source);
                    return hash[..10];
                }
            }

            private static string GetHash(HashAlgorithm hashAlgorithm, string input)
            {
                byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }

        }
    
