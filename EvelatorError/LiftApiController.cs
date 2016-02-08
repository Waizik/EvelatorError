using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
//using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;


namespace EvelatorError
{
  

    [RoutePrefix("api")]
    public sealed class LiftApiController : ApiController
    {
       
        
        [Route("getErrors")]
        [HttpGet]
        public HttpResponseMessage GetErrors()
        {
            HttpResponseMessage response = new HttpResponseMessage();

            //load from db
            List<ErrorMessageAndEvelator> errors = Database.GetAllErrors();
            //Database return error - http status 200 - OK
            if (errors != null)
            {
                response.StatusCode = HttpStatusCode.OK;
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
           List<ErrorMessageAndEvelator> errors = Database.GetAllFilterErrors(filter);
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

        
       [Route("updateEvelator")]
       [HttpPost]
       public HttpResponseMessage UpdateEvelator(filterModel filter)
       {
           HttpResponseMessage response = new HttpResponseMessage();
           try
           {
               Database.UpdateEvelator(filter);
               response.StatusCode = HttpStatusCode.OK;
           }
           catch (UpdateException e) //rozdelit podle vyjimek a vyzkouset.
           {
               response.StatusCode = HttpStatusCode.BadRequest;
               response.Content = new StringContent(e.message);
           }
           catch (Exception e)
           {
               response.StatusCode = HttpStatusCode.BadRequest;
               response.Content = new StringContent(e.Message);
           }
           return response;
       }

       [Route("getIdInfo")]
       [HttpPost]
       public HttpResponseMessage GetIdInfo([FromBody] int id)
       {
           HttpResponseMessage response = new HttpResponseMessage();
           ErrorMessageAndEvelator error  = Database.GetIdError(id);
           if (error != null)
           {
               response.StatusCode = HttpStatusCode.OK;
               var jsonContent = JsonConvert.SerializeObject(error);
               response.Content = new StringContent(jsonContent);
           }
           else
           {
               response.StatusCode = HttpStatusCode.Forbidden;
               response.Content = new StringContent("Error when loading from DB");
           }
           return response;
       }
    }
}
