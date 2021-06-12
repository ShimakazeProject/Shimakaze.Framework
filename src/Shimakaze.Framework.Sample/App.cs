using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Shimakaze.Framework;
using Shimakaze.Framework.Brushes;
using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Sample.UI;
using Shimakaze.LogTracer;

namespace Shimakaze.Framework.Sample
{
    public class App : Application
    {


        protected override void Initialize()
        {
            Window = new MainWindow();
            IsMouseVisible = true;
            GameWindow.AllowUserResizing = true;
            GameWindow.Title = "Demo";
            base.Initialize();
        }
    }
}
