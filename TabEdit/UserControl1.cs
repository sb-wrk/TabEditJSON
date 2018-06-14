using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;

namespace TabEdit
{
    public partial class UserControl1 : UserControl
    {
        //TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        //TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        //TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        //TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        //TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        //TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        //TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        //MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        public UserControl1()
        {
            InitializeComponent();

            textBox = new ScintillaNET.Scintilla();
            this.Controls.Add(textBox);
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.TextChanged += TextBox_TextChanged;
            //tb = new FastColoredTextBox();
            //tb.AutoSize = true;
            //tb.Dock = DockStyle.Fill;
            //this.Controls.Add(tb);
            //tb.TextChanged += Tb_TextChanged;
            InitSyntaxColoring();
        }


        private void InitSyntaxColoring()
        {
            textBox.StyleResetDefault();
            textBox.Styles[Style.Json.Default].ForeColor = Color.Silver;
            textBox.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            textBox.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            textBox.Styles[Style.Json.Number].ForeColor = Color.Olive;
            textBox.Styles[Style.Json.PropertyName].ForeColor = Color.Blue;
            textBox.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            textBox.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            textBox.Styles[Style.Json.Operator].ForeColor = Color.Purple;
            textBox.Lexer = Lexer.Json;

            textBox.Styles[Style.BraceLight].BackColor = Color.LightGray;
            textBox.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            textBox.Styles[Style.BraceBad].ForeColor = Color.Red;
            textBox.Styles[Style.Default].Font = "Consolas";
            textBox.Styles[Style.Default].Size = 10;

            textBox.UpdateUI += TextBox_UpdateUI;
        }

        private void TextBox_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            // Has the caret changed position?
            var caretPos = textBox.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(textBox.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(textBox.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = textBox.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                        textBox.BraceBadLight(bracePos1);
                    else
                        textBox.BraceHighlight(bracePos1, bracePos2);
                }
                else
                {
                    // Turn off brace matching
                    textBox.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var maxLineNumberCharLength = textBox.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            textBox.Margins[0].Width = textBox.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        int lastCaretPos = 0;
        private int maxLineNumberCharLength;


        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '[':
                case ']':
                case '{':
                case '}':
                    return true;
            }

            return false;
        }

        ScintillaNET.Scintilla textBox;

        public void setContent(string text)
        {
            this.textBox.Text = text;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
