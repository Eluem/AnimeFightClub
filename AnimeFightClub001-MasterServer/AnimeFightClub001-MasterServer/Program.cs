//**********************************************************
// File: Program.cs
//
// Purpose: Used to control the master server.
//
// Written By: Salvatore Hanusiewicz
//**********************************************************
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Lidgren.Network;

namespace AnimeFightClub001_MasterServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }
    }
}
