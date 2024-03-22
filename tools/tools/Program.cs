using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Options:\n");
                Console.WriteLine("[1] Merge PDF files");
                Console.WriteLine("[2] Compress a PDF file");
                Console.WriteLine("[3] Compress all PDF files");
                Console.WriteLine("[4] Resize/Convert images to PDF files\n");
                Console.Write("Select an option [1/2/3/4], select [q] to exit: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.Write("Write the path where PDF files are located: ");
                        string pdfFilesPath = Console.ReadLine();
                        Console.WriteLine($"Searching PDF files inside {pdfFilesPath}");
                        List<string> listPdfFiles = GetAllPdfFilesAsList(pdfFilesPath);
                        Console.WriteLine(string.Join(Environment.NewLine, listPdfFiles));
                        Console.Write("Write the name of the final file: ");
                        string outputFile = Console.ReadLine();
                        Console.WriteLine($"All files in {listPdfFiles} will be merged into {outputFile}");
                        MergePdfFiles(outputFile, listPdfFiles);
                        break;

                    case "2":
                        Console.Write("Write the output file name, Example:[sample_compressed.pdf]: ");
                        string outPutFile = Console.ReadLine();
                        Console.Write("Write the path of the file that you want to compress, Example:sample.pdf: ");
                        string filename = Console.ReadLine();
                        CompressPdf(outPutFile, filename);
                        break;

                    case "3":
                        Console.Write("Write the path where your PDF files are located, Example:[Desktop/pdf_files/]: ");
                        string filesPath = Console.ReadLine();
                        CompressAllPdf(filesPath);
                        break;

                    case "4":
                        Console.Write("Write the path where the images are located, Example:[/path/to/images/]: ");
                        string imgPath = Console.ReadLine();
                        List<string> images = GetImages(imgPath);
                        Console.WriteLine("################################################");
                        foreach (string img in images)
                        {
                            string pdfConvertedFile = ConvertImageToPdf(Path.Combine(imgPath, img));
                            Console.WriteLine($"Conversion Complete! File {img} converted to file {pdfConvertedFile}");
                        }
                        Console.WriteLine("################################################");
                        break;

                    case "q":
                        Console.WriteLine("Goodbye");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }


        static List<string> GetImages(string path)
        {
            List<string> imgList = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach (FileInfo file in folder.GetFiles())
            {
                string fileName = file.Name;
                string extension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                if (extension.Equals("jpg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals("png", StringComparison.OrdinalIgnoreCase))
                {
                    imgList.Add(fileName);
                }
            }
            return imgList;
        }

        static List<string> GetAllPdfFilesAsList(string path)
        {
            List<string> pdfs = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach (FileInfo file in folder.GetFiles())
            {
                string fileName = file.Name;
                string extension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                if (extension.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    pdfs.Add(fileName);
                }
            }
            return pdfs;
        }

        static string ConvertImageToPdf(string imagePath)
        {
            string file = "";
            try
            {
                Bitmap image = new Bitmap(imagePath);
                string fileName = Path.GetFileNameWithoutExtension(imagePath) + ".pdf";
                Console.WriteLine($"New image to process, {image.Width}x{image.Height} in size");
                Console.Write("Do you want to resize? [y/n]: ");
                string answer = Console.ReadLine();
                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Bitmap newImage = ResizeImage(image, fileName);
                    Console.WriteLine($"Image size = {newImage.Width}x{newImage.Height}");
                    file = fileName;
                }
                else
                {
                    Console.WriteLine($"Image size = {image.Width}x{image.Height}");
                    image.Save(fileName, ImageFormat.Png);
                    file = fileName;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return file;
        }

        static Bitmap ResizeImage(Bitmap image, string fileName)
        {
            Console.Write("Write the new width: ");
            int newW = Convert.ToInt32(Console.ReadLine());
            Console.Write("Write the new height: ");
            int newH = Convert.ToInt32(Console.ReadLine());
            Bitmap resizedImage = new Bitmap(newW, newH);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, newW, newH);
            }
            resizedImage.Save(fileName, ImageFormat.Jpeg);
            return resizedImage;
        }

        static void MergePdfFiles(string outputFile, List<string> filesToMerge)
        {
            string files = string.Join(" ", filesToMerge);
            string script = $"gs -dBATCH -dNOPAUSE -q -sDEVICE=pdfwrite -dAutoRotatePages=/None -sOutputFile={outputFile} {files}";
            Process.Start("CMD.exe", "/C " + script);
        }

        static void CompressPdf(string outputFile, string fileToCompress)
        {
            string script = $"gs -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -sNAME=setting -sOutputFile={outputFile} {fileToCompress}";
            Process.Start("CMD.exe", "/C " + script);
        }

        static void CompressAllPdf(string path)
        {
            List<string> files = GetAllPdfFilesAsList(path);
            foreach (string file in files)
            {
                string fileFullPath = Path.Combine(path, file);
                Console.WriteLine($"File path: {fileFullPath}");
                CompressPdf("", fileFullPath);
            }
            Console.WriteLine("All files Compressed");
            Console.WriteLine("ls -alt *_compressed.pdf");
        }
    }
}
