using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Lidgren.Network;

namespace AnimeFightClub001_MasterServer
{
    public partial class Form1 : Form
    {
        NetworkHandler m_networkHandler;

        public Form1()
        {
            InitializeComponent();


            m_networkHandler = new NetworkHandler(this);
            Thread networkingThread = new Thread(new ThreadStart(m_networkHandler.NetworkListenThread));
            networkingThread.Start();
        }


        private void listBox1_Clicked(object sender, EventArgs e)
        {
        }


        public void AddToServerListBox(string IP)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                listBox1.Items.Add(IP);
            });
        }


        public void ServerListChanged()
        {
            BeginInvoke((MethodInvoker)delegate{listBox1.Items.Clear();});

            foreach (KeyValuePair<string, Server> server in m_networkHandler.ServerDict)
            {
                AddToServerListBox(server.Key);
            }

            /*
            foreach (NetConnection connection in m_networkHandler.ServerNetConnections)
            {
                AddToServerListBox(connection.RemoteEndpoint.Address.ToString());
                //AddToServerListBox(connection.l);
            }
            */
        }
    }
}
