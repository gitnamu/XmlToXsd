using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlToXsdConverter
{
    class MainClass
    {
        static void Main(string[] args)
        {
            /** 파일 경로 설정 **/
            const String inputFileUrl = @"C:\Users\Mobile_Workstation#1\Desktop\kim_namho\schema\1.0 Draft 210322\S-247_FC_ver_1.0.0_draft (20210326).xml";   // 바꿀 xml파일 경로
            const String outputFileUrl = @"C:\Users\Mobile_Workstation#1\Desktop\kim_namho\schema\1.0 Draft 210322\test.xsd";                                 // 바꾼 후 xsd파일 저장 경로

            /** FileIO 및 OutputFileBuilder 객체 생성 **/
            FileIO fileIO = new FileIO(inputFileUrl, outputFileUrl);
            OutputFileBuilder outputFileBuilder = new OutputFileBuilder();

            /** xml파일 읽기 **/
            XmlDocument inputFile = new XmlDocument();
            fileIO.loadIntputFile(inputFile);

            /** 변환 실행 **/
            XDocument outputFile = outputFileBuilder.buildOutputFile(inputFile);

            /** 변환한 xsd파일 저장**/
            fileIO.saveOutputFile(outputFile);

        }
    }
}
