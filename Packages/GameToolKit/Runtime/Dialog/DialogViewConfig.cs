using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit.Dialog
{
    public class DialogViewConfig : SingletonSO<DialogViewConfig>
    {
        public List<string> DialogBoxEnums;
        public List<string> OptionViewEnums;
    }
}
