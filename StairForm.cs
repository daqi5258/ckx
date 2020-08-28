using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ckx {
    public partial class StairForm : Form {
        public List<myStair> res = new List<myStair>();
        public StairForm(List<myStair> stairs)
        {
            InitializeComponent();
            init(stairs);
        }

        public void init(List<myStair> stairs)
        {
            if (stairs != null && stairs.Count > 0)
            {
                for (int n = 0; n < stairs.Count; n++)
                {
                    int index = InformData.Rows.Add();
                    DataGridViewRow row = InformData.Rows[index];
                    row.Cells[0].Value = stairs[n].Num;
                    row.Cells[1].Value = stairs[n].FloorWidth;
                    row.Cells[2].Value = stairs[n].FloorHeight;
                    row.Cells[3].Value = stairs[n].LtW;
                    row.Cells[4].Value = stairs[n].LtH;
                    row.Cells[5].Value = stairs[n].LTN;
                    row.Cells[6].Value = stairs[n].Cover;
                    row.Cells[7].Value = stairs[n].ExtH;
                    row.Cells[8].Value = stairs[n].ExtW;
                    row.Cells[9].Value = stairs[n].ExtW2;
                    row.Cells[10].Value = stairs[n].ExtH2;
                    row.Cells[11].Value = stairs[n].Sh;
                    row.Cells[12].Value = stairs[n].SW;
                    row.Cells[13].Value = stairs[n].type;
                }
            }
        }
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (res.Count > 0)
                res.Clear();
            //int MaxFl = 0, MinFL = 999;
            try
            {
                for (int n = 0; n < InformData.Rows.Count - 1; n++)
                {
                    DataGridViewRow row = InformData.Rows[n];

                    //if (row.Cells[0].Value.ToString().IndexOf("-") < 0 && Regex.IsMatch(row.Cells[0].Value.ToString(), @"^[1-9]\d*$"))
                    //{
                    myStair stair = new myStair();
                    stair.Num = row.Cells[0].Value.ToString();
                    /*
                    if (stair.Num > MaxFl)
                        MaxFl = stair.Num;
                    if (stair.Num < MinFL)
                        MinFL = stair.Num;*/
                    stair.FloorWidth = double.Parse(row.Cells[1].Value.ToString());
                    stair.FloorHeight = double.Parse(row.Cells[2].Value.ToString());
                    stair.LtW = double.Parse(row.Cells[3].Value.ToString());
                    stair.LtH = double.Parse(row.Cells[4].Value.ToString());
                    stair.LTN = double.Parse(row.Cells[5].Value.ToString());
                    stair.Cover = double.Parse(row.Cells[6].Value.ToString());
                    stair.ExtH = double.Parse(row.Cells[7].Value.ToString());
                    stair.ExtW = double.Parse(row.Cells[8].Value.ToString());
                    stair.ExtW2 = double.Parse(row.Cells[9].Value.ToString());
                    stair.ExtH2 = double.Parse(row.Cells[10].Value.ToString());
                    stair.Sh = double.Parse(row.Cells[11].Value.ToString());
                    stair.SW = double.Parse(row.Cells[12].Value.ToString());
                    stair.type = row.Cells[13].Value.ToString();
                    res.Add(stair);
                    //}
                    /*else if (row.Cells[0].Value.ToString().IndexOf("-") > -1){
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
                            stair.Cover = double.Parse(row.Cells[6].Value.ToString());
                            stair.ExtH = double.Parse(row.Cells[7].Value.ToString());
                            stair.ExtW = double.Parse(row.Cells[8].Value.ToString());
                            stair.ExtW2 = double.Parse(row.Cells[9].Value.ToString());
                            stair.ExtH2 = double.Parse(row.Cells[10].Value.ToString());
                            stair.Sh = double.Parse(row.Cells[11].Value.ToString());
                            stair.SW = double.Parse(row.Cells[12].Value.ToString());
                            stair.type =  row.Cells[13].Value.ToString();
                            res.Add(stair);
                        }
                    }*/

                }
            }
            catch
            {
                MessageBox.Show("\nError!不允许有空值和其他字符(楼层-除外)");
            }
            /*
            if ((MaxFl - MinFL) == res.Count - 1 && res.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
            else
            {
                MessageBox.Show("楼层数目不对,实际楼层有:" + res.Count + ",但是最大楼层为:" + MaxFl + ",请检查!");
            }*/
            this.DialogResult = DialogResult.OK;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            InformData.Rows.Clear();
        }

        //新增默认复制上面一行信息
        private void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int index = e.Row.Index;
            if (index == 0)
            {

                e.Row.Cells[3].Value = "260";
                e.Row.Cells[4].Value = "175";
                e.Row.Cells[6].Value = "20";
                e.Row.Cells[7].Value = "120";
                e.Row.Cells[8].Value = "1200";
                e.Row.Cells[9].Value = "1200";
                e.Row.Cells[10].Value = "100";
                e.Row.Cells[11].Value = "400";
                e.Row.Cells[12].Value = "200";
                e.Row.Cells[13].Value = "双楼梯";
            }
            if (index > 0)
            {
                //e.Row.Cells[0].Value = InformData.Rows[index - 1].Cells[0].Value;
                //e.Row.Cells[1].Value = InformData.Rows[index - 1].Cells[1].Value;
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
                e.Row.Cells[12].Value = InformData.Rows[index - 1].Cells[12].Value;
                e.Row.Cells[13].Value = InformData.Rows[index - 1].Cells[13].Value;
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


        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var c = InformData.CurrentCell;

            if (c != null)
            {
                int rowId = c.DataGridView.CurrentCell.RowIndex;
                int colId = c.DataGridView.CurrentCell.ColumnIndex;
                double LtW = 0, FloorWidth = 0, FloorHeight = 0, LtH = 0, LTN = 0, ExtW = 0
                    , ExtW2 = 0, ExtH = 0, ExtH2 = 0, Cover = 0, Sh = 0, SW = 0;
                if (InformData.Rows[rowId].Cells[1].Value != null)
                    FloorWidth = double.Parse(InformData.Rows[rowId].Cells[1].Value.ToString());
                if (InformData.Rows[rowId].Cells[2].Value != null)
                    FloorHeight = double.Parse(InformData.Rows[rowId].Cells[2].Value.ToString());
                if (InformData.Rows[rowId].Cells[3].Value != null)
                    LtW = double.Parse(InformData.Rows[rowId].Cells[3].Value.ToString());
                if (InformData.Rows[rowId].Cells[4].Value != null)
                    LtH = double.Parse(InformData.Rows[rowId].Cells[4].Value.ToString());
                if (InformData.Rows[rowId].Cells[5].Value != null)
                    LTN = double.Parse(InformData.Rows[rowId].Cells[5].Value.ToString());
                if (InformData.Rows[rowId].Cells[6].Value != null)
                    ExtH = double.Parse(InformData.Rows[rowId].Cells[6].Value.ToString());
                if (InformData.Rows[rowId].Cells[7].Value != null)
                    Cover = double.Parse(InformData.Rows[rowId].Cells[7].Value.ToString());
                if (InformData.Rows[rowId].Cells[8].Value != null)
                    ExtW = double.Parse(InformData.Rows[rowId].Cells[8].Value.ToString());
                if (InformData.Rows[rowId].Cells[9].Value != null)
                    ExtW2 = double.Parse(InformData.Rows[rowId].Cells[9].Value.ToString());
                if (InformData.Rows[rowId].Cells[10].Value != null)
                    ExtH2 = double.Parse(InformData.Rows[rowId].Cells[10].Value.ToString());

                if (InformData.Rows[rowId].Cells[11].Value != null)
                    Sh = double.Parse(InformData.Rows[rowId].Cells[11].Value.ToString());
                if (InformData.Rows[rowId].Cells[12].Value != null)
                    SW = double.Parse(InformData.Rows[rowId].Cells[12].Value.ToString());
                string type = InformData.Rows[rowId].Cells[13].Value.ToString();
                if (FloorHeight > 0 && LtH > 0 && (colId == 4 || colId == 2))
                {
                    if (type == "双楼梯")
                    {
                        LTN = Math.Ceiling(FloorHeight / LtH / 2);
                        LtH = FloorHeight / LTN / 2;
                        InformData.Rows[rowId].Cells[5].Value = LTN;
                        InformData.Rows[rowId].Cells[4].Value = LtH;
                    }
                    else
                    {
                        LTN = Math.Ceiling(FloorHeight / LtH);
                        LtH = FloorHeight / LTN;
                        InformData.Rows[rowId].Cells[5].Value = LTN;
                        InformData.Rows[rowId].Cells[4].Value = LtH;
                    }
                }
                if (FloorHeight > 0 && LTN > 0 && (colId == 5 || colId == 2))
                {
                    if (type == "双楼梯")
                    {
                        LtH = FloorHeight / LTN / 2;
                        InformData.Rows[rowId].Cells[4].Value = LtH;
                    }
                    else
                    {
                        LtH = FloorHeight / LTN;
                        InformData.Rows[rowId].Cells[4].Value = LtH;
                    }
                }
                if (LtW > 0 && LTN > 0)
                {
                    double w = LtW * LTN;
                    if (w > 6000)
                        InformData.Rows[rowId].Cells[7].Value = 230;
                    else if (w > 5700)
                        InformData.Rows[rowId].Cells[7].Value = 220;
                    else if (w > 5400)
                        InformData.Rows[rowId].Cells[7].Value = 200;
                    else if (w > 5100)
                        InformData.Rows[rowId].Cells[7].Value = 180;
                    else if (w > 4800)
                        InformData.Rows[rowId].Cells[7].Value = 170;
                    else if (w > 4500)
                        InformData.Rows[rowId].Cells[7].Value = 160;
                    else if (w > 4200)
                        InformData.Rows[rowId].Cells[7].Value = 150;
                    else if (w > 3900)
                        InformData.Rows[rowId].Cells[7].Value = 140;
                    else if (w > 3600)
                        InformData.Rows[rowId].Cells[7].Value = 140;
                    else if (w > 3300)
                        InformData.Rows[rowId].Cells[7].Value = 130;
                    else if (w > 3000)
                        InformData.Rows[rowId].Cells[7].Value = 120;
                    else if (w > 2700)
                        InformData.Rows[rowId].Cells[7].Value = 110;
                    else if (w > 2400)
                        InformData.Rows[rowId].Cells[7].Value = 100;


                }

                if (ExtW > 0 && ExtW2 > 0 && LTN > 0 && LtW > 0)
                {
                    FloorWidth = ExtW + ExtW2 + (LTN - 1) * LtW;
                    InformData.Rows[rowId].Cells[1].Value = FloorWidth;
                }
                /*
                if (ExtW > 0 && ExtW2 > 0 && LTN > 1 && LtW > 0 && FloorWidth < (ExtW + ExtW2 + (LTN - 1) * LtW))
                {
                    MessageBox.Show("设定层宽:"+ FloorWidth+",小于实际层宽:"+ (ExtW + ExtW2 + (LTN - 1) * LtW));
                }
                if (ExtW > 0 && ExtW2 > 0 && LTN > 1 && LtW > 0 && FloorWidth > (ExtW + ExtW2 + (LTN - 1) * LtW))
                {
                    MessageBox.Show("设定层宽:" + FloorWidth + ",大于实际层宽:" + (ExtW + ExtW2 + (LTN - 1) * LtW));
                }*/
            }
        }


    }
}
