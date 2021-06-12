using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Shimakaze.Framework;
using Shimakaze.Framework.Brushes;
using Shimakaze.Framework.Controls;
using Shimakaze.LogTracer;

namespace Shimakaze.Framework.Sample.UI
{
    public class MainWindow : Window
    {
        protected override void LoadContent()
        {
            App.Current!.Content!.RootDirectory = Path.Combine("Assets");
            App.Current.FontSystem!.AddFont(App.Current.Content.RootDirectory, "Fonts", "Roboto-Regular.ttf");
            App.Current.FontSystem!.AddFont(App.Current.Content.RootDirectory, "Fonts", "simsun.ttf");
            var texture = App.Current!.Content.Load<Texture2D>("Images", "background.png");

            Child = new StackPanel(new Control[]{
                new StackPanel(){
                    new TextBlock("这里是第一个输入框")
                    {
                        Size = new(700, 32),
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Color = Color.Red,
                    },
                    new Border(new TextBox()
                    {
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Color = Color.Blue,
                        Size = new(700, 32),
                    })
                    {
                        Thickness = new(2),
                        BorderColor = Color.Red,
                    },
                    new TextBlock("这里是第二个输入框")
                    {
                        Size = new(700, 32),
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Color = Color.Red,
                    },
                    new Border(new TextBox()
                    {
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Size = new(700, 32),
                        Color = Color.Blue,
                    })
                    {
                        Thickness = new(2),
                        BorderColor = Color.Red,
                    },
                    new TextBlock("这里是第三个输入框, 且这个没有启用输入法")
                    {
                        Size = new(700, 32),
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Color = Color.Red,
                    },
                    new Border(new TextBox()
                    {
                        FontSystem = App.Current.FontSystem,
                        FontSize=34,
                        Size = new(700, 32),
                        Color = Color.Blue,
                        UseIME = false
                    })
                    {
                        Thickness = new(2),
                        BorderColor = Color.Red,
                    }
                },
            })
            {
                Texture = texture.ToBrush(ImageMode.UniformFill)
            };
        }
    }
}
