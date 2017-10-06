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
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("currentCity.txt"))
                {
                    //Check Whether the currentCity File is empty                
                    if(String.IsNullOrWhiteSpace(Convert.ToString(sr)))
                    {
                        //If the currentCity file is empty value "Colombo" will be set to it since Colombo is assumed as the default Location
                        line = "Colombo";
                    }
                    else
                    {
                        // Read the stream to a string, and write the string to the console.
                        line = sr.ReadToEnd();
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " Unable to Read from file");
            }
            return line;
        }
    }
}
