using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using iTextSharp.text.pdf.parser;

namespace _project_pdf_
{
    public partial class Form1 : Form
    {
        #region global_declaration
        OpenFileDialog ofd=new OpenFileDialog();
        FolderBrowserDialog fld = new FolderBrowserDialog();
        SaveFileDialog svd=new SaveFileDialog();
        string password;
        string file_name;
        string loadedpdf = "";
        string newpdf = "";
        ListViewItem current_li = new ListViewItem();
        ListViewItem previous_li = new ListViewItem();
        bool passwordrequired = false;
        //file_path means path+filename
        string file_path;
        int total_page;
        internal string pages;
        loading_form ldfobj;
        supporting_class scobj;
        string current_tabpage="";
        string previous_tabpage="";
        List<int>[] blank_pages;
        List<int>[] without_blank_page;

        bool state;

        
        /// <summary>
        /// Password required or not for each files is saved in listviews subitem as pages.tag subitem[1] require=1 not required=0
        /// password is saved in path.tag subitem[2]   
        /// </summary>
        enum operation_return_type { error_in_opening_file = 0, can_not_save_file = 1, user_pressed_cancel = 2 };


        #endregion global_declaration

        public Form1()
        {
            InitializeComponent();                   
        }
        public void initializer()
        {
            ofd.DefaultExt = "pdf";
            ofd.Filter = "PDF Documents (*.pdf)|*.pdf";
            ofd.Multiselect = true;
            ofd.ShowReadOnly = true;
            ofd.Title = "Add PDF Files";
            svd.Title = "Save File";
            svd.Filter = "PDF Documents (*.pdf)|*.pdf";
            fld.Description = "Select folder where you want to save";
            fld.ShowNewFolderButton = true;
            
            Label[] lbl = new Label[3];
           
            lbl[0] = new Label();
            lbl[0].Text = "Status";
            lbl[0].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl[0].AutoSize = true;
            lbl[0].Dock = DockStyle.Fill;

            
            lbl[1] = new Label();
            lbl[1].Text = "Progress";
            lbl[1].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl[1].AutoSize = true;
            lbl[1].Dock = DockStyle.Fill;

            lbl[2] = new Label();
            lbl[2].Text = "Cancel";
            lbl[2].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl[2].AutoSize = true;
            lbl[2].Dock = DockStyle.Fill;

           tableLayoutPanel1.Controls.Add(lbl[0], 0, 0);
           tableLayoutPanel1.Controls.Add(lbl[1], 1, 0);
           tableLayoutPanel1.Controls.Add(lbl[2], 2, 0);
        }

        public void show_loading_form()
        {
            ldfobj = new loading_form();
            ldfobj.ShowDialog();
            
        }

   

        private void openPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    supporting_class sc = new supporting_class();               
                    string[] inputfiles = sc.getnewfiles(ofd.FileNames,listView1);
                    if (inputfiles.Length > 0)
                    {
                        new Thread((() => add_to_system(inputfiles))).Start();
                        show_loading_form();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            { }
        }
       

        public bool add_to_listview(string f, bool pas_h_t_save, int t_page)
        {
            try
            {

                total_page = t_page;
                ListViewItem lvi = new ListViewItem(new string[] { Path.GetFileName(f), this.total_page.ToString(), f });
                if (pas_h_t_save)
                {
                    lvi.SubItems[1].Tag = "1";
                    lvi.SubItems[2].Tag = password;
                    pas_h_t_save = false;
                }
                else
                {
                    lvi.SubItems[1].Tag = "0";
                    pas_h_t_save = false;
                }
          
                if (listView1.InvokeRequired)
                {
                    listView1.Invoke((MethodInvoker)(() => listView1.Items.Add(lvi)));
                    listView1.Invoke((MethodInvoker)(() => listView1.Items[listView1.Items.IndexOf(lvi)].Selected = true));
                }
                else
                {
                    listView1.Items.Add(lvi);
                    listView1.Items[listView1.Items.IndexOf(lvi)].Selected = true;
                }
                return true;

            }

            catch (Exception ex)
            {
                
                return false;
            }
        }

     
        public bool add_to_system(string[] file_path)
        {
            PdfReader pr = null;
            bool passhavetosaved = false;

            try
            {
                foreach (string f in file_path)
                {
                password_required_statement:

                    //Open with password
                    if (passwordrequired)
                    {
                        password_needed_form pnf = new password_needed_form(Path.GetFileName(f));
                        pnf.ShowDialog();
                        password = pnf.getpasswrod();

                        if (password != null)
                        {
                            try
                            {
                                byte[] bytepassword = Encoding.UTF8.GetBytes(password);
                                pr = new PdfReader(new RandomAccessFileOrArray( f), bytepassword);
                                supporting_class sc = new supporting_class();
                               // new Thread((() => sc.GetandSetBookmark(pr, f,treeView1))).Start();
                                passhavetosaved = true;
                                passwordrequired = false;
                                add_to_listview(f, passhavetosaved, pr.NumberOfPages);
                            }
                            catch (BadPasswordException bpex)
                            {

                                MessageBox.Show("Password did not match.");
                                passwordrequired = false;
                                goto passwrod_did_not_match_statement;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not open " + Path.GetFileName(f) + ".It is Corrupted or Not wel formatted.");


                            }
                        }
                        else
                        {
                            passwordrequired = false;
                            goto passwrod_did_not_match_statement;
                        }
                    }

                    //Try to open without password
                    else
                    {
                        try
                        {
                            pr = new PdfReader( new RandomAccessFileOrArray(f),null);
                            supporting_class sc = new supporting_class();
                           // new Thread((() => sc.GetandSetBookmark(pr, f,treeView1))).Start();
                            add_to_listview(f, passhavetosaved, pr.NumberOfPages);
                        }
                        catch (BadPasswordException bpex)
                        {
                            passwordrequired = true;

                            goto password_required_statement;
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Could not open \n" + Path.GetFileName(f) + " \nFile may be Corrupted or Not wel formatted.");

                        }
                    }

                    

                passwrod_did_not_match_statement: ;

                }

                return true;
            }
            catch (Exception ex)
            {
                // show_message_box(ex.ToString());

                return false;
            }
            finally
            {
                if (!ldfobj.IsDisposed)
                {
                    ldfobj.Invoke((MethodInvoker)(() => ldfobj.Close()));
                }
            }
        }

       /* public int[] Pages
        {
            get
            {
                ArrayList ps = new ArrayList();
                if (this.pages == null || pages.Length == 0)
                {
                }
                else
                {
                    string[] ss = this.pages.Split(new char[] { ',', ' ', ';' });
                    foreach (string s in ss)
                        if (Regex.IsMatch(s, @"\d+-\d+"))
                        {
                            int start = int.Parse(s.Split(new char[] { '-' })[0]);
                            int end = int.Parse(s.Split(new char[] { '-' })[1]);
                            if (start > end)
                                return new int[] { 0 };
                            while (start <= end)
                            {
                                ps.Add(start);
                                start++;
                            }
                        }
                        else
                        {
                            ps.Add(int.Parse(s));
                        }
                }
                return ps.ToArray(typeof(int)) as int[];
            }
        }
        */

        public bool update_status(string text, int value)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void show_message_box(string text)
        {
            MessageBox.Show(text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initializer();

            state = true;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            this.listView1.Focus();
            if (this.listView1.SelectedItems.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                if (i > 0)
                {
                    ListViewItem lvi = this.listView1.Items[i - 1];
                    this.listView1.Items.RemoveAt(i - 1);
                    this.listView1.Items.Insert(i, lvi);
                }
            }
        }

        private void button_down_Click(object sender, EventArgs e)
        {
            this.listView1.Focus();
            if (this.listView1.SelectedItems.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                if (i < this.listView1.Items.Count - 1)
                {
                    ListViewItem lvi = this.listView1.Items[i + 1];
                    this.listView1.Items.RemoveAt(i + 1);
                    this.listView1.Items.Insert(i, lvi);
                }
            }
        }

        public PdfReader getreader(ListViewItem lvi)
        {
          
                if ((string)lvi.SubItems[1].Tag == "1")
                {
                    byte[] bytepassword = Encoding.UTF8.GetBytes(listView1.SelectedItems[0].SubItems[2].Tag.ToString());
                    return new PdfReader(new RandomAccessFileOrArray(lvi.SubItems[2].Text), bytepassword);
                }
                else
                {
                    return new PdfReader(new RandomAccessFileOrArray(lvi.SubItems[2].Text), null);
                    //return new PdfReader(lvi.SubItems[2].Text);
                }

          
            
           
        }

        //to remove seleted item form listview
        private void buttonRemove_Click(object sender, EventArgs e)
        {
           try
           {
                if (listView1.SelectedItems.Count > 0)
                {
                    scobj = new supporting_class();
                    PdfReader tempreader = null;
                    string temp_path = scobj.removeListViewItem(listView1, listView1.SelectedItems[0]);
                    if (temp_path != null)
                    {
                      ///  tempreader = getreader(listView1.SelectedItems[0]);
                        
                      //  scobj.GetandSetBookmark(tempreader, temp_path,treeView1);
                    }
                }
             }
            catch (Exception ex)
            {
                
               
            }
            
            
        }

        public string get_merging_file_path(string name)
        {
            if (name.Length > 20)
            {
                name = name.Remove(20);
                name = name.Replace(name, name + "...");
            }
            if (output_path_txt_bx.Text.Length > 0)
            {
                if (default_file_prefix_TxtBx.Text.Length > 0)
                {
                    file_path = output_path_txt_bx.Text + "\\" + default_file_prefix_TxtBx.Text + name + " merged.pdf";
                }
                else
                    file_path = output_path_txt_bx.Text + "\\" + name + " merged.pdf";

                if (!(checkBox_replace_existing.Checked) && File.Exists(file_path))
                {
                    svd.FileName = file_path;

                    if (svd.ShowDialog() == DialogResult.OK)
                    {
                        file_path = svd.FileName;
                        return file_path;
                    }
                    else
                    { return null; }

                }
                return file_path;
            }
            else
            {
                if (default_file_prefix_TxtBx.Text.Length > 0)
                {
                    svd.FileName = default_file_prefix_TxtBx.Text + name + " merged.pdf";
                }
                else
                    svd.FileName = name + " merged.pdf";

                if (svd.ShowDialog() == DialogResult.OK)
                {
                    file_path = svd.FileName;
                }
                else
                { return null; }

                return file_path;
            }
        }

        private void merge_btn_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 1)
            {
                splitting_and_merging_class sm = new splitting_and_merging_class();
                supporting_class scobj = new supporting_class();

                ArrayList arraylist = new ArrayList();
                string name = "";
                foreach (ListViewItem lvi in listView1.Items)
                {
                    arraylist.Add(lvi.SubItems[2].Text);
                    name = name + "_" + Path.GetFileNameWithoutExtension(lvi.SubItems[2].Text);

                }
                file_path = get_merging_file_path(name);
                if (file_path == null)
                    goto finish;
                PdfReader[] pr = new PdfReader[arraylist.Count];
                int counter = 0;
                int progressbarmaxvalue = 0;
                foreach (ListViewItem lvi in listView1.Items)
                {
                    try
                    {
                        pr[counter] = getreader(lvi);
                        progressbarmaxvalue += pr[counter].NumberOfPages;
                        counter++;
                    }
                    catch (Exception ex)
                    {

                    }

                }
       
               scobj.start_BackgroundWorker_for_merging(sender, e, tableLayoutPanel1, progressbarmaxvalue,pr, file_path  );
                

            }
            else
            {
                MessageBox.Show("Please Add atleast 2 files to merge");
            }

        finish: ;
        }

       

        private void output_path_select_btn_Click(object sender, EventArgs e)
        {
            if (fld.ShowDialog() == DialogResult.OK)
            {
                output_path_txt_bx.Text = fld.SelectedPath;
            }
        }
      
       
        public string getpath()
        {
            string path = null;
            if (output_path_txt_bx.Text.Length < 1)
            {
                if (fld.ShowDialog() == DialogResult.OK)
                {
                    path = fld.SelectedPath;
                    
                }
                else
                    path = null;
            }

            else
            {
                path = output_path_txt_bx.Text+"\\"+default_file_prefix_TxtBx.Text;
            }
            return path;
        }

    
        private void split_single_btn_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            //initialization
            supporting_class scobj = new supporting_class();           
            bool can_replace = false;
            int total_pdf=listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;
            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.Items)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed
                
                tmp = c;
               pr[c] = getreader(lvi);
               totalpage[c] = pr[c].NumberOfPages;
              
               file[c] = Path.GetFileName(lvi.SubItems[2].Text);
               
               max_progress_value =max_progress_value+ totalpage[c];
               c++;
            }
        
         Thread th = new Thread((() =>  scobj.start_thread_for_Single_page_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value)));
         th.SetApartmentState(ApartmentState.STA);
         th.Start();

     cancel: ;
          
        }

        private void backup_single_btn_Click(object sender, EventArgs e)
        {
            //initialization
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;

            //evaluation
            string path = getpath();

            foreach (ListViewItem lvi in listView1.Items)
            {

                pr[c] = getreader(lvi);
                totalpage[c] = pr[c].NumberOfPages;
                can_replace = checkBox_replace_existing.Checked;
                file[c] = Path.GetFileName(lvi.SubItems[2].Text);
                c++;
                max_progress_value = max_progress_value + totalpage[c];
                //start_BackgroundWorker_for_splitting(object sender, EventArgs e,TableLayoutPanel tablelayoutpanel,int progressbarmaxvalue, string path, PdfReader[] pr, string[] input_pdf, string output_pdf, int total, bool can_replace)
                //  new Thread((() => threaded_splitting(path, pr,file, "", total_page,can_replace))).Start();

            }

            //  scobj.start_BackgroundWorker_for_splitting(sender, e, tableLayoutPanel1, max_progress_value, path, pr, file, "", total_page, can_replace);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    buttonRemove_Click(sender, e);
                }
                else
                {
                    scobj = new supporting_class();
                    ListViewItem temp_lvitem = scobj.Up_or_DownkeyPressed(e, listView1);
                    if (temp_lvitem != null)
                    {
                      //  PdfReader temp_reader = getreader(temp_lvitem);
                      // scobj.GetandSetBookmark(temp_reader, temp_lvitem.SubItems[2].Text,treeView1);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
           
        }

        private void listView1_Click(object sender, EventArgs e)
        {

           supporting_class sc = new supporting_class();

            if (sc.listViewItemClicked(comboBox2, comboBox1, listView1, current_li, previous_li))
            {
                try
                {
                  //  PdfReader tempreader = getreader(listView1.SelectedItems[0]);
                  // sc.GetandSetBookmark(tempreader, listView1.SelectedItems[0].SubItems[2].Text,treeView1);
                }
                catch (Exception ex)
                {
                 
                }
                
            }
           
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                scobj = new supporting_class();
                scobj.listViewSelectedIndexChanged(listView1,comboBox2,comboBox1);
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                supporting_class sc = new supporting_class();
                string[] tempinputfiles = e.Data.GetData(DataFormats.FileDrop) as string[];

                string[] inputfiles = sc.getnewfiles(tempinputfiles,listView1);
                if (inputfiles.Length > 0)
                {
                    new Thread((() => add_to_system(inputfiles))).Start();
                    show_loading_form();
                }
            }
            catch (Exception ex)
            {
            }
       

                
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void maintabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            current_tabpage = e.TabPage.Name;
            if(listView1.Items.Count>0)
            newpdf = listView1.SelectedItems[0].SubItems[2].Text;
            if (current_tabpage == "tabPageforReader" && previous_tabpage != current_tabpage && newpdf != loadedpdf)
            {
                supporting_class sc = new supporting_class();
                
                if (sc.listViewItemClicked(comboBox2, comboBox1, listView1, current_li, previous_li))
                {
                    
                    try
                    {
                        PdfReader tempreader = getreader(listView1.SelectedItems[0]);
                        string s=listView1.SelectedItems[0].SubItems[2].Text;
                        new Thread((() => sc.GetandSetBookmark(tempreader,s , pdf_filetree,contextMenuStrip1))).Start();

                        if (listView1.SelectedItems[0].SubItems[1].Tag.ToString() == "1")
                        {
                            this.OpenPDFfile(listView1.SelectedItems[0].SubItems[2].Text, listView1.SelectedItems[0].SubItems[2].Tag.ToString());
                        }
                        else
                        {
                            this.OpenPDFfile(listView1.SelectedItems[0].SubItems[2].Text, null);
                        }

                        
                       // sc.GetandSetBookmark(tempreader, listView1.SelectedItems[0].SubItems[2].Text, treeView1 );
                       
                         
                    }
                    catch (Exception ex)
                    {

                    }

                }
                loadedpdf = listView1.SelectedItems[0].SubItems[2].Text;  
            }
            

            previous_tabpage = current_tabpage;
        }

        private void RemoveBlankPageBtn_Click(object sender, EventArgs e)
        {

            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
           
            //declaration and initialization
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            blank_pages = new List<int>[total_pdf];
            without_blank_page = new List<int>[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;

            //evaluation
            string folder = getpath();
            can_replace = checkBox_replace_existing.Checked;

            try
            {
                foreach (ListViewItem lvi in this.listView1.Items)
                    {
                        tmp = c;
                        blank_pages[tmp] = new List<int>();
                        without_blank_page[tmp] = new List<int>();
                        blank_pages[tmp].Clear();
                        without_blank_page[tmp].Clear();
                        file[tmp] = Path.GetFileName(lvi.SubItems[2].Text);
                        pr[tmp] = getreader(lvi);
                        totalpage[tmp] = pr[tmp].NumberOfPages;

                        c++;
                    }
                // start_thread_for_removing_blank_pages(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int reader_count)
                  Thread th = new Thread((() => scobj.start_thread_for_removing_blank_pages(tableLayoutPanel1, folder, pr, file, "", totalpage, can_replace, max_progress_value, c)));
                  th.SetApartmentState( ApartmentState.STA);
                  th.Start();
            }

            catch (Exception ex)
            {
                
            }

      cancel: ;


            }
           


    
        private void WorkerDoWork(int value, string text, int p, int q)
        {
           
        }



        #region Arif code for reader
        public void OpenPDFfile(String filename, String password)
        {
            pdfreaderBox.OpenPDF(filename, password);
            // pdfreaderBox.OpenPDF(dlg.FileName, string.Empty);
            txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
            lbl_pagenum.Text = "of " + pdfreaderBox.PageCount.ToString();
            toolStrip1.Enabled = true;

        }

        private void bk_markhide_Click(object sender, EventArgs e)
        {
            if (bk_markhide.Text == ">")
            {
                bk_markhide.Text = "<";
                splitContainer6.Panel1Collapsed = false;
            }
            else
            {
                bk_markhide.Text = ">";
                splitContainer6.Panel1Collapsed = true;
            }
        }
        private void zoomIN_Click(object sender, EventArgs e)
        {
            pdfreaderBox.ZoomIn();
        }

        private void zoomout_Click(object sender, EventArgs e)
        {
            pdfreaderBox.ZoomOut();
        }
        private void go_firstpage_Click(object sender, EventArgs e)
        {
            pdfreaderBox.FirstPage();
            txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
        }

        private void previous_page_Click(object sender, EventArgs e)
        {
            pdfreaderBox.CurrentPage--;
            txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
        }

        private void next_page_Click(object sender, EventArgs e)
        {
            pdfreaderBox.CurrentPage++;
            txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
        }

        private void go_lastpage_Click(object sender, EventArgs e)
        {
            pdfreaderBox.LastPage();
            txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
        }
        private void txt_pagenum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                  && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '\r')
            {

                e.Handled = true;
                int pageNumber;

                if (int.TryParse(txt_pagenum.Text, out pageNumber) && pageNumber > 0 && pageNumber <= pdfreaderBox.PageCount)
                {
                    pdfreaderBox.CurrentPage = Convert.ToInt16(txt_pagenum.Text);
                }
                else
                    txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
            }
        }

        #endregion

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitEqualPageBtn_Click(object sender, EventArgs e)
        {
            int number_of_pages_in_each_file;
           //error checking
            if ( listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            if(splitEqualpagesTxtBx.Text.Length < 1 )
            {
                MessageBox.Show("Please write number in the textbox");
                goto cancel;
            }
            try 
            {
                number_of_pages_in_each_file = Convert.ToInt32(splitEqualpagesTxtBx.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please write Integer number in the textbox");
                goto cancel;
            }

            //declaration and initialization
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;


            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.Items)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed
                pr[c] = getreader(lvi);
                totalpage[c] = pr[c].NumberOfPages;
                file[c] = Path.GetFileName(lvi.SubItems[2].Text);
                if (totalpage[c] <= number_of_pages_in_each_file)
                {   
                    MessageBox.Show("Your expected page number in each file is equal or larger than total page of "+Path.GetFileName(file[c]));
                    goto cancel;
                }
                tmp = c;
                
               // totalpage[c] = (end - start) + 1;

                max_progress_value = max_progress_value + totalpage[c];
                c++;
            }

            Thread th = new Thread((() => scobj.start_thread_for_equal_page_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value, number_of_pages_in_each_file)));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

        cancel: ;
        }

        private void SplitEqualSizeBtn_Click(object sender, EventArgs e)
        {
            long desired_size = 0;
           // int number_of_pages_in_each_file;
            //error checking
            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            if (FileSizecomboBox.Text == null)
            {
                MessageBox.Show("Please select file size");
                goto cancel;
            }
            try
            {
               
                if (FileSizecomboBox.Text.Contains("KB"))
                {
                    desired_size = (Convert.ToInt64(FileSizecomboBox.Text.Substring(0, FileSizecomboBox.Text.Length - 2)) * 1024);
                }
                else
                {
                    desired_size = (Convert.ToInt64(FileSizecomboBox.Text.Substring(0, FileSizecomboBox.Text.Length - 2)) * 1024 * 1024);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select file size properly");
                goto cancel;
            }

            //declaration and initialization
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            long[] input_file_size = new long[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;


            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.Items)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed
                pr[c] = getreader(lvi);
                totalpage[c] = pr[c].NumberOfPages;
                file[c] = Path.GetFileName(lvi.SubItems[2].Text);

                FileInfo infi = new FileInfo(file[c]);
                 input_file_size[c] = infi.Length;

                if (input_file_size[c] <= desired_size)
                {
                    MessageBox.Show("Your expected file size is equal or larger than the size of " + Path.GetFileName(file[c]));
                    goto cancel;
                }
                tmp = c;

                // totalpage[c] = (end - start) + 1;

                max_progress_value = max_progress_value + totalpage[c];
                c++;
            }

            Thread th = new Thread((() => scobj.start_thread_for_equal_file_size_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value, desired_size,input_file_size)));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

        cancel: ;
           
           
        }

        private void splitByRangeSinglePageBtn_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            int start = 0;
            int end = 0;
            try
            {
                start = Convert.ToInt32(comboBox2.SelectedItem.ToString());
                end = Convert.ToInt32(comboBox1.SelectedItem.ToString());
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Please select range properly");
                goto cancel;
            }
           

            if (start > end)
            {
                MessageBox.Show("Please select range properly");
                goto cancel;
            }
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;


            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed

                tmp = c;
                pr[c] = getreader(lvi);

                totalpage[c] = (end - start) + 1;

                file[c] = Path.GetFileName(lvi.SubItems[2].Text);

                max_progress_value = max_progress_value + totalpage[c];
                c++;
            }

            Thread th = new Thread((() => scobj.start_thread_for_Single_page_splitting_with_range(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value, start, end)));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

        cancel: ;    
        
        }

     

        private void splitByRangeSingleFileBtn_Click(object sender, EventArgs e)
        {
           if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
           int start = 0; 
           int end = 0; 
            try
            {
                start = Convert.ToInt32(comboBox2.SelectedItem.ToString());
                 end =  Convert.ToInt32(comboBox1.SelectedItem.ToString());
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Please select range properly");
                goto cancel;
            }

            if (start > end)
            {
                MessageBox.Show("Please select range properly");
                goto cancel;
            }
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
            int max_progress_value = 0;
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;
            

            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed

                tmp = c;
                pr[c] = getreader(lvi);
             
                totalpage[c] = (end-start)+1;

                file[c] = Path.GetFileName(lvi.SubItems[2].Text);

                max_progress_value = max_progress_value + totalpage[c];
                c++;
            }

            Thread th = new Thread((() => scobj.start_thread_for_Single_file_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value,start,end)));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

        cancel: ; 
        
        }

        private void SplitSearchFirstLastBtn_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            string search_string = searchTxtBx.Text;
            
            if (searchTxtBx.Text.Length < 1)
            {
                MessageBox.Show("Please write something to search");
                goto cancel;
            }
           
            
            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];
         
            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;
           

            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.Items)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed

                tmp = c;
                pr[c]=  getreader(lvi);
                totalpage[c] = pr[c].NumberOfPages;
                file[c] = Path.GetFileName(lvi.SubItems[2].Text);
                c++;
                
            }
            if (c > 0)
            {
                Thread th = new Thread((() => scobj.start_thread_for_Single_file_splitting_with_search(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, c, search_string)));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
            }
        cancel: ; 
        }

        private void SplitSearchEachPagetBtn_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1)
            {
                MessageBox.Show("Please Add files to the list");
                goto cancel;
            }
            string search_string = searchTxtBx.Text;

            if (searchTxtBx.Text.Length < 1)
            {
                MessageBox.Show("Please write something to search");
                goto cancel;
            }


            supporting_class scobj = new supporting_class();
            bool can_replace = false;
            int total_pdf = listView1.Items.Count;
            string[] file = new string[total_pdf];
            int[] totalpage = new int[total_pdf];

            PdfReader[] pr = new PdfReader[total_pdf];
            int c = 0;
            int tmp = 0;


            //evaluation
            string path = getpath();
            can_replace = checkBox_replace_existing.Checked;
            foreach (ListViewItem lvi in listView1.Items)
            {
                //tmp is used because new thread is started and inside threadcall indexing is needed

                tmp = c;
                pr[c] = getreader(lvi);
                totalpage[c] = pr[c].NumberOfPages;
                file[c] = Path.GetFileName(lvi.SubItems[2].Text);
                c++;

            }
            if (c > 0)
            {
                Thread th = new Thread((() => scobj.start_thread_for_Single_page_splitting_with_search(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, c, search_string)));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
            }
        cancel: ; 
        }

        private void BlankPageFoundBtn_Click(object sender, EventArgs e)
        {

        }
        TreeNode current_tn = new TreeNode();
        TreeNode previous_tn = new TreeNode();
        private void pdf_filetree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            bool check = true;
            try
            {
                if (previous_tn != e.Node)
                {
                    int PageIndex = 0;
                    if (e.Node.Tag == null)
                    {
                        return;
                    }
                    else
                    {
                        PageIndex = Convert.ToInt32(e.Node.Tag);
                      
                        pdfreaderBox.CurrentPage=PageIndex;
                        txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
                    
                    }
                   
                    pdf_filetree.SelectedNode=e.Node;
          
                }
                else
                { }
                previous_tn = e.Node;
            }
            catch (Exception ex)
            {
               // show_message_box(ex.Message);
            }

            
        }
        int first_page_bkmark = 0;
        int last_page_bkmark = 0;
        private void fromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                first_page_bkmark = Convert.ToInt32(pdf_filetree.SelectedNode.Tag.ToString());
            }
            catch (Exception ex)
            {
                
                
            }
        }

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
           
        }

        private void pdfreaderBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (pdfreaderBox.CurrentPage != pdfreaderBox.PageCount)
                    {
                        // scrol_pdfpage.Value = scrol_pdfpage.Value + 1;
                        pdfreaderBox.CurrentPage++;
                        txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
                    }
                    break;
                case Keys.Up:
                    if (pdfreaderBox.CurrentPage != 1)
                    {
                        // scrol_pdfpage.Value = scrol_pdfpage.Value-1;
                        pdfreaderBox.CurrentPage--;
                        txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
                    }
                    break;
                case Keys.Right:
                    if (pdfreaderBox.CurrentPage != pdfreaderBox.PageCount)
                    {
                        //scrol_pdfpage.Value = scrol_pdfpage.Value + 1;
                        pdfreaderBox.CurrentPage++;
                        txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
                    }
                    break;
                case Keys.Left:
                    if (pdfreaderBox.CurrentPage != 1)
                    {
                        //scrol_pdfpage.Value = scrol_pdfpage.Value - 1;
                        pdfreaderBox.CurrentPage--;
                        txt_pagenum.Text = pdfreaderBox.CurrentPage.ToString();
                    }
                    break;
             
            }
        }

        private void maintabControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    {

                        if (listView1.Items.Count < 1)
                        {
                            MessageBox.Show("Please Add files to the list");
                            goto cancel;
                        }
                        string folder = getpath();
                        if (folder != null)
                        {
                            string file_name = Path.GetFileName(listView1.SelectedItems[0].SubItems[2].Text);
                            string file = Regex.Replace(file_name, @".pdf", " - " + pdfreaderBox.CurrentPage + "_.pdf", RegexOptions.IgnoreCase);
                            PdfReader pr = getreader(listView1.SelectedItems[0]);
                            splitting_and_merging_class smc = new splitting_and_merging_class();
                            if (smc.split_Page_by_keyboard(pr, folder + "\\" + file, pdfreaderBox.CurrentPage, pdfreaderBox.CurrentPage))
                            {
                                MessageBox.Show("Splitting single page successfull");
                            }
                            else
                            {
                                MessageBox.Show("Error occured in single page splitting");
                            }
                        }
                    }

                    break;
            }
        cancel: ;
    
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void singlePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                last_page_bkmark = Convert.ToInt32(pdf_filetree.SelectedNode.Tag.ToString());
                //  split_bkmark_processing(first_page_bkmark, last_page_bkmark);

                if (first_page_bkmark > last_page_bkmark || first_page_bkmark == 0 || last_page_bkmark == 0)
                {
                    MessageBox.Show("Please select range properly");
                    goto cancel;
                }
                int tmp_start = first_page_bkmark;
                int tmp_last = last_page_bkmark;
                first_page_bkmark = 0;
                last_page_bkmark = 0;

                supporting_class scobj = new supporting_class();
                bool can_replace = false;
                int total_pdf = listView1.Items.Count;
                string[] file = new string[1];
                int[] totalpage = new int[1];
                int max_progress_value = 0;
                PdfReader[] pr = new PdfReader[1];
                int c = 0;
                int tmp = 0;

                //evaluation
                string path = getpath();
                can_replace = checkBox_replace_existing.Checked;
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    //tmp is used because new thread is started and inside threadcall indexing is needed
                    // show_message_box(Path.GetFileName(lvi.SubItems[2].Text));
                    tmp = c;
                    pr[c] = getreader(lvi);

                    totalpage[c] = (tmp_last - tmp_start) + 1;

                    file[c] = Path.GetFileName(lvi.SubItems[2].Text);

                    max_progress_value = max_progress_value + totalpage[c];
                    c++;
                }

                Thread th = new Thread((() => scobj.start_thread_for_Single_page_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value)));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();

            cancel: ;

            }
            catch (Exception ex)
            {

            }
        }

        private void singleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                last_page_bkmark = Convert.ToInt32(pdf_filetree.SelectedNode.Tag.ToString());
                //  split_bkmark_processing(first_page_bkmark, last_page_bkmark);

                if (first_page_bkmark > last_page_bkmark || first_page_bkmark == 0 || last_page_bkmark == 0)
                {
                    MessageBox.Show("Please select range properly");
                    goto cancel;
                }
                int tmp_start = first_page_bkmark;
                int tmp_last = last_page_bkmark;
                first_page_bkmark = 0;
                last_page_bkmark = 0;

                supporting_class scobj = new supporting_class();
                bool can_replace = false;
                int total_pdf = listView1.Items.Count;
                string[] file = new string[1];
                int[] totalpage = new int[1];
                int max_progress_value = 0;
                PdfReader[] pr = new PdfReader[1];
                int c = 0;
                int tmp = 0;

                //evaluation
                string path = getpath();
                can_replace = checkBox_replace_existing.Checked;
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    //tmp is used because new thread is started and inside threadcall indexing is needed
                    // show_message_box(Path.GetFileName(lvi.SubItems[2].Text));
                    tmp = c;
                    pr[c] = getreader(lvi);

                    totalpage[c] = (tmp_last - tmp_start) + 1;

                    file[c] = Path.GetFileName(lvi.SubItems[2].Text);

                    max_progress_value = max_progress_value + totalpage[c];
                    c++;
                }
                
                Thread th = new Thread((() => scobj.start_thread_for_Single_file_splitting(tableLayoutPanel1, path, pr, file, "", totalpage, can_replace, max_progress_value, tmp_start, tmp_last)));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();

              //  tmp_last = 0;
               // tmp_start = 0;

            cancel: ;

            }
            catch (Exception ex)
            {

            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
              //  show_message_box(Path.GetDirectoryName(Application.ExecutablePath));
                System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "Help.pdf");
                // Path.GetDirectoryName( Application.ExecutablePath)+"\\"+
              
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //show_message_box(ex.Message + "\n\n\n\n\n" + ex.ToString());
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            maintabControl.SelectedTab = tabPageforListView;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            maintabControl.SelectedTab = tabPageforSettings;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            maintabControl.SelectedTab = tabPageforOperations;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            maintabControl.SelectedTab = tabPageforReader;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            maintabControl.SelectedTab = tabPagefoProgressBar;
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button_max_Click(object sender, EventArgs e)
        {
            if (state == true)
            {
                this.WindowState = FormWindowState.Normal;
                int boundWidth = Screen.PrimaryScreen.Bounds.Width;
                int boundHeight = Screen.PrimaryScreen.Bounds.Height;
                int x = boundWidth - this.Width;
                int y = boundHeight - this.Height;
                this.Location = new Point(x / 2, y / 2);
                state = false;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;

                state = true;
            }
        }

        private void button_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                about_Form af = new about_Form();
                af.ShowDialog();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
