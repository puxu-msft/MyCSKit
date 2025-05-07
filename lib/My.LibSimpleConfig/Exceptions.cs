using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace My
{

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(string message) : base(message) { }
    }

}
