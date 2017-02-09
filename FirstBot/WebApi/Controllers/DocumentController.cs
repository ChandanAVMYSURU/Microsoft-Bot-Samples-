using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class DocumentController : ApiController
    {
        DocumentRepo obj = new DocumentRepo();

        public HttpResponseMessage GetAll()
        {
            List<Document> docs = obj.Get().ToList();
            return Request.CreateResponse<List<Document>>(HttpStatusCode.OK, docs);
        }

        public HttpResponseMessage Get(int id)
        {
            Document doc = obj.Get(id);
            if (doc == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Document Not found for the Given ID");
            }

            else
            {
                return Request.CreateResponse<Document>(HttpStatusCode.OK, doc);
            }
        }

        public HttpResponseMessage Get(string name)
        {
            Document doc = obj.Get(name);
            if (doc == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Document Not found for the Given Name");
            }

            else
            {
                return Request.CreateResponse<Document>(HttpStatusCode.OK, doc);
            }
        }


    }
}