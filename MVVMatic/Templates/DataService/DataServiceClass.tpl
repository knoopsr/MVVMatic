using {Namespace}.DAL.{Folder};
using {Namespace}.Model.{Folder};
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {Namespace}.DataService.{Folder}
{
    public class cls{ModelName}DataService : I{ModelName}DataService
    {
        I{ModelName}Repository Repo = new cls{ModelName}Repository();

        public bool Delete(cls{ModelName}Model entity)
        {
            return Repo.Delete(entity);
        }

        public cls{ModelName}Model Find()
        {
            return Repo.GetFirst();
        }

        public ObservableCollection<cls{ModelName}Model> GetAll()
        {
            return Repo.GetAll();
        }

        public cls{ModelName}Model GetById(int id)
        {
            return Repo.GetById(id);
        }

        public cls{ModelName}Model GetFirst()
        {
            return Repo.GetFirst();
        }

        public bool Insert(cls{ModelName}Model entity)
        {
            return Repo.Insert(entity);
        }

        public bool Update(cls{ModelName}Model entity)
        {
            return Repo.Update(entity);
        }
    }
}
