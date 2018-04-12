using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using iTextSharp.text.pdf;


namespace _project_pdf_
{
    class bookmark_class
    {

        TreeNode tn;
        ContextMenuStrip contextMenuStrip1;
        public bookmark_class(ContextMenuStrip contxtmenustrip)
        {
            contextMenuStrip1 = contxtmenustrip;
        }
   
        public void recursive_func( IList<Dictionary<string, object>> ilist,TreeNode tnt)
        {
            try
            {
                foreach (Dictionary<string, object> bk in ilist)
                {

                    foreach (KeyValuePair<string, object> kvr in bk)
                    {

                        //have child node
                        if (kvr.Key == "Kids" || kvr.Key == "kids")
                        {
                            IList<Dictionary<string, object>> child = (IList<Dictionary<string, object>>)kvr.Value;
                            recursive_func(child, tn);
                        }

                        //add bkmark name to treenode
                        else if (kvr.Key == "Title" || kvr.Key == "title")
                        {
                            tn = new System.Windows.Forms.TreeNode(kvr.Value.ToString());
                        }

                        //add bkmark page number to treenode
                        else if (kvr.Key == "Page" || kvr.Key == "page")
                        {
                            tn.ContextMenuStrip = contextMenuStrip1;
                            tn.Tag = Regex.Match(kvr.Value.ToString(), "[0-9]+").Value;
                            tnt.Nodes.Add(tn);
                        }
                        else //do nothing
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            
        }

        public bool GetBKTreeView(PdfReader pdfreader,ref TreeView trv)
        {

            try
            {
                IList<Dictionary<string, object>> bookmark = SimpleBookmark.GetBookmark(pdfreader);

                foreach (Dictionary<string, object> bk in bookmark)
                {

                    foreach (KeyValuePair<string, object> kvr in bk)
                    {
                        //have child node
                        if (kvr.Key == "Kids" || kvr.Key == "kids")
                        {
                            IList<Dictionary<string, object>> child = (IList<Dictionary<string, object>>)kvr.Value;
                            recursive_func(child, tn);
                        }

                        //add bkmark name to treenode
                        else if (kvr.Key == "Title" || kvr.Key == "title")
                        {
                            tn = new System.Windows.Forms.TreeNode(kvr.Value.ToString());
                        }

                        //add bkmark page number to treenode
                        else if (kvr.Key == "Page" || kvr.Key == "page")
                        {
                            tn.ContextMenuStrip = contextMenuStrip1;
                            tn.Tag = Regex.Match(kvr.Value.ToString(), "[0-9]+").Value;
                            trv.Nodes.Add(tn);
                        }
                        else //do nothing
                        {

                        }
                    }
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
