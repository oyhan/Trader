using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDrive
{
    public interface  IAppForm 
    {
        public string FormName { get; }
    }

    public  class BaseAppForm : Form,IAppForm
    {
        public virtual string FormName { get; }

    }
}
