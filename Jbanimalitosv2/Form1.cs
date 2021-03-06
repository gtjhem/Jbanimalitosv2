﻿


using Jbanimalitosv2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Jbanimalitosv2
{


    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        //
        // Declaraciones del API de Windows (y constantes usadas para mover el form)
        //
        const int WM_SYSCOMMAND = 0x112;
        const int MOUSE_MOVE = 0xF012;
        //
        // Declaraciones del API
        [System.Runtime.InteropServices.DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        //
        [System.Runtime.InteropServices.DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);


        private void Form1_Load(object sender, EventArgs e)
        {
            
 
            sr_loteria();

            if (this.cmbloteria.Items.Count > 0){
                this.cmbloteria.SelectedIndex = 0;
                string[] v = cmbloteria.Text.Split('-');
                sr_sorteos(int.Parse(v[1]));
            }

            if (this.lstSorteos.Items.Count > 0 ){
                this.lstSorteos.SetItemChecked(0, true);
            }
                
        }

         public void sr_sorteos (int vr_key_codigo)
        {
            animalitos db = new animalitos(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=animalitos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True");
           
            var query = (from qrysorteos in db.dbSorteos
                         join qryhorario in db.dbhorarios on qrysorteos.ID_SORTEO equals qryhorario.IDSORTEOHR
                         where qrysorteos.ESTATUS == "A" && qrysorteos.ID_SORTEO == vr_key_codigo 
                         select new { qryhorario.HORA, qrysorteos.NOMBRE_SORTEO, qrysorteos.ID_SORTEO }).ToList();


            this.lstSorteos.Items.Clear();
            foreach (var aqui in query)

                if (aqui.HORA >= DateTime.Now.TimeOfDay )
                {
                                       
                    this.lstSorteos.Items.Add(aqui.HORA  + " - " + aqui.NOMBRE_SORTEO.ToString().ToUpper() + " - " + aqui.ID_SORTEO);
                }

                if (lstSorteos.Items.Count > 0 ) this.lstSorteos.SetItemChecked(0, true);


        }
        public void sr_loteria()
        {
            animalitos db = new animalitos(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=animalitos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True");

            var query = (from qrysorteos in db.dbSorteos
                         where qrysorteos.ESTATUS == "A"
                         select new {qrysorteos.NOMBRE_SORTEO, qrysorteos.ID_SORTEO }).ToList();


            this.cmbloteria.Items.Clear();

            foreach (var aqui in query)
                
                this.cmbloteria.Items.Add(aqui.NOMBRE_SORTEO.ToString().ToUpper() + " - " + aqui.ID_SORTEO);
        }

        public void sr_animalitos(int vr_key_loteria)
        {
            animalitos db = new animalitos(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=animalitos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True");

            var query = (from qryanimalito in db.dbanimalitos 
                         where qryanimalito.IDSORTEOAN == vr_key_loteria 
                         
                         select new { qryanimalito.CODIGO , qryanimalito.NOMBRE_ANIMALITO  }).ToList();


            this.Animales.Items.Clear();

            foreach (var aqui in query)
                this.Animales.Items.Add(aqui.CODIGO.ToString() + " - " + aqui.NOMBRE_ANIMALITO.ToString());
                    
        }


        private void Minz_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Cerrar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close ();
        }

        private void Relog_Tick(object sender, EventArgs e)
        {
            this.Hora.Text = DateTime.Now.ToString("G");
        }

        private void moverForm()
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, MOUSE_MOVE, 0);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            moverForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sr_agregar();
        }

        public void sr_agregar() {

            string msg = "";

            msg = validar();

            if (msg == "")
            {
                if (Ticket.Items.Count <= 0)
                {

                    foreach (object itemChecked in lstSorteos.CheckedItems) // obtengo el valor seleccionado 
                    {
                        this.Ticket.Items.Add(itemChecked.ToString());
                    }

                }

                if (Animales.CheckedItems.Count > 0)
                {
                    this.Animal.Text = "";
                    this.Nombre.Text = "";
                    
                    foreach (object seleccionados in Animales.CheckedItems)
                    {
                        this.Ticket.Items.Add(seleccionados.ToString() + " - " + this.Monto.Text);
                    }
                }
                else
                {
                    this.Ticket.Items.Add(this.Animal.Text + " - " + this.Nombre.Text + " - " + this.Monto.Text);

                }


                this.Monto.Text = "";
                Calcular();
                deseleccionar();
            }
            else
            {
                MessageBox.Show(msg, "Faltan Campos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        

        public void deseleccionar ()
        {
            for (int i = 0; i < Animales.Items.Count; i++)
                Animales.SetItemCheckState(i, (false  ? CheckState.Checked : CheckState.Unchecked));
        }

        public string validar()
        {
            string msg = "";
            int cont = 0;

            if (Animales.CheckedItems.Count > 0 )
            {

                if (this.Monto.Text == "") {
                    cont += 1;
                    msg = msg + " Debe Ingresar el monto " + Environment.NewLine;
                }
            }else
            {
                if (this.Animal.Text == "")
                {
                    cont += 1;
                    msg = msg + " Indique el codigo de la fruta o animal" + Environment.NewLine;
                }
                if (this.Monto.Text == "")
                {
                    cont += 1;
                    msg = msg + " Indique el monto de la jugada" + Environment.NewLine;
                }
            }

            return msg;

            
        }

        public void Calcular()
        {
            int vrtotal;
            string[] Matriz = new string[Ticket.Items.Count];
            vrtotal = 0;
            for (int i = 1; i < Ticket.Items.Count; i++) { 
                Matriz[i] = Ticket.Items[i].ToString();
                string[] picados = Ticket.Items[i].ToString().Split('-');
                vrtotal = vrtotal + int.Parse(picados[2]);
            }
            this.Total.Text = vrtotal.ToString ();
        }

        private void Monto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {

                e.Handled = false;

            }
            else if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == '\r')

                {
                    e.Handled = true;

                }else{
                    e.Handled = false;
                }
                
            }
            //SI lo activas permite usar el espacio
            //else if (Char.IsSeparator(e.KeyChar))
            //{
             //   e.Handled = false;
            //}
            else
            {
                e.Handled = true;
            }

            if (e.KeyChar == '\r')
            {
               
                sr_agregar();

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.Ticket.SelectedIndex > -1  && this.Ticket.SelectedIndex != 0)
            {
                this.Ticket.Items.RemoveAt(this.Ticket.SelectedIndex);
            }
            Calcular();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Ticket.Items.Clear();
            Calcular();
        }
        

        private void Animales_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Animal.Text = "";
            this.Nombre.Text = "";
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void lstSorteos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (lstSorteos.CheckedItems.Count == 1)
            {
                Boolean isCheckedItemBeingUnchecked = (e.CurrentValue == CheckState.Checked);
                if (isCheckedItemBeingUnchecked)
                {
                    e.NewValue = CheckState.Checked;
                }
                else
                {
                    Int32 checkedItemIndex = lstSorteos.CheckedIndices[0];
                    lstSorteos.ItemCheck -= lstSorteos_ItemCheck;
                    lstSorteos.SetItemChecked(checkedItemIndex, false);
                    lstSorteos.ItemCheck += lstSorteos_ItemCheck;
                }

                // Indica que sorteo es en el ticket
                if (Ticket.Items.Count > 0)
                {
                    if (Ticket.Items[0].ToString() != "")
                    {
                        Ticket.Items.RemoveAt(0);
                        Ticket.Items.Insert(0, lstSorteos.SelectedItem);

                    }
                }
                return;
            }

        }

        private void cmbloteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            //private string[] v;

            string [] v = cmbloteria.Text.Split('-');

            sr_animalitos( int.Parse(v[1]));

            sr_sorteos(int.Parse(v[1]));
        }

        private void Animal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {

                e.Handled = false;

            }
            else if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == '\r')

                {

                    e.Handled = true;

                }else
                {
                    e.Handled = false;
                }
            }
            //SI lo activas permite usar el espacio
            //else if (Char.IsSeparator(e.KeyChar))
            //{
            //   e.Handled = false;
            //}
            else
            {
                e.Handled = true;
            }


           



        }

        public void sr_nombre_Animalito(string vr_key_codigo, int vr_key_sorteo)
        {
            animalitos db = new animalitos(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=animalitos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True");

            var query = (from qryanimalito in db.dbanimalitos
                         where qryanimalito.IDSORTEOAN == vr_key_sorteo && qryanimalito.CODIGO == vr_key_codigo 
                         select new { qryanimalito.CODIGO, qryanimalito.NOMBRE_ANIMALITO }).ToList();


            this.Nombre.Text = "";

            foreach (var aqui in query)
                this.Nombre.Text = aqui.NOMBRE_ANIMALITO.ToString();
        }

        private void Animal_TextChanged(object sender, EventArgs e)
        {
            string[] v = cmbloteria.Text.Split('-');
            sr_nombre_Animalito(Animal.Text.ToString(), int.Parse(v[1]));
        }
    }
}
