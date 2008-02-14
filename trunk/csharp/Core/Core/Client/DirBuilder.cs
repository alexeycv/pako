using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Core.Client
{
    public class DirBuilder
    {


        public DirBuilder()
        {

        }

        public string b(params string[] parts)
        {
            string data = "";
            int i = 0;
            foreach (string part in parts)
            {
                i++;
                if (i != 1)
                    data += Path.DirectorySeparatorChar + part;
                else
                    data += part;
            }
            return data;
        }
    }
}
