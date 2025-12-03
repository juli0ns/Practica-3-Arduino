using System;
using System.IO.Ports;
using System.Media;
using System.Windows.Forms;

namespace Practica_3_Arduino
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool isBlinking;
        
        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort("COM5", 9600);
            serialPort.DataReceived += DataReceivedHandler;

            button1.Click += button1_Click;

            timer1.Interval = 500; // Intervalo de parpadeo en milisegundos
            timer1.Tick += timer1_Tick;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            base.OnFormClosing(e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                }
                catch
                {
                    MessageBox.Show("Error: No se pudo abrir el puerto COM.\nRevisa cuál COM usa tu Arduino.");
                }
            }
        }

       private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string linea = serialPort.ReadLine().Trim();

                // FILTRO: aceptar solo 0 o 1
                if (linea != "0" && linea != "1")
                    return;

                this.Invoke(new Action(() =>
                {
                    label1.Text = "Sensor Status: " + (linea == "1" ? "Water Detected" : "No Water Detected");

                    if (linea == "1")
                    {
                        if (!isBlinking)
                        {
                            timer1.Start();
                            pictureBox1.BackColor = System.Drawing.Color.Red;
                            isBlinking = true;
                        }
                    }
                    else
                    {
                        timer1.Stop();
                        pictureBox1.BackColor = System.Drawing.Color.Gray;
                        isBlinking = false;
                    }
                }));
            }
            catch
            {
                // Ignorar errores por líneas incompletas
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.BackColor =
       pictureBox1.BackColor == System.Drawing.Color.Gray
       ? System.Drawing.Color.Yellow
       : System.Drawing.Color.Gray;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort.Close();

            Application.Exit();
        }
    }
}