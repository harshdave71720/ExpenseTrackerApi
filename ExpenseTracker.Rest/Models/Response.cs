using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExpenseTracker.Rest.Models
{
    public class Response
    {
        [JsonProperty(Order = 1)]
        public int StatusCode { get; set; }

        [JsonProperty(Order = 2)]
        public object Data { get; set; }

        [JsonProperty(Order = 3)]
        public IEnumerable<string> Errors { get; set; }

        public Response(int statusCode)
        {
            this.StatusCode = statusCode;
        }

        public Response(int statusCode, object data) : this(statusCode)
        { 
            this.Data = data;
        }

        public Response(int statusCode, IEnumerable<string> errors) : this(statusCode)
        { 
            this.Errors = errors;
        }
    }
}
