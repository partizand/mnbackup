using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SevenZip;
using NLog;

namespace mnBackupLib
{
    /// <summary>
    /// Методы архивации
    /// </summary>
    public class Compressor
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Параметры сжатия
        /// </summary>
        CompressParam Param;

        public Compressor(CompressParam param)
        {
            Param = param;
        }

        /// <summary>
        /// Ужать файлы в архив
        /// </summary>
        /// <param name="ArhFileName"></param>
        /// <param name="files"></param>
        public bool CompressFiles(string ArhFileName, string[] files)
        {
            SevenZipCompressor cmp = new SevenZipCompressor();

            cmp.ArchiveFormat = Param.Format;
            cmp.CompressionLevel = Param.Level;
            cmp.CompressionMethod = Param.Method;
            cmp.VolumeSize = Param.VolumeSize;
            //cmp.Password = Param.Password;

            

            cmp.FileCompressionStarted += new EventHandler<FileNameEventArgs>(cmp_FileCompressionStarted);
            //cmp.CompressionFinished += new EventHandler<EventArgs>(cmp_CompressionFinished);

            try
            {
                if (String.IsNullOrEmpty(Param.Password))
                {
                    cmp.CompressFiles(ArhFileName, files);
                }
                else
                {
                    cmp.CompressFilesEncrypted(ArhFileName, Param.Password, files);
                }
                
                return true;
            }
            

            catch (Exception e)
            {
                
                logger.Error("Error compressing \"{0}\"", e.Message);
                return false;
            }

            

        }
        void cmp_FileCompressionStarted(object sender, FileNameEventArgs e)
        {
            logger.Info("Compressing \"{0}\"", e.FileName);
        }

        
    }
}
