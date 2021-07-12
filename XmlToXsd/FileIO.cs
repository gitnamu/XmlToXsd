using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsdConverter
{
    class FileIO
    {
        /** private 변수 **/
        private String inputFileUrl { get; }
        private String outputFileUrl { get; }
        
        /** 생성자 **/
        public FileIO(String inputFileUrl, String outputFileUrl)
        {
            this.inputFileUrl = inputFileUrl;
            this.outputFileUrl = outputFileUrl;
        }

        /** input파일(xml) 불러오기 **/
        public void loadIntputFile(XmlDocument inputFile)
        {
            inputFile.Load(inputFileUrl);
        }

        /** output파일(xsd) 저장하기 **/
        public void saveOutputFile(XDocument outputFile)
        {
            outputFile.Save(outputFileUrl);
        }
    }
}
