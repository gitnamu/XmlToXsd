using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace XmlToXsd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string inputFilePath { get; set; }
        private string outputFilePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        /** 입력받을 xml 파일 선택하기 버튼을 눌렀을 때 동작 **/
        private void FindInputFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.InitialDirectory = @"C:\driver";   // 기본 폴더
            openFileDlg.Filter = "Xml Files | *.xml"; // 필터설정

            openFileDlg.CheckFileExists = true;   // 파일 존재여부확인
            openFileDlg.CheckPathExists = true;   // 폴더 존재여부확인

            // 파일 열기 (값의 유무 확인)
            if (openFileDlg.ShowDialog().GetValueOrDefault())
            {
                inputFilePathBox.Text = openFileDlg.FileName;
                this.inputFilePath = openFileDlg.FileName;
            }
        }

        /** 출력할 xsd 파일의 경로 선택하기 버튼 눌렀을 때 동작 **/
        private void FindOutputFolder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenDlg = new CommonOpenFileDialog();
            commonOpenDlg.IsFolderPicker = true;

            if(commonOpenDlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                outputFilePathBox.Text = commonOpenDlg.FileName;
                this.outputFilePath = commonOpenDlg.FileName;
            }
        }

        /** 변환하기 버튼 눌렀을 때 동작 **/
        private void ConvertFile(object sender, RoutedEventArgs e)
        {
            if(inputFilePathBox == null || outputFilePathBox == null)
            {
                return;
            }

            Converter converter = new Converter(inputFilePath, outputFilePath);
            converter.Convert();
        }
    }
}
