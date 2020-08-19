using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ckx
{
    class Stair
    {
        public double FloorWidth, FloorHeight, LtW, LtH, ExtW, ExtH;
        public Point3d ptStart;
        public Point2dCollection pts = new Point2dCollection(),
             pts2 = new Point2dCollection();
        public void DrawStair(Point3d point, double w, double h, double ltw, double lth, double yc, double hd)
        {
            ptStart = point;
            FloorWidth = w;
            FloorHeight = h;
            LtW = ltw;
            LtH = lth;
            ExtW = yc;
            ExtH = hd;
            double num = Math.Ceiling(FloorWidth / LtW);
            LtH = FloorWidth / num;
            Point2d p0 = new Point2d(point.X - ExtW, point.Y);
            Point2d p1 = new Point2d(point.X, point.Y);
            pts.Add(p0); pts.Add(p1);
            Point2d ptl = Point2d.Origin;
            for (int i = 1; i < num; i++)
            {
                pts.Add(new Point2d(p0.X + (i - 1) * LtW, p0.Y + i * LtH));
                ptl = new Point2d(p0.X + i * LtW, p0.Y + i * LtH);
                pts.Add(ptl);
            }
            Point2d p3 = new Point2d(ptl.X + ExtW, ptl.Y);
            Point2d p4 = new Point2d(p3.X, p3.Y - ExtH);
            Point2d p5 = new Point2d(p4.X - ExtW, p4.Y);
            Point2d p6 = new Point2d(p1.X, p1.Y - ExtH);
            Point2d p7 = new Point2d(p0.X, p0.Y - ExtH);
            pts.Add(p3); pts.Add(p4); pts.Add(p5); pts.Add(p6); pts.Add(p7);

            pts2.Add(p1);
            for (int i = 1; i < num; i++)
            {
                pts2.Add(new Point2d(p0.X + i  * LtW, p0.Y - (i-1) * LtH));
                ptl = new Point2d(p0.X + i * LtW, p0.Y - i * LtH);
                pts2.Add(ptl);
            }
            Point2d p2_1 = new Point2d(ptl.X+ExtW,ptl.Y);
            pts2.Add(p2_1);
            Polyline pl = new Polyline();
            for (int i = 0; i < pts.Count; i++)
            {
                pl.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            Polyline pl2 = new Polyline();
            for (int i = 0; i < pts2.Count; i++)
            {
                pl.AddVertexAt(i, pts2[i], 0, 0, 0);
            }
            
            // Point2d p2_2 = new Point2d(p2_1.X, p2_1.Y-ExtH);
        }





    }
}
