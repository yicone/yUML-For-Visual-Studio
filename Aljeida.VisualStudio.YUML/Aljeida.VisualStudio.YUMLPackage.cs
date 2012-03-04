using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Aljeida.VisualStudio.Core;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;
using System.Web;

namespace Aljeida.VisualStudio.YUML
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidAljeida_VisualStudio_YUMLPkgString)]
    public sealed class YUMLPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public YUMLPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidAljeida_VisualStudio_YUMLCmdSet, (int)PkgCmdIDList.cmdidViewClassDiagramByYUML);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string yUMLUrl;

            SolutionParser parser = new SolutionParser();
            ProjectItem codeProjectItem = parser.GetSelectedFirstCodeProjectItem();
            if (codeProjectItem == null) return;

            List<CodeElement> codeElementList = parser.GetCodeElementList(codeProjectItem);

            if (codeElementList.Count > 0)
            {
                yUMLUrl = new YUMLURLBuilder(codeElementList).Build();

                if (!string.IsNullOrEmpty(yUMLUrl))
                {
                    Navigate(yUMLUrl);
                }
            }

            //string assemblyPath = GetSelectedProjectAssemblyPath();
            //Assembly assembly = Assembly.LoadFile(assemblyPath);
            //List<Type> typeList = new List<Type>();

            //foreach (CodeClass codeClass in codeClassList)
            //{
            //    typeList.Add(assembly.GetType(codeClass.FullName));
            //}

            //yUMLUrl = new YumlGenerator(typeList).Yuml();
            //Debug.Write(yUMLUrl);
            //OpenYuml(yUMLUrl);

            /*
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Aljeida.VisualStudio.YUMLPackage",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
             */
        }

        private void Navigate(string yUMLExpression)
        {
            string url = "http://yuml.me/diagram/scruffy/class/" + yUMLExpression;
            string tempFileName = Path.GetTempFileName() + ".htm";
            using (var sw = File.CreateText(tempFileName))
            {
                sw.Write(string.Format("<html><body><img src=\"{0}\" /><br /><br />yUML Expression: <b>{1}</b></body></html>", url, HttpUtility.HtmlEncode(yUMLExpression)));
            }

            IVsWebBrowsingService browser = GetService(typeof(SVsWebBrowsingService)) as IVsWebBrowsingService;
            IVsWindowFrame newWnd;
            browser.Navigate(tempFileName, (uint)__VSWBNAVIGATEFLAGS.VSNWB_ForceNew, out newWnd);
        }
    }
}
