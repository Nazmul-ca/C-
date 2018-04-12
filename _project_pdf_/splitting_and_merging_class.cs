using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;


namespace _project_pdf_
{
    class splitting_and_merging_class 
    {
        #region  global_declaration
        enum operation_return_type { error_in_opening_file = 0, can_not_save_file = 1, user_pressed_cancel = 2 };

        #endregion
       

        public bool merge(object sender, DoWorkEventArgs e, PdfReader[] pr, string output_pdf,  int progressbarmaxvalue, BackgroundWorker[] bgw,Label[] lbl,ProgressBar[] pgb,TableLayoutPanel tablelayoutpanel,Button[] btn,bool[] isused)
        {
           
            supporting_class sc = new supporting_class();
           // PdfReader reader = null;
            Document document = new Document();
            PdfImportedPage page = null;
            PdfCopy pdfCpy = null;
            int n = 0;
            int totalPages = 0;
            int page_offset = 0;
            int counter = 0;
            int progress = 0;
            bool canceled = false;
           // for (int i = 0; i <= sourcePdfs.Length - 1; i++)
             //   reader = new PdfReader(sourcePdfs[0]);

            List<Dictionary<string, object>> bookmarks = new List<Dictionary<string, object>>();
            IList<Dictionary<string, object>> tempBookmarks;

            try
            {
                foreach (PdfReader reader in pr)
                {
                    reader.ConsolidateNamedDestinations();
                    n = reader.NumberOfPages;
                    tempBookmarks = SimpleBookmark.GetBookmark(reader);

                    if (counter == 0)
                    {
                        document = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));
                        pdfCpy = new PdfCopy(document, new FileStream(output_pdf, FileMode.Create));
                        document.Open();
                        SimpleBookmark.ShiftPageNumbers(tempBookmarks, page_offset, null);
                        page_offset += n;
                        if (tempBookmarks != null)
                            bookmarks.AddRange(tempBookmarks);

                        totalPages = n;
                        counter++;
                    }
                    else
                    {
                        SimpleBookmark.ShiftPageNumbers(tempBookmarks, page_offset, null);
                        if (tempBookmarks != null)
                            bookmarks.AddRange(tempBookmarks);

                        page_offset += n;
                        totalPages += n;

                    }

                    for (int j = 1; j <= n; j++)
                    {
                        Application.DoEvents();
                        
                        if (bgw[Convert.ToInt32(e.Argument)].CancellationPending == true)
                        {
                            e.Cancel = true;


                            if (tablelayoutpanel.InvokeRequired)
                            {
                                // flowlayoutpanel1.Invoke((MethodInvoker)(() => flowlayoutpanel1.Controls.Remove(pgb[Convert.ToInt32(e.Argument)])));
                                canceled = true;
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl[Convert.ToInt32(e.Argument)].Text = "Merging Canceled"));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn[Convert.ToInt32(e.Argument)].Enabled = false));
                                // flowlayoutpanel1.Invoke((MethodInvoker)(() => flowlayoutpanel1.Controls.Remove(btn[Convert.ToInt32(e.Argument)])));
                                // isused[Convert.ToInt32(e.Argument)] = false;
                            }
                            break;
                        }
                        else
                        {
                            page = pdfCpy.GetImportedPage(reader, j);
                            pdfCpy.AddPage(page);
                            progress++;
                            bgw[Convert.ToInt32(e.Argument)].ReportProgress(progress, e.Argument);

                        }

                    }

                    reader.Close();

                }
                pdfCpy.Outlines = bookmarks;
                document.Close();
                // isused[Convert.ToInt32(e.Argument)] = false;
                if (tablelayoutpanel.InvokeRequired)
                {
                    if (!canceled)
                    {
                        tablelayoutpanel.Invoke((MethodInvoker)(() => btn[Convert.ToInt32(e.Argument)].Enabled = false));
                        tablelayoutpanel.Invoke((MethodInvoker)(() => lbl[Convert.ToInt32(e.Argument)].Text = "Merging Successfull"));

                    }
                }
                return true;
            }
            catch (IOException ex)
            {
                if (tablelayoutpanel.InvokeRequired)
                {
                    MessageBox.Show(ex.Message);
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn[Convert.ToInt32(e.Argument)].Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl[Convert.ToInt32(e.Argument)].Text = "Merging UnSuccessfull"));
                }
                return false;           
            }
            catch (Exception ex)
            {
                if (tablelayoutpanel.InvokeRequired)
                {
                    show_message_box(ex.ToString());
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn[Convert.ToInt32(e.Argument)].Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl[Convert.ToInt32(e.Argument)].Text = "Merging UnSuccessfull"));
                }
                return false;
            }
        
        }

        public void show_message_box(string text)
        {
            MessageBox.Show(text);
        }
        public bool split(TableLayoutPanel tablelayoutpanel, Label lbl,ProgressBar pgb,Button btn, PdfReader inputPdf,string input_file_name, string output_file_path, int first_page, int last_page, bool should_status_update,int maxprogressval)
        {
            bool invoking_required = tablelayoutpanel.InvokeRequired;
            try
            {
              //  int lowbound=0;
               // int upper_bound = 0;
                
                // retrieve the total number of pages
                int pageCount = inputPdf.NumberOfPages;

                if (last_page < first_page || last_page > pageCount)
                {
                    last_page = pageCount;
                }


                // load the input document
                Document inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

                // create the filestream
                if (File.Exists(output_file_path))
                {
                    File.Delete(output_file_path);
                }
                using (FileStream fs = new FileStream(output_file_path, FileMode.CreateNew))
                {
                    // create the output writer
                    PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();
                    PdfContentByte cb1 = outputWriter.DirectContent;



                    // copy pages from input to output document
                    // MessageBox.Show(start + "\n" + end);
                    for (int i = first_page; i <= last_page; i++)
                    {
                        Application.DoEvents();
                        try
                        {
                            inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                            inputDoc.NewPage();

                            PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);
                            int rotation = inputPdf.GetPageRotation(i);

                            if (rotation == 90 || rotation == 270)
                            {
                                cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                                    inputPdf.GetPageSizeWithRotation(i).Height);
                            }
                            else
                            {
                                cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }

                            if (tablelayoutpanel.InvokeRequired)
                            {
                               // if(i%100==0)
                               // show_message_box(i.ToString()+"\t"+first_page);
                                if (i <= pgb.Maximum && i >=pgb.Minimum )
                                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Value = i));
                             
                            }
                            else
                            {
                                if (i <= pgb.Maximum && i >= pgb.Minimum)
                                     pgb.Value = i;  
                            }
                            //  MessageBox.Show(start + "\n" + end);
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.ToString());
                        }


                    }
                    FileInfo f2 = new FileInfo(output_file_path);
                    long l = f2.Length;
                    // MessageBox.Show(outputFile + "size " + l.ToString());
                    inputDoc.Close();

                }

                
                return true;
            }
            catch (IOException ex)
            {
                if (invoking_required)
                {
                    MessageBox.Show(ex.Message);
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                }
                return false;
            }
            catch (ThreadAbortException ex)
            {
               
                if (invoking_required)
                {
                    
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                }
                return false;
            }
            catch (Exception ex)
            {
              // show_message_box(ex.ToString()+"\n"+ex.Message);
                if (invoking_required)
                {
                    //    MessageBox.Show(ex.Message);
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                }
                return false;
            }
        }

        public bool split_for_search(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, PdfReader inputPdf, string input_file_name, string output_file_path, int first_page, int last_page, bool should_status_update, int maxprogressval)
        {
            bool invoking_required = tablelayoutpanel.InvokeRequired;
            try
            {
                //  int lowbound=0;
                // int upper_bound = 0;

                // retrieve the total number of pages
                int pageCount = inputPdf.NumberOfPages;

                if (last_page < first_page || last_page > pageCount)
                {
                    last_page = pageCount;
                }


                // load the input document
                Document inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

                // create the filestream
                if (File.Exists(output_file_path))
                {
                    File.Delete(output_file_path);
                }
                using (FileStream fs = new FileStream(output_file_path, FileMode.CreateNew))
                {
                    // create the output writer
                    PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();
                    PdfContentByte cb1 = outputWriter.DirectContent;



                    // copy pages from input to output document
                    // MessageBox.Show(start + "\n" + end);
                   
                    for (int i = first_page; i <= last_page; i++)
                    {
                        Application.DoEvents();
                        try
                        {
                            inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                            inputDoc.NewPage();

                            PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);
                            int rotation = inputPdf.GetPageRotation(i);

                            if (rotation == 90 || rotation == 270)
                            {
                                cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                                    inputPdf.GetPageSizeWithRotation(i).Height);
                            }
                            else
                            {
                                cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }

                            if (tablelayoutpanel.InvokeRequired)
                            {

                                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Value = (pgb.Value+1)));
                                   // show_message_box(pgb.Minimum + "\t" + pgb.Maximum);
                            }
                            else
                            {
                                if (pgb.Value < (pgb.Maximum - 1))
                                   pgb.Value = (pgb.Value + 1);
                            }
                            //  MessageBox.Show(start + "\n" + end);
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.ToString());
                        }


                    }
                    FileInfo f2 = new FileInfo(output_file_path);
                    long l = f2.Length;
                    // MessageBox.Show(outputFile + "size " + l.ToString());
                    inputDoc.Close();

                }


                return true;
            }
            catch (IOException ex)
            {
                if (invoking_required)
                {
                    MessageBox.Show(ex.Message);
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                }
                return false;
            }
            catch (ThreadAbortException ex)
            {

                if (invoking_required)
                {

                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting Canceled"));
                }
                return false;
            }
            catch (Exception ex)
            {
                // show_message_box(ex.ToString()+"\n"+ex.Message);
                if (invoking_required)
                {
                    //    MessageBox.Show(ex.Message);
                    tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                    tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Splitting UnSuccessfull"));
                }
                return false;
            }
        }

        public bool remove_blank_Pages(TableLayoutPanel tablelayoutpanel, Label lbl, ProgressBar pgb, Button btn, PdfReader inputPdf, string inputFile, string outputFile,bool can_replace, int maxprogressvalue, List<int> without_blank_pages,int low_bound,int up_bound)
        {
            bool invoking_required = tablelayoutpanel.InvokeRequired;
            int c_val = 0;
            int d_val = 0;
            try
            {
                Document inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

                // create the filestream
                if (File.Exists(outputFile))
                {
                    try
                    {
                      
                        File.Delete(outputFile);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }

                }

                using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                {
                    // create the output writer
                    PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();
                    PdfContentByte cb1 = outputWriter.DirectContent;

                    // copy pages from input to output document
                    foreach (int i in without_blank_pages)
                    {
                        Application.DoEvents();
                        try
                        {
                            if (invoking_required)
                            {
                                tablelayoutpanel.Invoke((MethodInvoker)(() => c_val = pgb.Value));

                            }
                            else
                            {
                                c_val = pgb.Value;
                            }
                            inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                            inputDoc.NewPage();

                            PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);

                            int rotation = inputPdf.GetPageRotation(i);

                            if (rotation == 90 || rotation == 270)
                            {
                                cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                                    inputPdf.GetPageSizeWithRotation(i).Height);
                            }
                            else
                            {
                                cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }
                            if (invoking_required )
                            {
                                                               
                                if(c_val< up_bound)
                                {
                                    tablelayoutpanel.Invoke((MethodInvoker)(() => pgb.Value = (pgb.Value + 1)));
                                }
                                // show_message_box(pgb.Minimum + "\t" + pgb.Maximum);
                            }
                            else
                            {
                                if (c_val < up_bound)
                                    pgb.Value = (pgb.Value + 1);
                            }

                        }
                        catch (IOException ex)
                        {
                            if (invoking_required)
                            {
                                MessageBox.Show(ex.Message);
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing Blank Page UnSuccessfull"));
                            }
                            return false;
                        }
                        catch (ThreadAbortException ex)
                        {

                            if (invoking_required)
                            {

                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing Blank Page Canceled"));
                            }
                            return false;
                        }
                        catch (Exception ex)
                        {
                            // show_message_box(ex.ToString()+"\n"+ex.Message);
                            if (invoking_required)
                            {
                                //    MessageBox.Show(ex.Message);
                                tablelayoutpanel.Invoke((MethodInvoker)(() => btn.Enabled = false));
                                tablelayoutpanel.Invoke((MethodInvoker)(() => lbl.Text = "Removing Blank Page UnSuccessfull"));
                            }
                            return false;
                        }
                       
                    }
                    //  FileInfo f2 = new FileInfo(outputFile);
                    //long l = f2.Length;
                    // MessageBox.Show(outputFile + "size " + l.ToString());
                    inputDoc.Close();

                }
                return true;
            }

            catch (Exception ex)
            {
                show_message_box(ex.ToString());
                return false;
            }
        }

        public bool split_Page_by_keyboard(PdfReader inputPdf, string outputFile, int start, int end)
        {

            try
            {
                // load the input document
                Document inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

                // create the filestream
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                using (FileStream fs = new FileStream(outputFile, FileMode.CreateNew))
                {
                    // create the output writer
                    PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();
                    PdfContentByte cb1 = outputWriter.DirectContent;
                 
                    for (int i = start; i <= end; i++)
                    {
                        Application.DoEvents();
                        try
                        {
                            inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                            inputDoc.NewPage();

                            PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);

                            int rotation = inputPdf.GetPageRotation(i);

                            if (rotation == 90 || rotation == 270)
                            {
                                cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                                    inputPdf.GetPageSizeWithRotation(i).Height);
                            }
                            else
                            {
                                cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.ToString());
                        }


                    }
      
                    inputDoc.Close();

                }
     
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
