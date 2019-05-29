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
        //-ignorePasta1,Pasta2,Pasta3
        static string log = "";
        static void Main(string[] args)
        {

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
        static void Log(string text)
        {
            log += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - " + text + Environment.NewLine;
            Console.WriteLine(text);
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

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copiando {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
