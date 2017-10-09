using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace weather.Model
{
    class CurrentCity
    {
        //Get saved current City using StreamReader
        static public string GetCurrentCity()
        {
            string line = "";
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("currentCity.txt"))
                {
                    //Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                    if (line == "")
                    {
                        return "Colombo";
                    }
                    else
                    {
                        return line;
                    }
                }
            }
            catch
            {
                return "Colombo";
            }
            
        }
    }
}
