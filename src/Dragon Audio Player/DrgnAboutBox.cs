using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Dragon_Audio_Player
{

    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       06/12/2014                  |
    //      |   Version:    0.3                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------

    partial class DrgnAboutBox : Form
    {
        public DrgnAboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AppInfo.AssemblyTitle);
            this.labelProductName.Text = AppInfo.AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AppInfo.AssemblyVersion);
            this.labelCopyright.Text = AppInfo.AssemblyCopyright;
            this.labelCompanyName.Text = AppInfo.AssemblyCompany;
            this.labelDescription.Text = AppInfo.AssemblyDescription;
            
        }

    }

    public static class AppInfo
    {

        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                object[] lvAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (lvAttributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)lvAttributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] lvAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (lvAttributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)lvAttributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] lvAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (lvAttributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)lvAttributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] lvAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (lvAttributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)lvAttributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] lvAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (lvAttributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)lvAttributes[0]).Company;
            }
        }
        #endregion
    }
}
