using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VikiRakuten.Funciones
{
    public class Import
    {
        public static string GetUtils(string title)
        {
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos Txt (*.txt) | *.txt";
                openFileDialog.Title = title;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string text in File.ReadAllLines(openFileDialog.FileName))
                    {
                        text.Replace("\t", "");
                        text.Replace("\r", "");
                        text.Trim();
                        text.Replace(" ", "");
                    }

                    filePath = openFileDialog.FileName;
                }
            }
            return filePath;
        }

    }
}
