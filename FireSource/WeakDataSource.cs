using System.Collections.Generic;

namespace FireSource
{
    public class WeakDataSource : IDataSource
    {

        // Holds a dictionary for each combination of Project Id + Collection (the combination is not case sensative so "Example" = "example")
        private static Dictionary<string, Dictionary<object, Dictionary<string, object>>> singleton = new Dictionary<string, Dictionary<object, Dictionary<string, object>>>();
        // Lock to be used so the singleton can be thread safe.
        private static readonly object padlock = new object();
        // Dictionary that holds the actual data.
        private readonly Dictionary<object, Dictionary<string, object>> db;
        private string ProjectId, Collection, Key, CredentialsPath;
        
        public WeakDataSource(string ProjectId, string Collection, string Key, string CredentialsPath)
        {
            this.ProjectId = ProjectId;
            this.Collection = Collection;
            this.Key = Key;
            this.CredentialsPath = CredentialsPath;
            // Use lock to prevent diferent threads to acess the singleton at the same time causing problems.
            lock (padlock)
            {   
                string dbString = ProjectId.ToLower() + Collection.ToLower();
                if (!singleton.ContainsKey(dbString))
                {
                    singleton[dbString] = new Dictionary<object, Dictionary<string, object>>();
                }
                db = singleton[dbString];
            }
        }

        /*
            Add document to db of the current instance.
            Throws exception when the document doesn't contain this.Key or db already contains the current obj.
        */
        public void Add(Dictionary<string, object> obj)
        {
            if (obj.ContainsKey(Key) && !db.ContainsKey(obj[Key]))
                db[obj[Key]] = obj;
            else
                throw new DocumentAlreadyExistsException();
        }

        /*
            Delete document with Key=KeyValue from db of the current instance.
        */
        public void Delete(object KeyValue)
        {
            if (db.ContainsKey(KeyValue))
                db.Remove(KeyValue);
        }

        /*
            Return all documents in db as an IEnumerable.
        */
        public IEnumerable<Dictionary<string, object>> GetAll()
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> doc in db.Values)
            {
                lst.Add(doc);
            }
            return lst;
        }

        /*
            Return the document where Key is equal to KeyValue.
        */
        public Dictionary<string, object> GetById(object KeyValue)
        {
            return KeyValue != null && db.ContainsKey(KeyValue) ? db[KeyValue] : null;
        }

        /*
            Update the document in db if it exists.
        */
        public void Update(Dictionary<string, object> obj)
        {
            if (obj.ContainsKey(Key) && db.ContainsKey(obj[Key]))
                db[obj[Key]] = obj;
            else
                throw new DocumentNotFoundException();
        }
    }
}
