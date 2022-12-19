using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Serialization.Abstraction
{
    public class SerializationFailure
    {
        public SerializationFailure(string message)
        {
            Message = message;
        }


        public SerializationFailure() : this(string.Empty)
        {

        }

        public SerializationFailure(Exception ex) : this(ex.Message)
        {

        }

        public string Message { get; init; }
    }
}