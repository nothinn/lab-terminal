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

        StreamWriter stream;
        public Logging()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image | *.jpg | Bitmap Image | *.bmp | Gif Image | *.gif";
            saveFileDialog.Title = "Choose where to save log";
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ShowDialog();

            Console.WriteLine("Save dialog shown");
            Console.WriteLine(saveFileDialog.FileName);

            stream = new StreamWriter(saveFileDialog.OpenFile());
            Open = true;
        }

        ~Logging()
        {
            file.Dispose();
            Open = false;
        }

        public void AddLine(string line)
        {
            stream.WriteLine(line);
        }

        public bool HasLog()
        {
            if (file != null)
            {
                return file.CanWrite;
            }
            return false;
        }
    }
}
