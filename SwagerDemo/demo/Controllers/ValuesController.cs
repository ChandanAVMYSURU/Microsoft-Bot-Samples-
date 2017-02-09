using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace demo.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]

        public string Get(int id)
        {
            return "value";
        }
        [SwaggerOperation("Post")]

        // POST api/values
        public void Post([FromBody]string value)
        {
        }
        [SwaggerOperation("Put")]

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [SwaggerOperation("Delete")]

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
