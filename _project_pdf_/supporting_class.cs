using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace _project_pdf_
{
    class supporting_class 
    {
        //initialization
        ProgressBar[] pgb = new ProgressBar[500];
        BackgroundWorker[] bgw = new BackgroundWorker[500];
        Button[] btn = new Button[500];
        Label[] lbl = new Label[500];
    
        int max = 500;
        int c = 1;
        bool[] isused = new Boolean[500];

        ArrayList arr = new ArrayList();

        public string removeListViewItem(ListView lvi, ListViewItem lvIt )
        {
            String path = null;
            try
            {

                if (lvi.SelectedItems.Count > 0)
                {
                    lvi.Focus();
              
                        int i = lvi.SelectedIndices[0];
                      
                        if (i == 0 && lvi.Items.Count == 1)
                        {

                        }
                        lvi.Items.Remove(lvIt);
                        if (lvi.Items.Count > 0)
                        {
                            if (i < lvi.Items.Count)
                            {
                                 lvi.Items[i].Selected = true;
                                 path = @lvi.Items[i].SubItems[2].Text;
                             }
                            else
                            {
                                lvi.Items[lvi.Items.Count - 1].Selected = true;
                                path = @lvi.Items[lvi.Items.Count - 1].SubItems[2].Text;
                            
                            }
                        }
                        else
                        {
                            //split_option_panel.Hide();
                          /*  pnl_topview.Hide();
                            pdfImageBox.Hide();
                            btn_back.Hide();
                            btn_forward.Hide();
                            tabControl1.Hide();
                            button11.Show();*/
                            
                        }
                        //this.UpdateButtons();
                        lvi.Focus();
                    
                    }
                return path;
                
            }
            catch (Exception ex)
            {

                return path;
            }
            
        }

        void show_messageBox(string str)
        {
           // MessageBox.Show(str);
        }

        //update listview 
        public bool listViewItemClicked(ComboBox comboBox2, ComboBox comboBox1, ListView listView1, ListViewItem current_li, ListViewItem previous_li)
        {
            
            bool hastoopen = false;
            if(listView1.Items.Count > 0)
            current_li = listView1.SelectedItems[0];

            if (listView1.Items.Count == 1)
            {
                ListViewItem lvi = listView1.SelectedItems[0];
                foreach (ListViewItem temp in listView1.Items)
                {
                    temp.BackColor = Color.White;
                }

                listView1.SelectedItems[0].BackColor = Color.LimeGreen;
                comboBox2.Items.Clear();
                comboBox2.DisplayMember = "1";
                
                comboBox1.Items.Clear();

                for (int i = 1; i <= Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text); i++)
                {
                    comboBox2.Items.Add(i);
                    comboBox1.Items.Add(i);
                }
                hastoopen = true;
               

            }
            else if (current_li != previous_li && listView1.Items.Count > 1)
            {
                listView1.Focus();
          
                ListViewItem lvi = listView1.SelectedItems[0];
                foreach (ListViewItem temp in listView1.Items)
                {
                    temp.BackColor = Color.White;
                }

                listView1.SelectedItems[0].BackColor = Color.LimeGreen;
                comboBox2.Items.Clear();
                comboBox2.DisplayMember = "1";
                comboBox1.Items.Clear();

                for (int i = 1; i <= Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text); i++)
                {
                    comboBox2.Items.Add(i);
                    comboBox1.Items.Add(i);
                }
                hastoopen = true;
            }
            if (listView1.Items.Count > 0)
            previous_li = listView1.SelectedItems[0];
            return hastoopen;
        }

        public ListViewItem Up_or_DownkeyPressed(KeyEventArgs e, ListView listView1)
        {
            
            ListViewItem lvitem = null;
             if (e.KeyCode == Keys.Up)
                {
                    // this.listView1.Focus();
                    foreach (ListViewItem temp in listView1.Items)
                    {
                        temp.BackColor = Color.White;
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Selected)
                        {
                            if (i > 0)
                            {
                                lvitem = listView1.Items[i - 1];
                                listView1.Items[i - 1].Selected = true;
                                listView1.SelectedItems[0].BackColor = Color.LimeGreen;
                                //  this.OpenPDF(path);
                            }

                        }
                    }

                }
            else if (e.KeyCode == Keys.Down)
                {
                    foreach (ListViewItem temp in listView1.Items)
                    {
                        temp.BackColor = Color.White;
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Selected)
                        {
                            if (i < listView1.Items.Count - 1)
                            {
                                lvitem = listView1.Items[i + 1];
                                listView1.Items[i + 1].Selected = true;
                                listView1.SelectedItems[0].BackColor = Color.LimeGreen;
                                // this.OpenPDF(path);
                            }

                        }
                    }
                }
             return lvitem;
        }

        public bool listViewSelectedIndexChanged(ListView listView1, ComboBox comboBox2, ComboBox comboBox1)
        {
            foreach (ListViewItem temp in listView1.Items)
            {
                temp.BackColor = Color.White;
            }

            foreach (ListViewItem l in listView1.SelectedItems)
            {
               // MessageBox.Show(l.SubItems[0].Text);
            }
            listView1.SelectedItems[0].BackColor = Color.LimeGreen;
            comboBox2.Items.Clear();
            comboBox2.DisplayMember = "1";

            comboBox1.Items.Clear();
            comboBox1.DisplayMember = "1";

            for (int i = 1; i <= Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text); i++)
            {
                comboBox2.Items.Add(i);
                comboBox1.Items.Add(i);
            }
            return true;
        }

        public bool GetandSetBookmark(PdfReader pr, string f,TreeView treeView1,ContextMenuStrip cntxtmnstrip)
        {
            
            bool hasbookmark = false;
            try
            {
                bookmark_class obj = new bookmark_class(cntxtmnstrip);
                if (treeView1.InvokeRequired)
                {
                    treeView1.Invoke((MethodInvoker)(() => treeView1.Nodes.Clear()));
                    TreeNode trnd = new TreeNode(Path.GetFileNameWithoutExtension(f));
                    trnd.ForeColor = Color.MediumVioletRed;
                    treeView1.Invoke((MethodInvoker)(() => treeView1.Nodes.Add(trnd)));
                    treeView1.Invoke((MethodInvoker)(() => hasbookmark = obj.GetBKTreeView(pr, ref treeView1)));
                }
                else
                {
                    treeView1.Nodes.Clear();
                    TreeNode trnd = new TreeNode(Path.GetFileNameWithoutExtension(f));
                    trnd.ForeColor = Color.MediumVioletRed;
                    treeView1.Nodes.Add(trnd);
                    hasbookmark = obj.GetBKTreeView(pr, ref treeView1);
                }
                if (!hasbookmark)
                {
                    if (treeView1.InvokeRequired)
                    {
                        treeView1.Invoke((MethodInvoker)(() => treeView1.Nodes.Clear()));
                    }
                    else
                    {
                        treeView1.Nodes.Clear();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                show_messageBox(ex.ToString());
                return false;
            }
        }

        public string[] getnewfiles(string[] tempinputfiles,ListView listView1)
        {

            ArrayList AlreadyAdded = new ArrayList();
            ArrayList inputfile = new ArrayList();
            AlreadyAdded.Clear();
            inputfile.Clear();

            foreach (ListViewItem l in listView1.Items)
            {
                AlreadyAdded.Add(l.SubItems[2].Text);
            }
            foreach (string f in tempinputfiles)
            {
                if (!AlreadyAdded.Contains(f) && Path.GetExtension(f).ToLower() == ".pdf")
                {

                    inputfile.Add(f);
                }
            }
            return inputfile.ToArray(typeof(string)) as string[];
        }

        public PdfReader getreader(ListViewItem lvi)
        {
            if ((string)lvi.SubItems[1].Tag == "1")
            {
                byte[] bytepassword = Encoding.UTF8.GetBytes(lvi.SubItems[2].Tag.ToString());
                return new PdfReader(new RandomAccessFileOrArray(lvi.SubItems[2].Text), bytepassword);
            }
            else
            {
                return new PdfReader(new RandomAccessFileOrArray(lvi.SubItems[2].Text), null);
            }
        }

       

        public void start_BackgroundWorker_for_merging(object sender, EventArgs e, TableLayoutPanel tablelayoutpanel,int progressbarmaxvalue, PdfReader[] pr, string outputfile )
        {

           
            int row = tablelayoutpanel.RowCount-2;
            
            int l_column_width = (int) tablelayoutpanel.ColumnStyles[0].Width - 10;
            int p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
            int b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            
            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //  if (c == 1 && !(ispgbrfopen))
                    //{
                    //pgbrf.Show();
                    //  ispgbrfopen = true;
                    //}

                                     

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Merging as  " + Path.GetFileName(outputfile);
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;


                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += CancelOperation_Click;
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = progressbarmaxvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;
                    


                    //backgroundworker initialization
                    bgw[c] = new BackgroundWorker();
                    bgw[c].DoWork += ((obj, ep) => smc.merge(obj, ep, pr, outputfile, progressbarmaxvalue, bgw, lbl, pgb, tablelayoutpanel, btn, isused));
                    bgw[c].ProgressChanged += ((obj, ep) => ProgressChanged( obj,ep));
                    bgw[c].WorkerReportsProgress = true;
                    bgw[c].WorkerSupportsCancellation = true;



                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    
                    tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                    //add progressbar
                    // pgbrf.flowLayoutPanel1.Controls.Add(pgb[c]);
                   // mainformobj.tabPagefoProgressBar.Controls.Add(pgb[c]);
                    tablelayoutpanel.Controls.Add(pgb[c],1,row);

                    //add cancal btn
                    // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                    tablelayoutpanel.Controls.Add(btn[c],2,row);


                    //add row to tablelayoutpanel
                    tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                    tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                    tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                  

                    //backgroundworker start              
                    bgw[c].RunWorkerAsync((object)c);
                   // c++;
                    break;
                }

            }

        }

          
        void CancelOperation_Click(object sender, EventArgs e)
        {
            
            Control ctl = sender as Control;
            bgw[Convert.ToInt32(ctl.Name)].CancelAsync();

        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= pgb[Convert.ToInt32(e.UserState)].Maximum)
            {
                pgb[Convert.ToInt32(e.UserState)].Value = e.ProgressPercentage;
            }
        }

        public void start_thread_for_Single_page_splitting(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue)
        {
            int row =0;
           
            Thread c_th = Thread.CurrentThread;
       
            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            { 
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;
                
            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Splitting Running " ;
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj,ep,c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;

           
                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                         tablelayoutpanel.Invoke((MethodInvoker)(() =>  tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                          tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() =>   tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() =>   tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                         tablelayoutpanel.Invoke((MethodInvoker)(() =>  tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                         tablelayoutpanel.Invoke((MethodInvoker)(() =>  tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    start_single_page_splitting( tablelayoutpanel, lbl[c],pgb[c],btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressvalue);
                    
                    
                   
                    //c++;
                    break;
                }

            }
        }
        
        public void cancel_splitting_click(object sender, EventArgs e,Thread th)
        {
            
            try
            {
                th.Abort();
            }
            catch (Exception ex)
            { 
                
            }
            
        }

        public void start_single_page_splitting( TableLayoutPanel tablelayoutpanel, Label lbl,ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue)
        {
            int lowbound = 0;
            int upper_bound = 0;
                
            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            foreach (PdfReader pdfreader in pr)
            {
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                    //  tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Minimum = first_page));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = total[tmp_counter]));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                    // 
                }
                for (int i = 1; i <= total[tmp_counter]; i++)
                {

                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + (i).ToString("00000") + ".pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;
                    

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }

                cancel: ;
                }
                tmp_counter++;
                if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
            }
        }

        public void start_thread_for_Single_file_splitting(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int start,int end)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Splitting Running ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    start_single_file_splitting(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressvalue,start,end);



                    //c++;
                    break;
                }

            }
        }
        
        
        public void start_single_file_splitting(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int start, int end)
        {
            int lowbound = 0;
            int upper_bound = 0;

            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            foreach (PdfReader pdfreader in pr)
            {
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                    //  tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Minimum = first_page));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = total[tmp_counter]));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                    // 
                }
              //  for (int i = 1; i <= total[tmp_counter]; i++)
                {

                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + start+"_"+end + "_.pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, start, end, false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, start, end, false, maxprogressvalue);
                    }

                cancel: ;
                }
                tmp_counter++;
                if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
            }
        }

        public void search_for_single_file_splitting(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int reader_count,string search_string)
        {
            int max_progress_value = 0;
            int tmp_counter = 0;
            int c=0;
            List<int>[] pages = new List<int>[reader_count];
            int[] start = new int[reader_count];
            int[] end = new int[reader_count];
            
            Regex regex = new Regex("~!+@#$%^", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            regex = new Regex(search_string, RegexOptions.IgnoreCase | RegexOptions.Compiled);
           PdfReader[] reader_with_search_item=new PdfReader[reader_count];
            string[] input_pdf_with_search_item=new string[reader_count];

            foreach (PdfReader pdfreader in pr )
            {
                if (tablelayoutpanel.InvokeRequired)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Searching "+Path.GetFileName(input_pdf[tmp_counter])));
                }
                PdfReaderContentParser parser = new PdfReaderContentParser(pr[tmp_counter]);
                pages[c] = new List<int>();
                for (int i = 1; i <= total[tmp_counter]; i++)
                {
                    try
                    {
                        ITextExtractionStrategy strategy = parser.ProcessContent(i, new SimpleTextExtractionStrategy());
                        if (regex.IsMatch(strategy.GetResultantText()))
                        {
                            // do whatever with corresponding page number i...
                            //  MessageBox.Show("Found  " + search_string + " in " + i.ToString() + " no page");
                            // show_message_box(i.ToString());
                            pages[c].Add(i);
                        }
                    }
                    catch (ThreadAbortException ex)
                    {

                        if (tablelayoutpanel.InvokeRequired)
                        {

                            tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                            tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        // show_message_box(ex.Message + "\n" + ex.ToString());
                        goto omitthisfile;
                    }
                }

                if (pages[c].Count > 0)
                {
                    input_pdf_with_search_item[c] = input_pdf[tmp_counter];
                    reader_with_search_item[c] = pdfreader;
                    start[c] = pages[c][0];
                    end[c] = pages[c][pages[c].Count - 1];
                    max_progress_value = max_progress_value + (end[c] - start[c]) + 1;

                    c++;
                }
                else
                {
                    MessageBox.Show("Could not find " + search_string+" in "+Path.GetFileName(input_pdf[tmp_counter]));
                }
               
            omitthisfile: ;
                tmp_counter++;
            }
           // start_thread_for_Single_file_splitting_with_search(tablelayoutpanel, folder, reader_with_search_item, input_pdf_with_search_item, "", can_replace, max_progress_value, start, end, c);
            if (c >0)
            {

                start_single_file_splitting_with_search(tablelayoutpanel, lbl, pgb, btn, folder, reader_with_search_item, input_pdf_with_search_item, "", can_replace, max_progress_value, start, end, c);
            }
            else
            {
                if (tablelayoutpanel.InvokeRequired)
                {

                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = search_string + " could not be found"));
                }
            }
        }

        public void search_for_single_page_splitting(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int reader_count, string search_string)
        {
            int max_progress_value = 0;
            int tmp_counter = 0;
            int c = 0;
            List<int>[] pages = new List<int>[reader_count];
            int[] start = new int[reader_count];
            int[] end = new int[reader_count];

            Regex regex = new Regex("~!+@#$%^", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            regex = new Regex(search_string, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            PdfReader[] reader_with_search_item = new PdfReader[reader_count];
            string[] input_pdf_with_search_item = new string[reader_count];

            foreach (PdfReader pdfreader in pr)
            {
                if (tablelayoutpanel.InvokeRequired)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Searching " + Path.GetFileName(input_pdf[tmp_counter])));
                }
                PdfReaderContentParser parser = new PdfReaderContentParser(pr[tmp_counter]);
                pages[c] = new List<int>();
                for (int i = 1; i <= total[tmp_counter]; i++)
                {
                    try
                    {
                        ITextExtractionStrategy strategy = parser.ProcessContent(i, new SimpleTextExtractionStrategy());
                        if (regex.IsMatch(strategy.GetResultantText()))
                        {
                            // do whatever with corresponding page number i...
                            //  MessageBox.Show("Found  " + search_string + " in " + i.ToString() + " no page");
                            // show_message_box(i.ToString());
                            pages[c].Add(i);
                        }
                    }
                    catch (ThreadAbortException ex)
                    {

                        if (tablelayoutpanel.InvokeRequired)
                        {

                            tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                            tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                        }

                    }
                    catch (Exception ex)
                    {
                        // show_message_box(ex.Message + "\n" + ex.ToString());
                        goto omitthisfile;
                    }
                }

                if (pages[c].Count > 0)
                {
                    input_pdf_with_search_item[c] = input_pdf[tmp_counter];
                    reader_with_search_item[c] = pdfreader;
                    start[c] = pages[c][0];
                    end[c] = pages[c][pages[c].Count - 1];
                    max_progress_value = max_progress_value + end[c] - start[c] + 1;

                    c++;
                }
                else
                {
                    
                    MessageBox.Show("Could not find " + search_string + " in " + Path.GetFileName(input_pdf[tmp_counter]));
                   
                }

            omitthisfile: ;
                tmp_counter++;
            }
            // start_thread_for_Single_file_splitting_with_search(tablelayoutpanel, folder, reader_with_search_item, input_pdf_with_search_item, "", can_replace, max_progress_value, start, end, c);
            if (c > 0)
            {
                start_single_page_splitting_with_search(tablelayoutpanel, lbl, pgb, btn, folder, reader_with_search_item, input_pdf_with_search_item, "", can_replace, max_progress_value, start, end, c);
            }
            else
            {
                if (tablelayoutpanel.InvokeRequired)
                {

                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = search_string+" could not be found"));
                }
            }
        }

        public void start_thread_for_Single_file_splitting_with_search(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int reader_count, string search_string)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Searching ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                   // pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    search_for_single_file_splitting(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total,can_replace, reader_count,search_string);

           
                    //c++;
                    break;
                }

            }
        }


        public void start_single_file_splitting_with_search(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf,  bool can_replace, int maxprogressvalue, int[] start, int[] end, int reader_count)
        {
            int lowbound = 0;
            int upper_bound = 0;

            
            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool canceled = false;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            if (invoking_required)
            {
               
                tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum =maxprogressvalue));
                tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                // 
            }

            foreach (PdfReader pdfreader in pr)
            {
                if (reader_count == tmp_counter)
                    break;
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                  
                    // 
                }
                //  for (int i = 1; i <= total[tmp_counter]; i++)
                {
                   // show_messageBox("file name\t " + input_pdf[tmp_counter] + "\ttotal page " +total[tmp_counter]+ "\n start" + start[tmp_counter] + "\nend" + end[tmp_counter]);
                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + start[tmp_counter] + "_" + end[tmp_counter].ToString("0000") + "_.pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split_for_search(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, start[tmp_counter], end[tmp_counter], false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split_for_search(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, start[tmp_counter], end[tmp_counter], false, maxprogressvalue);
                    }

                cancel: ;
                canceled = true;
                }
                tmp_counter++;
                if (canceled)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Canceled";
                    }
                }
                else if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
            }
        }


        public void start_thread_for_Single_page_splitting_with_search(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int reader_count, string search_string)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Searching ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    // pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    search_for_single_page_splitting(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, reader_count, search_string);


                    //c++;
                    break;
                }

            }
        }


        public void start_single_page_splitting_with_search(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, bool can_replace, int maxprogressvalue, int[] start, int[] end, int reader_count)
        {
            int lowbound = 0;
            int upper_bound = 0;


            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool canceled = false;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            if (invoking_required)
            {

                tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = maxprogressvalue));
                tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                // 
            }

            foreach (PdfReader pdfreader in pr)
            {
                if (reader_count == tmp_counter)
                    break;
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));

                    // 
                }
                  for (int i = start[tmp_counter]; i <= end[tmp_counter]; i++)
                {
                    // show_messageBox("file name\t " + input_pdf[tmp_counter] + "\ttotal page " +total[tmp_counter]+ "\n start" + start[tmp_counter] + "\nend" + end[tmp_counter]);
                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + i.ToString("00000") + "_.pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split_for_search(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split_for_search(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }

                cancel: ;
                canceled = true;
                }
                tmp_counter++;
                if (canceled)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Canceled";
                    } 
                }
                else if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
            }
        }


        public void start_thread_for_Single_page_splitting_with_range(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int start,int end)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                   // lbl[c].Size = new System.Drawing.Size(230, 15);
                    lbl[c].Text = "Splitting Running ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;
                    lbl[c].Dock = DockStyle.Fill;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    start_single_page_splitting_with_range(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressvalue,start,end);



                    //c++;
                    break;
                }

            }
        }

        public void start_single_page_splitting_with_range(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int start,int end)
        {
            int lowbound = 0;
            int upper_bound = 0;

            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            foreach (PdfReader pdfreader in pr)
            {
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                    //  tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Minimum = first_page));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = total[tmp_counter]));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                    // 
                }
                for (int i = start; i <= end; i++)
                {

                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + (i).ToString("00000") + ".pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, i, i, false, maxprogressvalue);
                    }

                cancel: ;
                }
                tmp_counter++;
                if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
            }
        }

        public void start_thread_for_equal_page_splitting(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int wanted_page)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Splitting Running ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    start_equal_page_splitting(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressvalue,wanted_page);



                    //c++;
                    break;
                }

            }
        }

        public void start_equal_page_splitting(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int n_page)
        {
            int lowbound = 0;
            int upper_bound = 0;

            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool iscanceled = false;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            #region foreach
            foreach (PdfReader pdfreader in pr)
            {
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                    //  tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Minimum = first_page));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = total[tmp_counter]));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                    // 
                }
                int total_page = total[tmp_counter];
                int total_part = 0;
                string input_file = input_pdf[tmp_counter];

                if (total_page % n_page != 0)
                {
                    total_part = (total_page / n_page) + 1;
                }
                else
                    total_part = (total_page / n_page);

                #region forloop

                for (int i = 0; i < total_part; i++)
                {
                    int first = (i * n_page) + 1;
                    int last = (i * n_page) + n_page;
                    if (last > total_page)
                        last = total_page;

                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + first + "_" + last + ".pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, first, last, false, maxprogressvalue);
                    }
                    else
                    {
                        sm.split(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, first, last, false, maxprogressvalue);
                    }

                cancel: ;
                    if (!iscanceled)
                    {
                        iscanceled = true;
                    }
                }

                #endregion forloop
                
                tmp_counter++;
                if (success)
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Successfull";
                    }
                }
                else
                {
                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                    }
                    else
                    {
                        btn.Enabled = false;
                        lbl.Text = "Splitting Unuccessfull";
                    }
                }
   
            }
            #endregion foreach

        
            
        }

        public void start_thread_for_equal_file_size_splitting(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue, long desired_file_size,long[] input_file_size)
        {
            int row = 0;

            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Splitting Running ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                    pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                    start_equal_file_size_splitting(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressvalue, desired_file_size,input_file_size);



                    //c++;
                    break;
                }

            }
        }

        public void start_equal_file_size_splitting(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue, long desired_file_size,long[] input_file_size)
        {
            int pgb_current_value = 0;
            int lowbound = 0;
            int upper_bound = 0;
            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool iscanceled = false;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            if (invoking_required)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum ));
                tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Maximum = maxprogressvalue));
                tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                // 
            }

            #region foreach
            
            foreach (PdfReader pdfreader in pr)
            {
                if (btn.InvokeRequired)
                    btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                else
                    btn.Enabled = true;

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting " + Path.GetFileName(input_pdf[tmp_counter])));
                                 
                    // 
                }
               
                #region forloop

                bool result = false;
                Document document = new Document();
                PdfImportedPage page = null;
                PdfCopy pdfCpy = null;
               // int n = 0;
                int counter;
                int previous = 1;
               // ProgressBar_lower_panel.Maximum = n;
                //left_StatusLabel.Text = "Splitting PDF";
                for (int i = 1; i <=  total[tmp_counter]; )
                {
                    result = false;
                    
                    try
                    {
                        string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " +i.ToString("00000") + ".pdf", RegexOptions.IgnoreCase);
                        file_path = folder + "\\" + file;

                        if (File.Exists(file_path) && (!can_replace))
                        {

                            tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                            tmpsvd.FileName = Path.GetFileName(file_path);
                            if (tmpsvd.ShowDialog() == DialogResult.OK)
                            {
                                file_path = tmpsvd.FileName;
                            }
                            else
                            { goto cancel; }
                        }

                        pdfreader.ConsolidateNamedDestinations();
                        document = new iTextSharp.text.Document(pdfreader.GetPageSizeWithRotation(1));
                        //MessageBox.Show(output_file_name);
                        pdfCpy = new PdfCopy(document, new FileStream(file_path, FileMode.Create));
                        document.Open();
                        counter = 0;
                    }
                    catch (IOException ex)
                    {
                        if (invoking_required)
                        {
                            
                            tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                            tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                        }
                        goto finish;
                    }
                    

                    for (int j = previous; j <= total[tmp_counter]; j++)
                    {
                        try
                        {
                            page = pdfCpy.GetImportedPage(pdfreader, j);
                            pdfCpy.AddPage(page);
                            counter++;
                            FileInfo f = new FileInfo(file_path);
                            long l = f.Length;


                            if (invoking_required)
                            {
                                if (pgb_current_value <= upper_bound && pgb_current_value >= lowbound)
                                {
                                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Value = pgb_current_value));
                                }
                            }
                            pgb_current_value++;

                            if (l >= desired_file_size)
                            {
                                document.Close();
                                result = true;
                                previous = j + 1;
                                break;
                            }
                        }
                        
                        catch (IOException ex)
                        {
                            if (invoking_required)
                            {
                                MessageBox.Show(ex.Message);
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                            }
                            success = false;
                            goto finish;
                        }
                        catch (ThreadAbortException ex)
                        {

                            if (invoking_required)
                            {
                                
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                            }
                            success = false;
                            goto finish;
                        }
                        catch (Exception ex)
                        {
                            
                            if (invoking_required)
                            {
                                //    MessageBox.Show(ex.Message);
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                            }
                            success = false;
                            goto finish;
                        }
                        

                    }
                    pdfreader.Close();
                    i = i + counter;
                    document.Close();
                    result = true;



                cancel: ;
                    if (!iscanceled)
                    {
                        iscanceled = true;
                    }

                }

                #endregion forloop

                tmp_counter++;
                

            }
            #endregion foreach

            if (success)
            {
                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Successfull"));
                }
                else
                {
                    btn.Enabled = false;
                    lbl.Text = "Splitting Successfull";
                }
            }
            else
            {
                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Unuccessfull"));
                }
                else
                {
                    btn.Enabled = false;
                    lbl.Text = "Splitting Unuccessfull";
                }
            }

        finish: ;

        }

        public void start_thread_for_removing_blank_pages(TableLayoutPanel tablelayoutpanel, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue,int reader_count)
        {
            int row = 0;
            List<int>[] without_blank_pages = new List<int>[reader_count];
            Thread c_th = Thread.CurrentThread;

            int l_column_width = 0;
            int p_column_width = 0;
            int b_coulmn_width = 0;

            if (tablelayoutpanel.InvokeRequired)
            {
                tablelayoutpanel.Invoke((MethodInvoker)(() => row = tablelayoutpanel.RowCount - 2));

                tablelayoutpanel.Invoke((MethodInvoker)(() => l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10));
                tablelayoutpanel.Invoke((MethodInvoker)(() => p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15));
                tablelayoutpanel.Invoke((MethodInvoker)(() => b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5));
            }
            else
            {
                l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
                p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
                b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

            }

            for (int k = 1; k < max; k++)
            {
                splitting_and_merging_class smc = new splitting_and_merging_class();
                if (isused[k] == false)
                {
                    c = k;
                    isused[k] = true;

                    //label initialization
                    lbl[c] = new Label();
                    lbl[c].Dock = DockStyle.Fill;
                    lbl[c].Text = "Blank Page Removing ";
                    lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lbl[c].AutoSize = true;

                    //progressbar initialization
                    pgb[c] = new ProgressBar();
                   // pgb[c].Maximum = maxprogressvalue;
                    pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                    pgb[c].Dock = DockStyle.Fill;

                    //cancel button initialzation
                    btn[c] = new Button();
                    btn[c].Click += ((obj, ep) => cancel_splitting_click(obj, ep, c_th));
                    btn[c].Name = c.ToString();
                    btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                    btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;


                    //add label
                    //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(lbl[c], 0, row)));

                        //add progressbar
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(pgb[c], 1, row)));

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.Controls.Add(btn[c], 2, row)));

                        //add row to tablelayoutpanel
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F))));
                    }
                    else
                    {
                        tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                        //add progressbar
                        tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                        //add cancal btn
                        // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                        tablelayoutpanel.Controls.Add(btn[c], 2, row);
                        //add row to tablelayoutpanel
                        tablelayoutpanel.RowCount = tablelayoutpanel.RowCount + 1;
                        tablelayoutpanel.ColumnCount = tablelayoutpanel.ColumnCount + 1;
                        tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    }
                  
                    without_blank_pages = search_blank_pages(tablelayoutpanel,lbl[c],btn[c], c_th, input_pdf, pr, total, reader_count);
                    if (tablelayoutpanel.InvokeRequired)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => pgb[c].Maximum = maxprogressval_for_blank_page));
                    }
                    else
                    {
                        pgb[c].Maximum = maxprogressval_for_blank_page;
                    }
                    if (without_blank_pages != null)
                    {
                        
                        start_removing_blank_pages(tablelayoutpanel, lbl[c], pgb[c], btn[c], folder, pr, input_pdf, output_pdf, total, can_replace, maxprogressval_for_blank_page, without_blank_pages);
                    }

                    //c++;
                    break;
                }
                
            }
        }
        private int blank_page_found_reader_count = 0;
        private int maxprogressval_for_blank_page = 0;
        public List<int>[] search_blank_pages(TableLayoutPanel tablelayoutpanel,Label lbl,Button btn,Thread c_th, string[] inputfiles, PdfReader[] pr, int[] totalpage, int reader_count)
        {
            List<int>[] blank_pages = new List<int>[reader_count];
            List<int>[] without_blank_page = new List<int>[reader_count];
            int tmp = 0,c=0;
            bool invoking_required = tablelayoutpanel.InvokeRequired;
            List<int>[] modified_without_blank_page = new List<int>[reader_count];
           
           
            foreach (PdfReader pdfreader in pr)
            {
                tmp = c;
                without_blank_page[tmp] = new List<int>();


                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Searching blank page in " + Path.GetFileName(inputfiles[tmp])));

                }
                else
                { 
                     lbl.Text = "Searching blank page in " + Path.GetFileName(inputfiles[tmp]);
                }
                            

                for (int i = 1; i <= totalpage[tmp]; i++)
                {
                    try
                    {
                        PdfDictionary pg = pr[tmp].GetPageN(i);
                        // recursively search pages, forms and groups for images.
                        PdfObject obj = FindImageInPDFDictionary(pg);
                        string str = PdfTextExtractor.GetTextFromPage(pr[tmp], i);

                        if (str == "" && obj == null)
                        {
                            //  blank_pages[tmp].Add(i);
                        }
                        else
                        {
                            without_blank_page[tmp].Add(i);
                            maxprogressval_for_blank_page++;
                        }
                    }
                    catch (ThreadAbortException ex)
                    {

                        if (invoking_required)
                        {

                            tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                            tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing blank page Canceled"));
                        }
                       return null;
                    }
                }

                //reader count
                c++;
                if (without_blank_page[tmp].Count < totalpage[tmp])
                {
                    modified_without_blank_page[blank_page_found_reader_count] = new List<int>();
                    modified_without_blank_page[blank_page_found_reader_count] = without_blank_page[blank_page_found_reader_count];
                    blank_page_found_reader_count++;
                    
                }
                else
                {
                    MessageBox.Show("Blank page does not exist in " + Path.GetFileName(inputfiles[tmp]));
                }
            }
            if (blank_page_found_reader_count > 0)
            {
                return modified_without_blank_page;
            }
            else
            {

                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Could not found Blank Page "));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled=false));
                }
                else
                {
                    lbl.Text = "Could not found Blank Page ";
                    btn.Enabled = false;
                }
                return null;
            }
        }
       
        public void start_removing_blank_pages(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue, List<int>[] without_blank_pages)
        {
            int lowbound = 0;
            int upper_bound = 0;

            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            bool success = true;
            bool invoking_required = tablelayoutpanel.InvokeRequired;

            foreach (PdfReader pdfreader in pr)
            {
                    if (btn.InvokeRequired)
                        btn.Invoke((MethodInvoker)(() => btn.Enabled = true));
                    else
                        btn.Enabled = true;

                    if (invoking_required)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing Blank Pages from " + Path.GetFileName(input_pdf[tmp_counter])));
                        //  tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Minimum = first_page));
                      
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lowbound = pgb.Minimum));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => upper_bound = pgb.Maximum));
                        // 
                    }
                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " -blank page removed " +  "_.pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;
                  
                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    if (success)
                    {
                        success = sm.remove_blank_Pages (tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, can_replace, maxprogressvalue,without_blank_pages[tmp_counter],lowbound,upper_bound);
                    }
                    else
                    {
                        sm.remove_blank_Pages(tablelayoutpanel, lbl, pgb, btn, pdfreader, input_pdf[tmp_counter], file_path, can_replace, maxprogressvalue, without_blank_pages[tmp_counter],lowbound,upper_bound);
                    }

                cancel: ;
                 tmp_counter++;
                 if (tmp_counter == blank_page_found_reader_count)
                     break;
           }

            if (success)
            {
                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Blank pages removed Successfully"));
                }
                else
                {
                    btn.Enabled = false;
                    lbl.Text = "Blank pages removed Successfully";
                }
            }
            else
            {
                if (invoking_required)
                {
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing Blank pages Unsuccessfull"));
                }
                else
                {
                    btn.Enabled = false;
                    lbl.Text = "Removing Blank pages Unsuccessfull";
                }
            }     
            
        }

        public  PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));


            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {

                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type =
                          (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }

                    }
                }
            }

            return null;

        }

        /*  public void backup_threaded_splitting(string folder, PdfReader[] pr, string[] input_pdf, string output_pdf, int[] total, bool can_replace, int maxprogressvalue)
        {
            splitting_and_merging_class sm = new splitting_and_merging_class();
            SaveFileDialog tmpsvd = new SaveFileDialog();
            tmpsvd.RestoreDirectory = true;
            tmpsvd.Title = "Save File";
            tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
            string file_path = "";
            int tmp_counter = 0;
            foreach (PdfReader pdfreader in pr)
            {
                for (int i = 1; i <= total[tmp_counter]; i++)
                {

                    string file = Regex.Replace(input_pdf[tmp_counter], @".pdf", " - " + (i).ToString("00000") + ".pdf", RegexOptions.IgnoreCase);
                    file_path = folder + "\\" + file;

                    if (File.Exists(file_path) && (!can_replace))
                    {

                        tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                        tmpsvd.FileName = Path.GetFileName(file_path);
                        if (tmpsvd.ShowDialog() == DialogResult.OK)
                        {
                            file_path = tmpsvd.FileName;
                        }
                        else
                        { goto cancel; }
                    }

                    sm.split( pdfreader, file_path, i, i, false, maxprogressvalue);

                cancel: ;
                }
                tmp_counter++;
            }
        }

       */ 
       
        /*   public void start_BackgroundWorker_for_splitting(object sender, EventArgs e,TableLayoutPanel tablelayoutpanel,int progressbarmaxvalue, string path, PdfReader[] pr, string[] input_pdf, string output_pdf, int total, bool can_replace)
       {
           int row = tablelayoutpanel.RowCount - 2;
           int tmpcounter=0;
           int l_column_width = (int)tablelayoutpanel.ColumnStyles[0].Width - 10;
           int p_column_width = (int)tablelayoutpanel.ColumnStyles[1].Width - 15;
           int b_coulmn_width = (int)tablelayoutpanel.ColumnStyles[2].Width - 5;

           foreach (PdfReader pdfreader in pr)
           {
                for (int k = 1; k < max; k++)
               {
                    splitting_and_merging_class smc = new splitting_and_merging_class();
                   if (isused[k] == false)
                   {
                       c = k;
                       isused[k] = true;

                       //  if (c == 1 && !(ispgbrfopen))
                       //{
                       //pgbrf.Show();
                       //  ispgbrfopen = true;
                       //}

                        
                       #region dynamic progress initialization
                        //label initialization
                       lbl[c] = new Label();
                       lbl[c].Size = new System.Drawing.Size(230, 15);
                       lbl[c].Text = "Merging as  " + Path.GetFileName(outputfile);
                       lbl[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                       lbl[c].AutoSize = true;


                       //cancel button initialzation
                       btn[c] = new Button();
                       btn[c].Click += CancelOperation_Click;
                       btn[c].Name = c.ToString();
                       btn[c].Size = new System.Drawing.Size(b_coulmn_width, 20);
                       btn[c].Image = global::_project_pdf_.Properties.Resources.cancelOperation;

                       //progressbar initialization
                       pgb[c] = new ProgressBar();
                       pgb[c].Maximum = progressbarmaxvalue;
                       pgb[c].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                       pgb[c].Dock = DockStyle.Fill;



                       //backgroundworker initialization
                       bgw[c] = new BackgroundWorker();
                      // bgw[c].DoWork += ((obj, ep) => smc.merge(obj, ep, pr, outputfile, progressbarmaxvalue, bgw, lbl, pgb, tablelayoutpanel, btn, isused));
                       bgw[c].DoWork += ((obj, ep) => splittinginitialization(obj,ep, path,pdfreader,input_pdf[tmpcounter],output_pdf,pdfreader.NumberOfPages,can_replace));
                      // object sender, DoWorkEventArgs e,BackgroundWorker[] bgw,Label[] lbl,ProgressBar[] pgb,TableLayoutPanel tablelayoutpanel,Button[] btn,bool[] isused,

                       //object sender, DoWorkEventArgs e, string path, PdfReader pr, string input_pdf, string output_pdf, int total, bool can_replace
                        
                        
                       bgw[c].ProgressChanged += ((obj, ep) => ProgressChanged(obj, ep));
                       bgw[c].WorkerReportsProgress = true;
                       bgw[c].WorkerSupportsCancellation = true;
                       #endregion dynamic progress initialization
                        


                       //add progressbar
                       // pgbrf.flowLayoutPanel1.Controls.Add(pgb[c]);
                       // mainformobj.tabPagefoProgressBar.Controls.Add(pgb[c]);
                       tablelayoutpanel.Controls.Add(pgb[c], 1, row);

                       //add cancal btn
                       // pgbrf.flowLayoutPanel1.Controls.Add(btn[c]);
                       tablelayoutpanel.Controls.Add(btn[c], 2, row);

                       //all label
                       //pgbrf.flowLayoutPanel1.Controls.Add(lbl[c]);
                       tablelayoutpanel.Controls.Add(lbl[c], 0, row);

                       //backgroundworker start              
                       bgw[c].RunWorkerAsync((object)c);

                       //add row to tablelayoutpanel
                       tablelayoutpanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                       //c++;
                       break;
                   }

               } 
           }

           
       }

       public void splittinginitialization(object sender, DoWorkEventArgs e, string path, PdfReader pr, string input_pdf, string output_pdf, int total, bool can_replace)
       {

           Thread.CurrentThread.ApartmentState = ApartmentState.STA;
           splitting_and_merging_class sm = new splitting_and_merging_class();
           SaveFileDialog tmpsvd = new SaveFileDialog();
           tmpsvd.RestoreDirectory = true;
           tmpsvd.Title = "Save File";
           tmpsvd.Filter = "PDF Documents (*.pdf)|*.pdf";
           string file_path = "";
           for (int i = 1; i <= total; i++)
           {

               string file = Regex.Replace(input_pdf, @".pdf", " - " + (i).ToString("00000") + ".pdf", RegexOptions.IgnoreCase);
               file_path = path + "\\" + file;

               if (File.Exists(file_path) && (!can_replace))
               {

                   tmpsvd.InitialDirectory = Path.GetDirectoryName(file_path);
                   tmpsvd.FileName = Path.GetFileName(file_path);
                   if (tmpsvd.ShowDialog() == DialogResult.OK)
                   {
                       file_path = tmpsvd.FileName;
                   }
                   else
                   { goto cancel; }
               }


               sm.split(pr, file_path, i, i, false);

           cancel: ;
           }
       }
*/
    
    
    }
}
