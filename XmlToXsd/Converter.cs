using System;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsd
{
    class Converter
    {
        /** 파일 경로 설정 **/
        public string inputFilePath { get; set; }    // 입력 받은 xml 파일 경로
        public string outputFilePath { get; set; }   // 바꾼 후 xsd 파일 저장 경로

        /** 생성자 **/
        public Converter(string inputFilePath, string outputFilePath)
        {
            this.inputFilePath = inputFilePath;
            this.outputFilePath = outputFilePath;
        }

        /** 변환을 실행하는 메인 함수 **/
        public bool Convert()
        {
            try
            {
                // FileIO 객체 생성
                FileIO fileIO = new FileIO(inputFilePath, outputFilePath);

                // xml파일 읽기
                XmlDocument inputFile = new XmlDocument();
                fileIO.LoadIntputFile(inputFile);

                // OutputFileBuilder 객체 생성 및 변환 실행
                OutputFileBuilder outputFileBuilder = new OutputFileBuilder(inputFile);
                XDocument outputFile = outputFileBuilder.BuildOutputFile();

                // 변환한 xsd파일 저장
                fileIO.SaveOutputFile(outputFile);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
