using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace MordochkaProg.EF
{
    public partial class Client
    {
        public Nullable<System.DateTime> LastVisit
        {
            get
            {
                var result = ClientService
                    .OrderBy(r => r.StartTime)
                    .LastOrDefault();
                if (result is null)
                    return null;
                return result.StartTime;
            }
        }

        public int VisitsCount
        {
            get
            {
                return ClientService.Count;
            }
        }

        public List<TextBlock> TagsView
        {
            get
            {
                var result = new List<TextBlock>();
                foreach (var tag in Tag)
                {
                    var brushConverter = new BrushConverter();
                    var brush = (Brush)brushConverter.ConvertFromString("#" + tag.Color);
                    var color = (Color)ColorConverter.ConvertFromString("#" + tag.Color);
                    var tb = new TextBlock()
                    {
                        Text = tag.Title,
                        Background = brush,
                        Foreground = (color.R + color.G + color.B) / 3 < 127 ? Brushes.White : Brushes.Black,
                        Padding = new System.Windows.Thickness(3)

                    };
                    result.Add(tb);
                }
                return result;
            }
        }

        public string GetImgSrc {
            get
            {
                var crDir = new DirectoryInfo(Environment.CurrentDirectory);
                if (!crDir
                    .Parent.Parent
                    .GetDirectories()
                    .Select(d => d.Name).Contains("Clients"))
                {
                    return null;
                }
                return crDir
                    .Parent.Parent.FullName + "\\" + PhotoPath;
            }
        }
    }
}
