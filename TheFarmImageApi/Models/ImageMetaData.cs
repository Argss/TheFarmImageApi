using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFarmImageApi.Models
{

    public class CreateImageMetaData
    {
        public string UserName { get; set; }
        public string ImageFileName { get; set; }
    }

    public class ImageMetaData
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ImageFileName { get; set; }
    }
}
