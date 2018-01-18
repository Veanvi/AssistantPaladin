using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistantPaladin
{
    class KayKlicker
    {
        public List<Char> charList = new List<Char>();

        private bool isWork = false;

        public void KlickKays()
        {
            if (!isWork)
            {
                Task.Run(() =>
                {
                    isWork = true;
                    foreach (var kay in charList)
                    {
                        AutoIt.AutoItX.Send("{" + kay.ToString() + "}");
                    }
                    Task.Delay(1000).Wait();
                    isWork = false;
                });
            }
        }
    }
}
