using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _project_pdf_
{
    public partial class password_needed_form : Form
    {
        public string password;
      
        public password_needed_form(string file_name)
        {
            
            InitializeComponent();
            label1.Text = file_name + " is password protected. Please enter password";
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            if (this.pwdtxtbx.Text.Length > 0)
            {
                password = this.pwdtxtbx.Text;

            }
            else
                password = null;
            this.Close();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            password = null;
            this.Close();
        }

        public string getpasswrod()
        {
            return password;
        }
    }
}
