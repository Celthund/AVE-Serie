using System.Collections;
using System.Collections.Generic;
namespace FireMapper
{
    public interface IDataMapper
    {
        IEnumerable GetAll();
        object GetById(object keyValue);
        void Add(object obj);
        void Update(object obj);
        void Delete(object keyValue);

        List<IGetter> GetPropertiesList();
    }
    
    
}
