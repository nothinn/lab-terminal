using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_terminal
{
    class Logging
    {
        FileStream file;

        public static bool Open = false;

        static StreamWriter stream;
        public Logging()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "text file | *.txt | Bitmap Image | *.bmp | Gif Image | *.gif";
            saveFileDialog.Title = "Choose where to save log";
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ShowDialog();

            Console.WriteLine("Save dialog shown");
            Console.WriteLine(saveFileDialog.FileName);
            if (stream != null)stream.Close();
            stream = new StreamWriter(saveFileDialog.FileName);
            
            Open = true;
            

        }

        ~Logging()
        {
            if (stream != null) stream.Dispose();
            Open = false;
        }

        public void AddLine(string line)
        {
            stream.WriteLine(line);
            stream.Flush();
        }

        public bool HasLog()
        {
            if (file != null)
            {
                return file.CanWrite;
            }
            return false;
        }

        public void Stop()
        {
            if (stream != null) stream.Dispose();
        }
    }
}
