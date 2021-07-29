using Microsoft.Win32;
using System.IO;
using System.Windows;

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
                inputFilePath = openFileDlg.FileName;
            }
        }

        /** 출력할 xsd 파일의 경로 선택하기 버튼 눌렀을 때 동작 **/
        private void FindOutputFolder(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XSD file (*.xsd)|*.xsd";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "");
                outputFilePathBox.Text = saveFileDialog.FileName;
                outputFilePath = saveFileDialog.FileName;
            }
        }

        private void ShowList()
        {

        }

        /** 변환하기 버튼 눌렀을 때 동작 **/
        private void ConvertFile(object sender, RoutedEventArgs e)
        {
            if (inputFilePathBox == null || outputFilePathBox == null)
            {
                return;
            }

            ShowList();

            Converter converter = new Converter(inputFilePath, outputFilePath);
            bool result = converter.Convert();
            if (result)
            {
                MessageBox.Show("정상적으로 변환이 완료되었습니다.");
            }
            else
            {
                MessageBox.Show("오류가 발생하였습니다.");
            }
        }
    }
}
