using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
{
    public enum RequestType : byte
    {
        Create = 0x00,
        DataTable = 0x01,
        PaginationDataTable = 0x02,
        ListingSimple = 0x03
    };
    public class HttpBodyReader : BinaryReader
    {
        public HttpBodyReader(Stream stream) : base(stream)
        {

        }
        public RequestType GetRequestType() => (RequestType)ReadByte();
    }
}
