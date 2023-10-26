using SQLTest.ReadFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace linkDatabaseSample
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            test test = new test();
            test.execute();
        }
    }
}
