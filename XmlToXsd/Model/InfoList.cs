using System;
using System.Collections.Generic;
using System.Text;

namespace XmlToXsd
{
    class InfoList
    {
        private InputFileReader inputFileReader;

        /** 생성자 **/
        public InfoList(InputFileReader inputFileReader)
        {
            this.inputFileReader = inputFileReader;
        }

        public void test()
        {
            foreach (S100_FC_SimpleAttribute simple in inputFileReader.s100_FC_SimpleAttribute)
            {
                
            }
        }
    }
}
