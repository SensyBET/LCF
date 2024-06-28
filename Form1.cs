using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LCF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Redirige la sortie de la console
            TextWriter writer = new TextBoxWriter(richTextBox2);
            Console.SetOut(writer);
            Console.SetError(writer);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Bienvenue dans LCF");
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }


    public class TextBoxWriter : TextWriter
    {
        private readonly RichTextBox _output;
        private readonly StringBuilder _buffer;

        public TextBoxWriter(RichTextBox output)
        {
            _output = output;
            _buffer = new StringBuilder();
            _output.HandleCreated += Output_HandleCreated;
        }

        private void Output_HandleCreated(object sender, EventArgs e)
        {
            // Lorsque le handle est créé, déchargez le buffer
            if (_buffer.Length > 0)
            {
                _output.Invoke(new Action(() => _output.AppendText(_buffer.ToString())));
                _buffer.Clear();
            }
        }

        public override void Write(char value)
        {
            if (_output.IsHandleCreated)
            {
                _output.Invoke(new Action(() => _output.AppendText(value.ToString())));
            }
            else
            {
                _buffer.Append(value);
            }
        }

        public override void Write(string value)
        {
            if (_output.IsHandleCreated)
            {
                _output.Invoke(new Action(() => _output.AppendText(value)));
            }
            else
            {
                _buffer.Append(value);
            }
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
