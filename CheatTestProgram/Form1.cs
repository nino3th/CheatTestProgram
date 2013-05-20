/*
 *  LiteON-Module Team 
 *  
 *  Copyright (c)  NinoLiu\LiteON , Inc 2013
 * 
 *  Description:
 *    In order to avoid to deal repeatitive behavior, you can use this program to arrang a simple script
 *    then polling except pressure test on the existed tool.
 * 
 * ======================================================================================================
 * History
 * ----------------------------------------------------------------------------------------------------
 * 201305015 | NinoLiu  | 1.0.0  | Released for user test.
 * ----------------------------------------------------------------------------------------------------
 * 20130520  | NinoLiu  | 1.0.1  | Add Trackbar item that can modify the transparency of Form, and improve
 *                                 problem of message of messsagebox show repeatly.
 * ----------------------------------------------------------------------------------------------------
 * 20130520  | NinoLiu  | 1.0.2  | Add status strip to show cursor poition.
 * ----------------------------------------------------------------------------------------------------
 * ======================================================================================================
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CheatTestProgram
{
    public partial class CheatClick : Form
    {
        public CheatClick()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpstring, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
       
        private const int wm_close = 0x10;
        static int capture_time = 0;
        static int move_interval = 500;

        Mouse_Behavior mouse_behavior = new Mouse_Behavior();
        
        public static DateTime PauseForDelay(int Milisecond)
        {
            System.DateTime ThisMoment = System.DateTime.Now;
            System.TimeSpan Duration = new System.TimeSpan(0, 0, 0, 0, Milisecond);
            System.DateTime AfterWards = ThisMoment.Add(Duration);

            while (AfterWards >= ThisMoment)
            {
                ThisMoment = System.DateTime.Now;
            }
            return System.DateTime.Now;
        }

        static int init_handle = 0;
        IntPtr handle_old;
        public void timer1_Tick(object sender, EventArgs e)
        {            
            toolStripStatusLabel1.Text = "(" + Cursor.Position.X.ToString() + "," + Cursor.Position.Y.ToString() + ")";

            IntPtr handle = FindWindow("#32770", null);
            if (init_handle == 0)
            {
                init_handle++;
                handle_old = handle;
            }
            
            //this.richTextBox1.AppendText("handle:" + handle.ToString() + "\n");
            //if (handle.ToString() != "197954" && handle.ToString() != "0")

            if (handle.ToString() != handle_old.ToString() )
            {
                MessageBox_Killer(handle, sender, e);
            }
            handle_old = handle;
        }

        private void MessageBox_Killer(IntPtr hWnd, object sender, EventArgs e)
        {
            StringBuilder mSB = new StringBuilder();

            IntPtr txhandle = FindWindowEx(hWnd, IntPtr.Zero, "static", null);
                        
            int len = GetWindowTextLength(txhandle);    

            GetWindowText(txhandle, mSB, len + 1);

            if (hWnd.ToString() != "0" && txhandle.ToString() != "67470" && mSB.ToString() != "")
            {
                capture_time++;

                this.richTextBox1.AppendText("Message content:" + mSB.ToString() + "\n");                                   
                this.richTextBox1.AppendText("Error times:" + capture_time.ToString() + "\n");
                this.richTextBox1.ScrollToCaret();
            }
            
            //Close messagebox
            SendMessage(new HandleRef(null, hWnd), wm_close, IntPtr.Zero, IntPtr.Zero);     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mouse_behavior.ProcessToMem(textBox1.Text.ToString(), textBox2.Text.ToString());
            this.richTextBox2.AppendText("(X,Y) : (" + textBox1.Text + "," + textBox2.Text + ")\n");
            this.richTextBox2.ScrollToCaret();
            this.textBox1.Clear();
            this.textBox2.Clear();
        }//end button1

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello world!!");
        }

        private void button3_Click(object sender, EventArgs e)
        {                       
            string[] temp_array = new string[20];
            
            //In order to avoid interval time has no 
            if (this.textBox3.Text.ToString() == "")
            {
                move_interval = 3000;//default interval time
            }
            else
                move_interval = Convert.ToInt32(this.textBox3.Text.ToString());
            
            mouse_behavior.ShowData(ref temp_array);

            for (int i = 0; i < temp_array.Length; i = i + 2)
            {
                if (temp_array[i] != null)
                {
                    this.richTextBox1.AppendText("tx:" + temp_array[i] + ", ty:" + temp_array[i + 1] + "\n");
                    this.richTextBox1.ScrollToCaret();
                    Mouse_Behavior.MoveTo(temp_array[i], temp_array[i + 1]);
                    PauseForDelay(100);
                    Mouse_Behavior.LeftClick();
                }                
                else
                    break;
                PauseForDelay(move_interval);
            }    
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Opacity = Convert.ToDouble (trackBar1.Value * 0.01);
        }
    }
}
