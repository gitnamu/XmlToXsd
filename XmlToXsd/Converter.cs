﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlToXsd
{
    class Converter
    {
        /** 파일 경로 설정 **/
        public string inputFilePath { get; set; }    // 입력 받은 xml 파일 경로
        public string outputFilePath { get; set; }   // 바꾼 후 xsd 파일 저장 경로

        public Converter(string inputFilePath, string outputFilePath)
        {
            this.inputFilePath = inputFilePath;
            this.outputFilePath = outputFilePath;
        }

        public bool Convert()
        {
            if(this.inputFilePath == null || this.outputFilePath == null)
            {
                return false;
            }

            /** FileIO 객체 생성 **/
            FileIO fileIO = new FileIO(inputFilePath, outputFilePath);

            /** xml파일 읽기 **/
            XmlDocument inputFile = new XmlDocument();
            fileIO.LoadIntputFile(inputFile);

            /** OutputFileBuilder 객체 생성 및 변환 실행 **/
            OutputFileBuilder outputFileBuilder = new OutputFileBuilder(inputFile);
            XDocument outputFile = outputFileBuilder.BuildOutputFile();

            /** 변환한 xsd파일 저장**/
            fileIO.SaveOutputFile(outputFile);

            return true;
        }
    }
}