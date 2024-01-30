using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
namespace MergePdfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Address 
    {
        public Address(string address)
        {
            this.address = address;
        }
        public Address()
        {
            address = null;
        }
        public string address
        {
            get;
            set;
        }

    }
    public partial class MainWindow : Window
    {
        ObservableCollection<Address> addresses = new ObservableCollection<Address>();
        private List<string> files = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            Save.Text = Environment.CurrentDirectory + "\\mergepdf.pdf";
        }


        private void MergePdf(object sender, RoutedEventArgs e)
        {
            try
            {
                if (files.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите файлы!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Document document = new Document();
                var resultStream = new FileStream(Save.Text, FileMode.Create);
                document.OpenDocument();
                PdfReader pdfReader = null;
                PdfCopy pdfCopy = new PdfCopy(document, resultStream);
                foreach (string file in files)
                {
                    var fileStream = new FileStream(file, FileMode.Open);
                    pdfReader = new PdfReader(fileStream);
                    document.OpenDocument();
                    pdfCopy.AddDocument(pdfReader);
                    if (file == files[files.Count - 1])
                    {
                        document.Close();
                    }
                }
                MessageBox.Show("Объединение файлов прошло успешно.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
            }
            finally
            {
                files.Clear();
                addresses.Clear();
            }
        }

        private void AttachFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".pdf";
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
            bool? IsOpen = openFileDialog.ShowDialog();
            
            if (IsOpen == true)
            {
                files.AddRange(openFileDialog.FileNames);
                
                for (int i = 0; i < openFileDialog.SafeFileNames.Length; i++)
                {
                    Address currentAddress = new Address(openFileDialog.SafeFileNames[i]);
                    addresses.Add(currentAddress);
                    
                }

                ListFiles.ItemsSource = addresses;
            }
        }

        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            
            ListBox file = (ListBox)sender;
            if (file.SelectedItem == null)
                return;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Contains(file.SelectedItem.ToString()))
                {
                    files.RemoveAt(i);
                }
            }
            addresses.RemoveAt(ListFiles.SelectedIndex);
        }

        private void SelectSave(object sender)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".pdf";
            saveFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
            bool? IsSave = saveFileDialog.ShowDialog();
            if (IsSave == true)
            {
                Save.Text = saveFileDialog.FileName;  
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            SelectSave(sender);
        }

        private void ClickSave(object sender, MouseButtonEventArgs e)
        {
            SelectSave(sender);
        }

        private void RemoveFileButton(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int num = -1;
            for (int i = 0; i < addresses.Count; i++)
            {
                if(addresses[i].address == button.CommandParameter.ToString())
                {
                    num = i;
                }
            }
            files.RemoveAt(num);
            addresses.RemoveAt(num);
        }
    }
}
