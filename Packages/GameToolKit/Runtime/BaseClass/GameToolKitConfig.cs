using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    public class GameToolKitConfig : SingletonSO<GameToolKitConfig>
    {
        public Dictionary<string, List<Type>> TypeGroup = new Dictionary<string, List<Type>>();
    }
}
