// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="DoubleDutch Dragons">
//   © 2015 DoubleDutch Dragons
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;

namespace Dragon_Audio_Player
{
    internal static class Program
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}