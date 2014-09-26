using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alphaleonis.Win32.Vss;
using System.IO;
using NLog;

namespace mnBackupLib
{
    /// <summary>
    /// Создание теневой копии тома
    /// </summary>
    /// <example>
    /// <code>
    /// using (Snapshot snap=new Snapshot("c:\\","L:"))
    /// {
    /// snap.DoSnapshotSet()
    /// //Here you have snapshot of c: mounted to L:
    /// 
    /// }
    /// </code>
    /// </example>
    public class Snapshot : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        // Flag: Has Dispose already been called?
        bool disposed = false;
        
        //IVssImplementation _vssImplementation; 
        IVssBackupComponents _backup;

        /// <summary>Metadata about this object's snapshot.</summary>
        VssSnapshotProperties _props;

        /// <summary>
        /// GUID снапшота
        /// </summary>
        Guid _snapGuid;
        /// <summary>
        /// GUID всего набора снапшотов
        /// </summary>
        Guid _setGuid;

        /// <summary>
        /// На какой диск монтируется образ
        /// </summary>
        string _MountDriveLetter;
        /// <summary>
        /// Для какого диска создается образ. Нужен слэш. Например c:\
        /// </summary>
        string _Volume;
        /// <summary>
        /// Создает объект. Снапшот не создается
        /// </summary>
        /// <param name="Volume">Для какого диска создается образ. Нужен слэш. Например c:\</param>
        /// <param name="mountDriveLetter">На какой диск монтируется образ</param>
        public Snapshot(string Volume,string mountDriveLetter)
        {
            _Volume = Volume;
            _MountDriveLetter = mountDriveLetter;
            
        }
        /// <summary>
        /// Создать снапшот
        /// </summary>
        public void DoSnapshotSet()
        {
            // VSS step 1: Initialize
            IVssImplementation _vssImplementation = VssUtils.LoadImplementation();
            _backup = _vssImplementation.CreateVssBackupComponents();
            _backup.InitializeForBackup(null);
            // VSS step 2: Getting Metadata from all the VSS writers
            _backup.GatherWriterMetadata();

            // VSS step 3: VSS Configuration
            _backup.SetContext(VssVolumeSnapshotAttributes.Persistent | VssVolumeSnapshotAttributes.NoAutoRelease);
            _backup.SetBackupState(false, true, Alphaleonis.Win32.Vss.VssBackupType.Full, false);
            // VSS step 4: Declaring the Volumes that we need to use in this beckup. 
            // The Snapshot is a volume element (Here come the name "Volume Shadow-Copy")
            // For each file that we nee to copy we have to make sure that the propere volume is in the "Snapshot Set"
            _setGuid = _backup.StartSnapshotSet();
            _snapGuid = _backup.AddToSnapshotSet(_Volume, Guid.Empty);

            // VSS step 5: Preparation (Writers & Provaiders need to start preparation)
            _backup.PrepareForBackup();
            // VSS step 6: Create a Snapshot For each volume in the "Snapshot Set"
            logger.Info("Creating snapshot for {0}",_Volume);
            _backup.DoSnapshotSet();
            /***********************************
            /* At this point we have a snapshot!
            /* This action should not take more then 60 second, regardless of file or disk size.
            /* THe snapshot is not a backup or any copy!
            /* please more information at http://technet.microsoft.com/en-us/library/ee923636.aspx
            /***********************************/

            // VSS step 7: Expose Snapshot
            /***********************************
            /* Snapshot path look like:
             * \\?\Volume{011682bf-23d7-11e2-93e7-806e6f6e6963}\
             * The build in method System.IO.File.Copy do not work with path like this, 
             * Therefor, we are going to Expose the Snapshot to our application, 
             * by mapping the Snapshot to new virtual volume
             * - Make sure that you are using a volume that is not already exist
             * - This is only for learning purposes. usually we will use the snapshot directly as i show in the next example in the blog 
            /***********************************/
            logger.Info("Exposing snapshot on {0}", _MountDriveLetter);
            _backup.ExposeSnapshot(_snapGuid, null, VssVolumeSnapshotAttributes.ExposedLocally, _MountDriveLetter);
        }
        /// <summary>
        /// Преобразование пути к файлу на путь в snapshot
        /// С путями в snapshot System.IO работать не может!
        /// </summary>
        /// <param name="localPath">Имя файла в системе</param>
        /// <returns>Имя файла в снапшоте</returns>
        public string GetSnapshotPath(string localPath)
        {
            

            // This bit replaces the file's normal root information with root
            // info from our new shadow copy.
            if (Path.IsPathRooted(localPath))
            {
                string root = Path.GetPathRoot(localPath);
                localPath = localPath.Replace(root, String.Empty);
            }
            string slash = Path.DirectorySeparatorChar.ToString();
            if (!this.Root.EndsWith(slash) && !localPath.StartsWith(slash))
                localPath = localPath.Insert(0, slash);
            localPath = localPath.Insert(0, this.Root);

            

            return localPath;
        }
        /// <summary>
        /// Преобразование списка полных путей к файлам на пути в snapshot
        /// С путями в snapshot System.IO работать не может!
        /// </summary>
        /// <param name="localPath">Имя файла в системе</param>
        /// <returns>Имя файла в снапшоте</returns>
        public string[] GetSnapshotPath(string[] localPath)
        {
            string[] path = new string[localPath.Length];
            int i;
            for (i = 0; i < localPath.Length; i++)
            {
                path[i] = GetSnapshotPath(localPath[i]);
            }
            return path;

        }

        /// <summary>
        /// Gets the string that identifies the root of this snapshot.
        /// </summary>
        public string Root
        {
            get
            {
                if (_props == null)
                    _props = _backup.GetSnapshotProperties(_snapGuid);
                if (!String.IsNullOrEmpty(_MountDriveLetter))
                {
                    return _MountDriveLetter;
                }
                else
                    return _props.SnapshotDeviceObject;
            }
        }

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this); 
            
            /*
            // VSS step 9: Delete the snapshot (using the Exposed Snapshot name)
            foreach (VssSnapshotProperties prop in _backup.QuerySnapshots())
            {
                if (prop.ExposedName == @"L:\")
                {
                    Console.WriteLine("prop.ExposedNam Found!");
                    _backup.DeleteSnapshot(prop.SnapshotId, true);
                }
            }
             */ 
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                
            }

            // Free any unmanaged objects here.
            //
            Delete();
            disposed = true;
        }

        ~Snapshot()
        {
            Dispose(false);
        }

        void Delete()
        {
            try
            {
                logger.Info("Deleting snapshot set");
                _backup.DeleteSnapshotSet(_setGuid, false);
            }
            catch (Exception e)
            {
                logger.Error("Erorr deleting snapshot {0}", e.Message);
            }
        }
    }
}
