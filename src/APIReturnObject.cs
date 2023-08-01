using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow
{
    public class APIReturnObject
    {
        public int Code { get; set; }
        public string Title
        {
            get
            {
                //Only common status codes are specified
                if (Code == 200)
                    return "Ok";
                else if (Code == 201)
                    return "Created";
                else if (Code == 204)
                    return "No Content";
                else if (Code == 400)
                    return "Bad Request";
                else if (Code == 401)
                    return "Unauthorized";
                else if (Code == 404)
                    return "Not Found";
                else if (Code >= 500)
                    return "Internal Server Error";
                else
                    return "Other statuses";
            }
        }
        public string Message { get; set; }
        public object Details { get; set; }
        public List<string> ListMessage { get; set; }
    }
}
