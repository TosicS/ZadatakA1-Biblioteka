using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZadatakA1_Biblioteka
{
    public partial class Form1 : Form
    {
        SqlConnection konekcija = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Skola\MATURA\Programiranje\ZadatakA1-Biblioteka\ZadatakA1-Biblioteka\A1.mdf;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        public void PrikaziLV() //prva forma
        {
            SqlCommand cmd = new SqlCommand("Select * from Citalac", konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                listView1.Items.Clear();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    ListViewItem item = new ListViewItem(row[0].ToString());
                    item.SubItems.Add(row[1].ToString());
                    item.SubItems.Add(row[2].ToString());
                    item.SubItems.Add(row[3].ToString());
                    item.SubItems.Add(row[4].ToString());

                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PopuniCB() //druga forma
        {
            string sqlUpit = "Select CitalacID as sifra, CONCAT(CitalacID, + '-' + Ime + ' ' + Prezime) as PunoIme from Citalac";

            SqlCommand cmd = new SqlCommand(sqlUpit, konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

                comboBoxCitalac.DataSource = dt;
                comboBoxCitalac.DisplayMember = "PunoIme";
                comboBoxCitalac.ValueMember = "sifra";
                comboBoxCitalac.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clearData()
        {
            textBoxIme.Text = "";
            textBoxJBMG.Text = "";
            textBoxPrezime.Text = "";
            textBoxAdresa.Text = "";
            buttonUpisi.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrikaziLV();
            PopuniCB();
            buttonUpisi.Enabled = false;
        }

        private void textBoxBrKarte_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBrKarte.Text == "") //texbox sa br clasne karte/sifra
            {
                clearData();
                return;
            }

            try
            {
                int.Parse(textBoxBrKarte.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Unesite ispravnu sifru!");
                textBoxBrKarte.Text = "";
                clearData();
                return;
            }

            string sqlUpit = "Select * from Citalac where CitalacID = @parSifra";

            SqlCommand cmd = new SqlCommand(sqlUpit, konekcija);

            cmd.Parameters.AddWithValue("@parSifra", int.Parse(textBoxBrKarte.Text));

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    textBoxJBMG.Text = dt.Rows[0][1].ToString();
                    textBoxIme.Text = dt.Rows[0][2].ToString();
                    textBoxPrezime.Text = dt.Rows[0][3].ToString();
                    textBoxAdresa.Text = dt.Rows[0][4].ToString();
                    buttonUpisi.Enabled = false;

                }
                else
                {
                    clearData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpisi_Click(object sender, EventArgs e)
        {
            buttonUpisi.Enabled = false;
            try
            {
                int.Parse(textBoxBrKarte.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Unesite ispravnu sifru!");
                textBoxBrKarte.Text = "";
                clearData();
                return;
            }

            if (textBoxBrKarte.Text != "" && textBoxJBMG.Text != "" && textBoxIme.Text != "" && textBoxPrezime.Text != "" && textBoxAdresa.Text != "")
            {
                string sqlUpit = "Insert into Citalac values(@parSifra,@parJMBG,@parIme,@parPrezime,@parAdresa)";

                SqlCommand cmd = new SqlCommand(sqlUpit, konekcija);

                cmd.Parameters.AddWithValue("@parSifra", int.Parse(textBoxBrKarte.Text));
                cmd.Parameters.AddWithValue("@parJMBG", textBoxJBMG.Text);
                cmd.Parameters.AddWithValue("@parIme", textBoxIme.Text);
                cmd.Parameters.AddWithValue("@parPrezime", textBoxPrezime.Text);
                cmd.Parameters.AddWithValue("@parAdresa", textBoxAdresa.Text);

                try
                {
                    konekcija.Open();

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Uspesan upis.");
                    PrikaziLV();
                    clearData();

                    foreach (ListViewItem row in listView1.Items)
                    {
                        if (row.SubItems[0].Text == textBoxBrKarte.Text)
                        {
                            row.Selected = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    konekcija.Close();
                }
            }
            else
            {
                clearData();
                MessageBox.Show("Niste popunili sva polja!");
            }
        }

        private void buttonIzadji_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //DRUGI TAB

        private void buttonPrikazi_Click(object sender, EventArgs e)
        {
            if (comboBoxCitalac.Text != "")
            {
                string sqlUpit = "Select CONCAT(Citalac.Ime, '-', Citalac.Prezime) as 'Citalac', " +
                    "DATEPART(year, Na_Citanju.DatumUzimanja) AS 'Godina', " +
                    "COUNT(Na_Citanju.DatumUzimanja) as 'Broj iznajmljivanja', " +
                    "(COUNT(Na_Citanju.DatumUzimanja) - COUNT(Na_Citanju.DatumVracanja)) as 'Nije vraceno' " +
                    "from Citalac " +
                    "inner join Na_Citanju on Na_Citanju.CitalacID = Citalac.CitalacID " +
                    "where DATEPART(year, Na_Citanju.DatumUzimanja) between @parOd and @parDo " +
                    "and Citalac.CitalacID = @parID " +
                    "group by Citalac.Ime, Citalac.Prezime, DATEPART(year, Na_Citanju.DatumUzimanja)";

                SqlCommand cmd = new SqlCommand(sqlUpit, konekcija);

                cmd.Parameters.AddWithValue("@parID", comboBoxCitalac.SelectedValue);
                cmd.Parameters.AddWithValue("@parOd", numericUpDownOd.Value);
                cmd.Parameters.AddWithValue("@parDo", numericUpDownDo.Value);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    chart1.DataSource = dt;
                    chart1.Series[0].XValueMember = "Godina";
                    chart1.Series[0].YValueMembers = "Broj iznajmljivanja";
                    chart1.Series[1].XValueMember = "Godina";
                    chart1.Series[1].YValueMembers = "Nije vraceno";
                    chart1.Series[0].IsValueShownAsLabel = false;
                    chart1.Series[1].IsValueShownAsLabel = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void buttonIzadjiF2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
