using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class Casecontroller :ApiController
    {
       static readonly ValuesController obj = new ValuesController();

        public HttpResponseMessage GetAll() {
            List<Case> cases = obj.Get().ToList();
            return Request.CreateResponse<List<Case>>(HttpStatusCode.OK,cases);
        }

        public HttpResponseMessage Get(int id)
        {
            Case cas = obj.Get(id);
            if (cas == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Case Not found for the Given ID");
            }

            else
            {
                return Request.CreateResponse<Case>(HttpStatusCode.OK,cas);
            }
        }


        public HttpResponseMessage Get(string name)
        {
            Case cas = obj.Get(name);
            if (cas == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Case Not found for the Given Name");
            }

            else
            {
                return Request.CreateResponse<Case>(HttpStatusCode.OK, cas);
            }
        }

        public HttpResponseMessage Post(Case newCaseObj)
        {
            
            if (!obj.Add(newCaseObj))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error occured while adding");

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        public HttpResponseMessage Delete(int id) {
            obj.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        public HttpResponseMessage Put(Case o)
        {
            obj.Put(o);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


    }
}