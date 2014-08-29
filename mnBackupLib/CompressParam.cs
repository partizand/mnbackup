using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SevenZip;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    [DataContract]
    public class CompressParam
    {
        /// <summary>
        /// Формат архива
        /// </summary>
        [DataMember]
        public SevenZip.OutArchiveFormat Format;
        /// <summary>
        /// Уровень сжатия
        /// </summary>
        [DataMember]
        public SevenZip.CompressionLevel Level;
        /// <summary>
        /// Алгоритм сжатия
        /// </summary>
        [DataMember]
        public SevenZip.CompressionMethod Method;
        /// <summary>
        /// Размер тома. 0-не делить на тома
        /// </summary>
        [DataMember]
        public int VolumeSize;
        /// <summary>
        /// Создает параметры по умолчанию
        /// </summary>
        public CompressParam()
        {
            Format = SevenZip.OutArchiveFormat.SevenZip;
            Level = SevenZip.CompressionLevel.Normal;
            Method = SevenZip.CompressionMethod.Lzma;
            VolumeSize = 0;
        }
    }
}
