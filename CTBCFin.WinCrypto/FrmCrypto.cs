using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ctbcfin.core.util;

namespace ctbcfin.winCrypto
{
    public partial class FrmCrypto : Form
    {
        public FrmCrypto()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearBoxs();
        }

        private void ClearBoxs()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Trim() == string.Empty)
                {
                    string message = "[來源文字]欄位無資料，請檢查。";
                    string title = ((Button)sender).Text;
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning); // for Warning
                    return;
                }

                textBox2.Text = string.Empty;
                string encryptTxt = CTBCFinCryptUtil.EncryptText(textBox1.Text.Trim());
                textBox2.Text = encryptTxt;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string title = ((Button)sender).Text;
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Trim() == string.Empty)
                {
                    string message = "[來源文字]欄位無資料，請檢查。";
                    string title = ((Button)sender).Text;
                    MessageBox.Show(message, title,  MessageBoxButtons.OK,  MessageBoxIcon.Warning ); // for Warning
                    return;
                }

                textBox2.Text = string.Empty;
                string encryptTxt = CTBCFinCryptUtil.DecryptText(textBox1.Text.Trim());
                textBox2.Text = encryptTxt;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string title = ((Button)sender).Text;
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() != string.Empty)
            Clipboard.SetText(textBox2.Text.Trim());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = ToHex(13059);
        }

        public string ToHex(int value)
        {
            return String.Format("0x{0:X}", value);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(textBox1.Text.Trim()));
            textBox2.Text = encodedStr;
        }
    }
}
