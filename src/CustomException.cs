using System;
using System.Collections.Generic;

namespace workflow
{
    [Serializable]
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public object Details { get; set; }
        public List<string> ListMessage { get; set; }
        public CustomException(string message, int statuscode, object details = null, List<string> listmessage = null)
            : base(message) 
        {
            StatusCode = statuscode;
            Details = details;
            ListMessage = listmessage;
        }
    }
}
