using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;


namespace EvelatorError
{
    public class filterModel
    {
        public int? PostCode { get; set; }    
        public  string Street { get; set; }
        public  int? Number { get; set; }
        public string Locality { get; set; }
        //time stamp
    }

    [RoutePrefix("api")]
    public sealed class LiftApiController : ApiController
    {
        
        [Route("getErrors")]
        [HttpGet]
        public HttpResponseMessage GetErrors()
        {
            HttpResponseMessage response = new HttpResponseMessage();

            //load from db
            List<ErrorAndEvelator> errors = Database.GetAllErrors();
            //Database return error - http status 200 - OK
            if (errors != null)
            {
                response.StatusCode = HttpStatusCode.OK;
             //   var jsonContent = JsonConvert.SerializeObject(errors);
             //  var jsonSerialiser = new JavaScriptSerializer();
               // var jsonContent = jsonSerialiser.Serialize(errors);
                var jsonContent = JsonConvert.SerializeObject(errors);
                response.Content = new StringContent(jsonContent);
            }
            else //Failed load error from database - http status 400 - Bad request
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("Error when loading from DB");
            }
         
         
            return response;
        }

        [Route("getFilterErrors")]
        [HttpPost]
        public HttpResponseMessage GetFilterErrors(filterModel filter)
        {
           
            HttpResponseMessage response = new HttpResponseMessage();
           
            List<ErrorAndEvelator> errors = Database.GetAllFilterErrors(filter);
            if (errors != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                var jsonContent = JsonConvert.SerializeObject(errors);
                response.Content = new StringContent(jsonContent);
            }
            else
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("Error when loading from DB");
            }
            return response;
        }

    }
}
