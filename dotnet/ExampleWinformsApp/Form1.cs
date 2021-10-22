using Discover.APIToolkit;
using Discover.APIToolkit.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExampleWinformsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var caseList = await Queries.Cases();
            dataGridView1.DataSource = caseList.data.cases;
            this.Cursor = Cursors.Default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        private async void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                this.Cursor = Cursors.WaitCursor;
                string caseName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                var caseList = await Queries.CaseWithInfo(caseName);
                var usersCount = caseList.data.cases[0].users?.Count;
                var docsCount = caseList.data.cases[0].statistics.aggregateBaseDocsUsageSummary;
                string ds = $"{{ \"data\": [{{ \"userscount\": {usersCount}, \"documentscount\": {docsCount} }}]}}";
                var ds1 = JsonConvert.DeserializeObject<dynamic>(ds);
                dataGridView2.DataSource = ds1.data;
                this.Cursor = Cursors.Default;
            }
        }
    }
}
