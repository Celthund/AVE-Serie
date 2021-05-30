using System.Reflection;

namespace FireMapper
{
    public class ComplexPropertyGetter : AbstractGetter
    {

        PropertyInfo property;
        /*
        Constructor
        */
        public ComplexPropertyGetter(PropertyInfo property, IDataMapper db) : base(property.Name, db)
        {
            this.property = property;
        }
        /*
        Get value of the obj stored in db collection
        */
        public override object GetValue(object obj)
        {
            return db.GetById(obj);
        }
        /*
        Get default value
        */
        public override object GetDefaultValue()
        {
            return null;
        }
        /*
        Get the value of the key Getter 
        */
        public override object GetKeyValue(object obj)
        {
            return db.GetFireKey().GetValue(property.GetValue(obj));    
        }
    }
}