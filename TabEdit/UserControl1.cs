using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace TabEdit
{
    public partial class UserControl1 : UserControl
    {
        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        TextStyle GrayStyle = new TextStyle(Brushes.DarkGray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        public UserControl1()
        {
            InitializeComponent();
            tb = new FastColoredTextBox();
            this.Dock = DockStyle.Fill;
            this.AutoSize = true;
            tb.AutoSize = true;
            tb.Dock = DockStyle.Fill;
            this.Controls.Add(tb);

            tb.TextChanged += Tb_TextChanged;
            tb.SelectionChangedDelayed += Tb_SelectionChangedDelayed;

            tb.Range.SetStyle(new ReadOnlyStyle(), @"[\s\w]+: ");
        }

        private void Tb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            tb.VisibleRange.ClearStyle(SameWordsStyle);
            if (!tb.Selection.IsEmpty)
            {
                return;
            }

            //get fragment around caret
            var fragment = tb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
            {
                return;
            }
            //highlight same words
            var ranges = tb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
            if (ranges.Length > 1)
            {
                foreach (var r in ranges)
                {
                    r.SetStyle(SameWordsStyle);
                }
            }
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            //tb.ClearStylesBuffer();

            tb.LeftBracket = '{';
            tb.RightBracket = '}';
            tb.LeftBracket2 = '[';
            tb.RightBracket2 = ']';
            Range range = (sender as FastColoredTextBox).VisibleRange;
            range.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);
            range.SetStyle(MagentaStyle, @"\b""?(true|false|null)""?\b");
            range.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            range.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            

            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}");//allow to collapse brackets block

        }

 

        FastColoredTextBox tb;

        public void setContent(string text)
        {
            this.tb.Text = text;
            Tb_TextChanged(tb, null);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
