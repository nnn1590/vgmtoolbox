﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractAdxWorker: AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        static readonly byte[] ADX_SIG_BYTES = new byte[] { 0x80, 0x00 };
        static readonly byte[] CRI_COPYRIGHT_BYTES = new byte[] { 0x28, 0x63, 0x29, 0x43, 0x52, 0x49 };

        public struct ExtractAdxStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }
        
        public ExtractAdxWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractAdxStruct, DoWorkEventArgs e)
        {
            ExtractAdxStruct extractAdxStruct = (ExtractAdxStruct)pExtractAdxStruct;

            long offset = 0;

            uint copyrightOffset;
            byte[] copyrightBytes;
            uint totalHeaderSize;

            int encodingType;
            uint blockSize;
            uint bitDepth;
            uint channelCount;
            uint sampleRate;
            
            uint totalSamples;
            uint totalBytes;

            uint fileSize;

            int fileCount = 0;
            string outputPath = Path.Combine(Path.GetDirectoryName(pPath), "_sony_adpcm_ext");
            string outputFileName;
            string outputFilePath;
            
            FileInfo fi = new FileInfo(pPath);            

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                outputPath = Path.Combine(Path.GetDirectoryName(pPath), String.Format("{0}_ADXs", Path.GetFileNameWithoutExtension(pPath)));
                
                while ((offset = ParseFile.GetNextOffset(fs, offset, ADX_SIG_BYTES)) > -1)
                {
                    // get offset to copyright string
                    copyrightOffset = (uint)ByteConversion.GetUInt16BigEndian(ParseFile.ParseSimpleOffset(fs, offset + 2, 2));
                    copyrightBytes = ParseFile.ParseSimpleOffset(fs, offset + copyrightOffset - 2, CRI_COPYRIGHT_BYTES.Length);                    

                    // check that copyright bytes are present
                    if (ParseFile.CompareSegment(copyrightBytes, 0, CRI_COPYRIGHT_BYTES))
                    {
                        // verify this is standard ADX
                        encodingType = ParseFile.ParseSimpleOffset(fs, offset + 4, 1)[0];

                        if (encodingType != 3)
                        {
                            continue;
                        }

                        // get other info
                        blockSize = (uint)ParseFile.ParseSimpleOffset(fs, offset + 5, 1)[0];
                        bitDepth = (uint)ParseFile.ParseSimpleOffset(fs, offset + 6, 1)[0];
                        channelCount = (uint)ParseFile.ParseSimpleOffset(fs, offset + 7, 1)[0];
                        sampleRate = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(fs, offset + 8, 4));
                        totalSamples = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4));
                        totalHeaderSize = copyrightOffset + 4;

                        // calculate file size
                        totalBytes = (totalSamples) / (bitDepth * 8);
                        fileSize = (totalBytes * channelCount * blockSize) + totalHeaderSize;

                        // extract file
                        outputFileName = String.Format("{0}_{1}.adx", Path.GetFileNameWithoutExtension(pPath), fileCount.ToString("X8"));
                        outputFilePath = Path.Combine(outputPath, outputFileName);

                        ParseFile.ExtractChunkToFile(fs, offset, (int)fileSize, outputFilePath, true, true);

                        fileCount++;
                    }

                    offset += 1;
                }
            }
        }    
    }
}