using Microsoft.Win32;
using System.Collections.Generic;
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
        Converter converter;

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

        /** 정보 창에 simpleAttribute 및 complexAttribute 표시 **/
        private void ShowList()
        {
            // simpleAttribute 요소들 xaml에 추가
            List<S100_FC_SimpleAttribute> s100_FC_SimpleAttributeList = converter.s100_FC_SimpleAttribute;
            simpleAttribute.Items.Add("개수: "+s100_FC_SimpleAttributeList.Count);
            foreach (S100_FC_SimpleAttribute s100_FC_SimpleAttribute in s100_FC_SimpleAttributeList)
            {
                simpleAttribute.Items.Add(s100_FC_SimpleAttribute.name);
            }

            // complexAttribute 요소들 xaml에 추가
            List<S100_FC_ComplexAttribute> s100_FC_ComplexAttributeList = converter.s100_FC_ComplexAttribute;
            complexAttribute.Items.Add("개수: " + s100_FC_ComplexAttributeList.Count);
            foreach (S100_FC_ComplexAttribute s100_FC_ComplexAttribute in s100_FC_ComplexAttributeList)
            {
                complexAttribute.Items.Add(s100_FC_ComplexAttribute.name);
            }
        }

        /** 변환하기 버튼 눌렀을 때 동작 **/
        private void ConvertFile(object sender, RoutedEventArgs e)
        {
            if (inputFilePathBox == null || outputFilePathBox == null)
            {
                return;
            }

            converter = new Converter(inputFilePath, outputFilePath);
            bool result = converter.Convert();
            ShowList();
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
