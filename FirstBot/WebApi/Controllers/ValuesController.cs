using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    
    public class ValuesController 
    {
        public static List<Case> cases = new List<Case>();
        string jsonData = "[{\"id\": 1,\"name\": \"case1\",\"info\": \"for test\"},{\"id\": 2,\"name\": \"case2\",\"info\": \"for test\"},{\"id\": 3,\"name\": \"case3\",\"info\": \"for test\"}]";

        public ValuesController() {
            
            cases = JsonConvert.DeserializeObject<List<Case>>(jsonData);

        }
     
        public IEnumerable<Case> Get()
        {
            return cases;
        }

        // GET api/values/5
        public Case Get(int caseId)
        {
            return cases.Find(i => i.id==caseId);   
        }

        public Case Get(string name) {
            
            return cases.Find(i => i.name == name);
        }

        public bool Add(Case obj) {
            cases.Add(obj);
            return true;
        }

        public void Delete(int objId) {
          
            cases.RemoveAll(s=>s.id==objId);
        }

        public void Put(Case obj) {
            ValuesController b = new ValuesController();
            Case updateObj = b.Get(obj.id);
            cases.Remove(updateObj);
            cases.Add(obj);
        }
        
    }
}
