using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Controllers
{
    public class DocumentRepo
    {
        List<Document> docs = new List<Document>();

        public DocumentRepo()
        {
            var jsonData = "[{\"id\": 1,\"name\": \"document1\",\"createdby\":\"chandan\",\"info\": \"document 1\"},{\"id\": 2,\"name\": \"doc2\",\"createdby\":\"avc\",\"info\": \"document 2\"},{\"id\": 3,\"name\": \"doc3\",\"createdby\":\"scr\",\"info\": \"document 3\"}]";
            docs = JsonConvert.DeserializeObject<List<Document>>(jsonData);
        }

        public IEnumerable<Document> Get()
        {
            return docs;
        }

        // GET api/values/5
        public Document Get(int Id)
        {
            return docs.Find(i => i.id == Id);
        }

        public Document Get(string name)
        {

            return docs.Find(i => i.name == name);
        }
    }
}