using Autodesk.AutoCAD.Geometry;

namespace ckx
{
    public class myStair
    {
        //左平台起点
        public Point3d point { get; set; }
        //楼层号
        public string Num { get; set; }
        //层宽
        public double FloorWidth { get; set; }
        //层高
        public double FloorHeight { get; set; }
        //楼梯宽
        public double LtW { get; set; }
        //楼梯高
        public double LtH { get; set; }
        //踏步数
        public double LTN { get; set; }
        //左平台宽
        public double ExtW { get; set; }
        //右平台宽
        public double ExtW2 { get; set; }
        //楼梯厚度
        public double ExtH { get; set; }
        //平台厚度
        public double ExtH2 { get; set; }
        public double Cover { get; set; }
        //梁高
        public double Sh { get; set; }
        //梁宽
        public double SW { get; set; }
        //楼梯类型
        public string type { get; set; }
    }
}
