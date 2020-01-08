using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Magic.GameEditor
{
    class XmlExportTools
    {

        #region 旧版的打开文件/文件夹框方法
       
        public static string DialogGetFolderName(string initPath)
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();

            ofd.SelectedPath = initPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.SelectedPath;
            }
            return initPath;
        }

        public static string DialogGetFileName(string initPath, string filter = "数据文件(.txt)| *.txt")
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = initPath;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return initPath;
        }

        public static string DialogSaveFileName(string initPath, string filter = "Flatbuffer数据文件(.bytes) | *.bytes")
        {
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.InitialDirectory = initPath;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return initPath;
        }
        
        #endregion


        #region 新版的打开文件/文件夹框方法
        public static string DialogGetFolderNameNew(string initPath)
        {
            return Dialog.GetFolderPath();
        }

        public static string DialogGetFileNameNew(string initPath, string filter = "txt (*.txt)")
        {
            return Dialog.OpenFileWithName(filter);
        }

        public static string DialogSaveFileNameNew(string initPath, string filter = "bytes (*.bytes)")
        {
            return Dialog.GetSaveFileName(filter);
        }

        #endregion

        #region 自处理的选择文件消息框
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class FileDlg
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public String filter = null;
            public String customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public String file = null;
            public int maxFile = 0;
            public String fileTitle = null;
            public int maxFileTitle = 0;
            public String initialDir = null;
            public String title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public String defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public String templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DirDlg
        {
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr pidlRoot = IntPtr.Zero;
            public String pszDisplayName = null;
            public String lpszTitle = null;
            public UInt32 ulFlags = 0;
            public IntPtr lpfn = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenFileDlg : FileDlg
        {
        }
        public class OpenFileDialog
        {
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetOpenFileName([In, Out] OpenFileDlg ofd);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class SaveFileDlg : FileDlg
        {
        }
        public class SaveFileDialog
        {
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetSaveFileName([In, Out] SaveFileDlg ofd);
        }


        public class FolderBrowserDialog
        {
            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern IntPtr SHBrowseForFolder([In, Out] DirDlg ofn);

            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

        }

        public class Dialog
        {
            public static  string GetSaveFileName(string fileNameUse , string flieter = "bytes (*.bytes)")
            {
                SaveFileDlg pth = new SaveFileDlg();
                pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
                pth.filter = flieter;
                pth.file = new string(new char[256]);
                pth.maxFile = pth.file.Length;
                pth.fileTitle = new string(new char[64]);
                pth.maxFileTitle = pth.fileTitle.Length;
                pth.initialDir = Application.dataPath;  // default path  
                pth.title = "保存bytes文件";
                pth.defExt = "bytes";
                pth.file = fileNameUse;
                pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
                string filepath = "";
                if (SaveFileDialog.GetSaveFileName(pth))
                {
                    filepath = pth.file;//选择的文件路径;  
                }
                return filepath;
            }

            public static string OpenFileWithName(string filter = "txt (*.txt)")
            {
                string filepath = "";
                OpenFileDlg pth = new OpenFileDlg();
                pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
                pth.filter = filter;
                pth.file = new string(new char[256]);
                pth.maxFile = pth.file.Length;
                pth.fileTitle = new string(new char[64]);
                pth.maxFileTitle = pth.fileTitle.Length;
                pth.initialDir = Application.dataPath;  // default path  
                pth.title = "打开xml";
                pth.defExt = "txt";
                pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
                if (OpenFileDialog.GetOpenFileName(pth))
                {
                    filepath = pth.file;//选择的文件路径;  
                }
                return filepath;
            }

            public  static string GetFolderPath()
            {
                DirDlg ofn2 = new DirDlg();
                ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区  
                ofn2.lpszTitle = "Open Folder";// 标题  
                IntPtr pidlPtr = FolderBrowserDialog.SHBrowseForFolder(ofn2);

                char[] charArray = new char[2000];
                for (int i = 0; i < 2000; i++)
                    charArray[i] = '\0';

                FolderBrowserDialog.SHGetPathFromIDList(pidlPtr, charArray);
                string fullDirPath = new String(charArray);

                return fullDirPath.Substring(0,fullDirPath.IndexOf('\0'));

            }
        }
        #endregion
    }
}
