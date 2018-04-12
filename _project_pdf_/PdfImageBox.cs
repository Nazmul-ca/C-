using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using nainam.GS.pdf2image;
using nainam.GS;
using System.Windows.Forms;

namespace Cyotek.Windows.Forms
{
  // Cyotek PdfImageBox
  // Copyright (c) 2011 Cyotek. All Rights Reserved.
  // http://cyotek.com

  // If you use this control in your applications, attribution or donations are welcome.

  /// <summary>
  /// Control for displaying PDF files by converting them on the fly to raster images
  /// </summary>
  [DefaultEvent("PageLoading"), DefaultProperty("Settings")]
  public class PdfImageBox : ImageBox
  {
  #region  Private Member Declarations  

    private Pdf2Image _converter;
    private int _currentPage;
    private object _lock = new object();
    private IDictionary<int, Bitmap> _pageCache;
    private string _pdfFileName;
    private string _pdfPassword;
    private Pdf2ImageSettings _settings;

  #endregion  Private Member Declarations  

  #region  Public Constructors  

    /// <summary>
    ///  Initializes a new instance of the PdfImageBox class.
    /// </summary>
    public PdfImageBox()
    {
      // override some of the original ImageBox defaults
      this.GridDisplayMode = ImageBoxGridDisplayMode.None;
      this.BackColor = SystemColors.AppWorkspace;
      this.ImageBorderStyle = ImageBoxBorderStyle.FixedSingleDropShadow;

      // new pdf conversion settings
      this.Settings = new Pdf2ImageSettings();
    }

  #endregion  Public Constructors  

  #region  Events  

    /// <summary> Event queue for all listeners interested in CurrentPageChanged events. </summary>
    public event EventHandler CurrentPageChanged;

    /// <summary> Event queue for all listeners interested in LoadedPage events. </summary>
    public event EventHandler LoadedPage;

    /// <summary> Event queue for all listeners interested in LoadingPage events. </summary>
    public event EventHandler LoadingPage;

    /// <summary> Event queue for all listeners interested in PdfFileNameChanged events. </summary>
    public event EventHandler PdfFileNameChanged;

    /// <summary> Event queue for all listeners interested in PdfPasswordChanged events. </summary>
    public event EventHandler PdfPasswordChanged;

    /// <summary> Event queue for all listeners interested in SettingsChanged events. </summary>
    public event EventHandler SettingsChanged;

  #endregion  Events  

  #region  Overriden Properties  

    [DefaultValue(typeof(Color), "AppWorkspace")]
    public override Color BackColor
    {
      get { return base.BackColor; }
      set { base.BackColor = value; }
    }

    [DefaultValue(typeof(ImageBoxGridDisplayMode), "None")]
    public override ImageBoxGridDisplayMode GridDisplayMode
    {
      get { return base.GridDisplayMode; }
      set { base.GridDisplayMode = value; }
    }

    [DefaultValue(typeof(ImageBoxBorderStyle), "FixedSingleDropShadow")]
    public override ImageBoxBorderStyle ImageBorderStyle
    {
      get { return base.ImageBorderStyle; }
      set { base.ImageBorderStyle = value; }
    }

  #endregion  Overriden Properties  

  #region  Protected Overridden Methods  

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">  true if managed resources should be disposed; otherwise, false. </param>
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        if (_settings != null)
          _settings.PropertyChanged -= SettingsPropertyChangedHandler;

        this.CleanUp();
      }
    }

  #endregion  Protected Overridden Methods  

  #region  Public Methods  

    /// <summary>
    ///  Navigates to the first page.
    /// </summary>
    public void FirstPage()
    {
      this.CurrentPage = 1;
    }

    /// <summary>
    ///  Navigates to the last page.
    /// </summary>
    public void LastPage()
    {
      this.CurrentPage = this.PageCount;
    }

    /// <summary>
    ///  Navigates to the next page.
    /// </summary>
    public void NextPage()
    {
       // MessageBox.Show("u");
      this.CurrentPage++;
    //  MessageBox.Show("ul");
    }

    /// <summary>
    ///  Opens a PDF.
    /// </summary>
    /// <param name="fileName"> Filename of the file. </param>
    /// <param name="password"> The password. </param>
    public void OpenPDF(string fileName, string password)
    {
      this.PdfFileName = fileName;
      this.PdfPassword = password;
      this.OpenPDF();
    }

    /// <summary>
    ///  Opens a PDF.
    /// </summary>
    public void OpenPDF()
    {
      this.CleanUp();

      if (!this.DesignMode)
      {
        _converter = new Pdf2Image()
        {
          PdfFileName = this.PdfFileName,
          PdfPassword = this.PdfPassword,
          Settings = this.Settings
        };

        this.Image = null;
        this.PageCache = new Dictionary<int, Bitmap>();
        _currentPage = 1;

        if (this.PageCount != 0)
        {
          _currentPage = 0;
          this.CurrentPage = 1;
        }
      }
    }

    /// <summary>
    ///  Navigates to the previous page.
    /// </summary>
    public void PreviousPage()
    {
      this.CurrentPage--;
    }

  #endregion  Public Methods  

  #region  Public Properties  

    /// <summary>
    ///  Gets a value indicating whether we can move first.
    /// </summary>
    /// <value>
    ///  true if we can move first, false if not.
    /// </value>
    [Browsable(false)]
    public bool CanMoveFirst
    { get { return this.PageCount != 0 && this.CurrentPage != 1; } }

    /// <summary>
    ///  Gets a value indicating whether we can move last.
    /// </summary>
    /// <value>
    ///  true if we can move last, false if not.
    /// </value>
    [Browsable(false)]
    public bool CanMoveLast
    { get { return this.PageCount != 0 && this.CurrentPage != this.PageCount; } }

    /// <summary>
    ///  Gets a value indicating whether we can move next.
    /// </summary>
    /// <value>
    ///  true if we can move next, false if not.
    /// </value>
    [Browsable(false)]
    public bool CanMoveNext
    { get { return this.PageCount != 0 && this.CurrentPage < this.PageCount; } }

    /// <summary>
    ///  Gets a value indicating whether we can move previous.
    /// </summary>
    /// <value>
    ///  true if we can move previous, false if not.
    /// </value>
    [Browsable(false)]
    public bool CanMovePrevious
    { get { return this.PageCount != 0 && this.CurrentPage > 1; } }

    /// <summary>
    ///  Gets or sets the current page.
    /// </summary>
    /// <value>
    ///  The current page.
    /// </value>
    [Category("Appearance"), DefaultValue(1)]
    public int CurrentPage
    {
      get { return _currentPage; }
      set
      {
        if (this.CurrentPage != value)
        {
            try
            {
                if (value < 1 || value > this.PageCount)
                    throw new ArgumentException("Page number is out of bounds");

                _currentPage = value;

                this.OnCurrentPageChanged(EventArgs.Empty);
            }
            catch (Exception)
            { ;}
        }
      }
    }

    /// <summary>
    ///  Gets the number of pages.
    /// </summary>
    /// <value>
    ///  The number of pages.
    /// </value>
    [Browsable(false)]
    public virtual int PageCount
    { get { return _converter != null ? _converter.PageCount : 0; } }

    /// <summary>
    ///  Gets or sets the filename of the PDF file.
    /// </summary>
    /// <value>
    ///  The filename of the PDF file.
    /// </value>
    [Category("Behavior"), DefaultValue("")]
    [Editor("System.Windows.Forms.Design.FileNameEditor", typeof(System.Drawing.Design.UITypeEditor))]
    public virtual string PdfFileName
    {
      get { return _pdfFileName; }
      set
      {
        if (this.PdfFileName != value)
        {
          _pdfFileName = value;

          this.OnPdfFileNameChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    ///  Gets or sets the PDF password.
    /// </summary>
    /// <value>
    ///  The PDF password.
    /// </value>
    [Category("Behavior"), DefaultValue("")]
    public virtual string PdfPassword
    {
      get { return _pdfPassword; }
      set
      {
        if (this.PdfPassword != value)
        {
          _pdfPassword = value;

          this.OnPdfPasswordChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    ///  Gets or sets options for controlling the operation.
    /// </summary>
    /// <value>
    ///  The settings.
    /// </value>
    [Category("Appearance"), DefaultValue(typeof(Pdf2ImageSettings), "")]
    public virtual Pdf2ImageSettings Settings
    {
      get { return _settings; }
      set
      {
        if (this.Settings != value)
        {
          if (_settings != null)
            _settings.PropertyChanged -= SettingsPropertyChangedHandler;

          _settings = value;
          _settings.PropertyChanged += SettingsPropertyChangedHandler;

          this.OnSettingsChanged(EventArgs.Empty);
        }
      }
    }

  #endregion  Public Properties  

  #region  Private Methods  

    /// <summary>
    ///  Cleans up generated images.
    /// </summary>
    private void CleanUp()
    {
      // release  bitmaps
      if (this.PageCache != null)
      {
        foreach (KeyValuePair<int, Bitmap> pair in this.PageCache)
          pair.Value.Dispose();
        this.PageCache = null;
      }
    }

    /// <summary>
    ///  Event handler. Called by _settings for property changed events.
    /// </summary>
    /// <param name="sender"> Source of the event. </param>
    /// <param name="e">      Event information to send to registered event handlers. </param>
    private void SettingsPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
      this.OnSettingsChanged(e);
    }

  #endregion  Private Methods  

  #region  Protected Properties  

    protected virtual IDictionary<int, Bitmap> PageCache
    {
      get { return _pageCache; }
      set { _pageCache = value; }
    }

  #endregion  Protected Properties  

  #region  Protected Methods  

    /// <summary>
    ///  Raises the CurrentPageChanged event and updates the active page image.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnCurrentPageChanged(EventArgs e)
    {
      this.SetPageImage();

      if (this.CurrentPageChanged != null)
        this.CurrentPageChanged(this, e);
    }

    /// <summary>
    ///  Raises the LoadedPage event.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnLoadedPage(EventArgs e)
    {
      if (this.LoadedPage != null)
        this.LoadedPage(this, e);
    }

    /// <summary>
    ///  Raises the LoadingPage event.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnLoadingPage(EventArgs e)
    {
      if (this.LoadingPage != null)
        this.LoadingPage(this, e);
    }

    /// <summary>
    ///  Raises the PdfFileNameChanged event.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnPdfFileNameChanged(EventArgs e)
    {
      if (this.PdfFileNameChanged != null)
        this.PdfFileNameChanged(this, e);
    }

    /// <summary>
    ///  Raises the PdfPasswordChanged event.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnPdfPasswordChanged(EventArgs e)
    {
      if (this.PdfPasswordChanged != null)
        this.PdfPasswordChanged(this, e);
    }

    /// <summary>
    ///  Raises the SettingsChanged event and reloads the active PDF file.
    /// </summary>
    /// <param name="e">  Event information to send to registered event handlers. </param>
    protected virtual void OnSettingsChanged(EventArgs e)
    {
      this.OpenPDF();

      if (this.SettingsChanged != null)
        this.SettingsChanged(this, e);
    }

    /// <summary>
    ///  Sets the page image.
    /// </summary>
    protected virtual void SetPageImage()
    {
      if (!this.DesignMode && this.PageCache != null)
      {
        lock (_lock)
        {
          if (!this.PageCache.ContainsKey(this.CurrentPage))
          {
            this.OnLoadingPage(EventArgs.Empty);
            this.PageCache.Add(this.CurrentPage, _converter.GetImage(this.CurrentPage));
            this.OnLoadedPage(EventArgs.Empty);
          }

          this.Image = this.PageCache[this.CurrentPage];
        }
      }
    }

  #endregion  Protected Methods  
  }
}
