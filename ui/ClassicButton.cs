using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RealPropertySystemApp.ui
{
    public class ClassicButton : Button
    {

        public ClassicButton(string text, ResourceDictionary parent)
        {
            Content = text;
            Style = (Style)parent["SwitchButtonStyle"];
            Resources = parent;
        }

        
    }
}
