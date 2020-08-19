using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ckx
{
    public partial class StairForm : Form
    {
        public List<myStair> res = new List<myStair>();
        public StairForm()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (res.Count > 0)
                res.Clear();
            int MaxFl = 0, MinFL = 999 ;
            try
            {
                for(int n=0;n<InformData.Rows.Count-1;n++ )
                {
                    DataGridViewRow row = InformData.Rows[n];
                    if (row.Cells[0].Value.ToString().IndexOf("-") < 0 && Regex.IsMatch(row.Cells[0].Value.ToString(), @"^[1-9]\d*$"))
                    {
                        myStair stair = new myStair();
                        stair.Num = int.Parse(row.Cells[0].Value.ToString());
                        if (stair.Num > MaxFl)
                            MaxFl = stair.Num;
                        if (stair.Num < MinFL)
                            MinFL = stair.Num;
                        stair.FloorWidth = double.Parse(row.Cells[1].Value.ToString());
                        stair.FloorHeight = double.Parse(row.Cells[2].Value.ToString());
                        stair.LtW = double.Parse(row.Cells[3].Value.ToString());
                        stair.LtH = double.Parse(row.Cells[4].Value.ToString());
                        stair.LTN = double.Parse(row.Cells[5].Value.ToString());
                        stair.ExtW = double.Parse(row.Cells[6].Value.ToString());
                        stair.ExtW2 = double.Parse(row.Cells[7].Value.ToString());
                        stair.ExtH = double.Parse(row.Cells[8].Value.ToString());
                        stair.Cover = double.Parse(row.Cells[9].Value.ToString());
                        stair.Sh = double.Parse(row.Cells[10].Value.ToString());
                        stair.SW = double.Parse(row.Cells[11].Value.ToString());
                        res.Add(stair);
                    }
                    else if (row.Cells[0].Value.ToString().IndexOf("-") > -1)
                    {
                        int start = int.Parse(row.Cells[0].Value.ToString().Substring(0, row.Cells[0].Value.ToString().IndexOf("-")));
                        int end = int.Parse(row.Cells[0].Value.ToString().Substring(row.Cells[0].Value.ToString().IndexOf("-") + 1));
                        for (int j = start; j <= end; j++)
                        {
                            myStair stair = new myStair();
                            stair.Num = j;
                            if (stair.Num > MaxFl)
                                MaxFl = stair.Num;
                            if (stair.Num < MinFL)
                                MinFL = stair.Num;
                            stair.FloorWidth = double.Parse(row.Cells[1].Value.ToString());
                            stair.FloorHeight = double.Parse(row.Cells[2].Value.ToString());
                            stair.LtW = double.Parse(row.Cells[3].Value.ToString());
                            stair.LtH = double.Parse(row.Cells[4].Value.ToString());
                            stair.LTN = double.Parse(row.Cells[5].Value.ToString());
                            stair.ExtW = double.Parse(row.Cells[6].Value.ToString());
                            stair.ExtW2 = double.Parse(row.Cells[7].Value.ToString());
                            stair.ExtH = double.Parse(row.Cells[8].Value.ToString());
                            stair.Cover = double.Parse(row.Cells[9].Value.ToString());
                            stair.Sh = double.Parse(row.Cells[10].Value.ToString());
                            stair.SW = double.Parse(row.Cells[11].Value.ToString());
                            res.Add(stair);
                        }
                    }

                }
            }
            catch(Exception Er)
            {
                MessageBox.Show("\nError!不允许有空值和其他字符(楼层-除外)");
            }
            if ((MaxFl-MinFL)==res.Count-1 && res.Count>0)
            {
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
            else{
                MessageBox.Show("楼层数目不对,实际楼层有:" + res.Count + ",但是最大楼层为:" + MaxFl +",请检查!");
            }
           
        }

        public static double ObjectToDouble(Object obj)
        {
            if (obj == null)
                return 0.0;
            else
            {
                string str = obj.ToString();
                str = Regex.Replace(str, @"[^0-9]+.?\d?", "");
                return int.Parse(str); ;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            InformData.Rows.Clear();
        }

        private void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int index = e.Row.Index;
            if (index > 0)
            {
                e.Row.Cells[0].Value = InformData.Rows[index - 1].Cells[0].Value;
                e.Row.Cells[1].Value = InformData.Rows[index - 1].Cells[1].Value;
                e.Row.Cells[2].Value = InformData.Rows[index - 1].Cells[2].Value;
                e.Row.Cells[3].Value = InformData.Rows[index - 1].Cells[3].Value;
                e.Row.Cells[4].Value = InformData.Rows[index - 1].Cells[4].Value;
                e.Row.Cells[5].Value = InformData.Rows[index - 1].Cells[5].Value;
                e.Row.Cells[6].Value = InformData.Rows[index - 1].Cells[6].Value;
                e.Row.Cells[7].Value = InformData.Rows[index - 1].Cells[7].Value;
                e.Row.Cells[8].Value = InformData.Rows[index - 1].Cells[8].Value;
                e.Row.Cells[9].Value = InformData.Rows[index - 1].Cells[9].Value;
                e.Row.Cells[10].Value = InformData.Rows[index - 1].Cells[10].Value;
                e.Row.Cells[11].Value = InformData.Rows[index - 1].Cells[11].Value;
            }

        }
        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (InformData.IsCurrentCellDirty)
            {
                if (InformData.CurrentCell.ColumnIndex == 0)
                    InformData.CommitEdit(DataGridViewDataErrorContexts.Commit);

            }
        }
    }
}
