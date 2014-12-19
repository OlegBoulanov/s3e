using System;
using System.Collections.Generic;
using System.Text;

namespace s3.Options
{
    class Yes : Option
    {
        static bool yesToAll = false;

        public Yes()
        {
            yesToAll = true;
        }

        static internal bool Confirm(string prompt)
        {
            Console.Error.Write(prompt);
            if (yesToAll)
            {
                Console.Error.WriteLine(" ? all");
                return true;
            }
            else
            {
                Console.Error.Write(" ? [yes/No/all] ");
                switch(Console.ReadLine().ToLower())
                {
                    case "y":
                    case "ye":
                    case "yes": return true;
                    case "n":
                    case "no": return false;
                    case "a":
                    case "al":
                    case "all": return yesToAll = true;
                }
                return false;
            }
        }
    }
}
