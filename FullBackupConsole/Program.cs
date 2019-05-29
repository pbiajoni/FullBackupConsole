using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FullBackupConsole
{
    class Program
    {

        //-sourceC://Caminho/do/diretorio/raiz
        //-targetD://Caminho/do/diretorio/destino
        //-z para zipar
        //-ignore[Pasta1,Pasta2,Pasta3]
        //-ignoreext[exe,mp3,mp4]
        static string log = "";
        static List<DirectoryInfo> directoriesToCopy = new List<DirectoryInfo>();
        static void Main(string[] args)
        {
            if (!ArgExists("-source", args))
            {
                throw new Exception("É necessário o argumento -sourceC://Caminho/do/diretorio/raiz");
            }

            if (!ArgExists("-target", args))
            {
                throw new Exception("É necessário o argumento -targetD://Caminho/do/diretorio/destino");
            }

            string rootPath = GetArg("-source", args);
            string targetPath = GetArg("-target", args);
            string ignoredFolders = GetArg("-ignore", args);

            foreach (string dir in Directory.GetDirectories(rootPath))
            {
                if (!IsIgnoredFolder(dir, args))
                {
                    directoriesToCopy.Add(new DirectoryInfo(Path.Combine(rootPath, dir)));
                }
            }

            string targetBackupPath = GetBackupFolderName(targetPath);

            foreach (DirectoryInfo directoryInfo in directoriesToCopy)
            {
                Copy(directoryInfo.FullName, targetPath,args);
            }
        }

        static void Log(string text)
        {
            log += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - " + text + Environment.NewLine;
            Console.WriteLine(text);
        }

        static bool IsIgnoredFolder(string folderName, string[] args)
        {
            if (ArgExists("-ignore", args))
            {
                string ign = GetArg("-ignore", args);
                ign = ign.Replace("[", "").Replace("]", "");
                string[] igns = ign.Split(',');

                foreach (string folder in igns)
                {
                    if (folderName.ToLower().Trim() == folder.ToLower().Trim())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsIgnoredFile(string fileName, string[] args)
        {
            if (ArgExists("-ignoreext", args))
            {
                string ign = GetArg("-ignoreext", args);
                ign = ign.Replace("[", "").Replace("]", "");
                string[] igns = ign.Split(',');

                foreach (string fileext in igns)
                {
                    string[] ext = fileName.Split('.');

                    if (ext.Length == 2)
                    {
                        if (ext[1].ToLower().Trim() == fileext.ToLower().Trim())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static string GetBackupFolderName(string targetPath)
        {
            DateTime today = DateTime.Now;
            string folderName = today.Day + "_" + today.Month + "_" + today.Year;
            string path = Path.Combine(targetPath, folderName);
            string hourPath = "";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                hourPath = Path.Combine(path, (today.Hour + "" + today.Minute + "" + today.Second));
                Directory.CreateDirectory(hourPath);
            }

            return hourPath;
        }

        static string ToSize(long source)
        {
            const int byteConversion = 1024;
            double bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3)) //GB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            else if (bytes >= byteConversion) //KB Range
            {
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
            }
            else //Bytes
            {
                return string.Concat(bytes, " Bytes");
            }
        }

        static string GetArg(string param, string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Contains(param))
                {
                    return arg.Replace(param, "");
                }
            }

            return null;
        }

        static bool ArgExists(string param, string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Contains(param))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Copy(string sourceDirectory, string targetDirectory, string[] args)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget, args);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, string[] args)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string fullFileName = fi.Name + "." + fi.Extension;

                if (!IsIgnoredFile(fullFileName, args))
                {
                    Console.WriteLine(@"Copiando {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                else
                {

                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, args);
            }
        }
    }
}
