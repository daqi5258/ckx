using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace ckx
{
    class ElRecJig : DrawJig
    {
        public Line myline;
        public Polyline mypl;
        public Point2d ptStart, ptlast, ptcurrent;
        public double LTW = 260.0, ltH = 175.5;
        public Point2dCollection pts = new Point2dCollection();
        public Matrix3d bmt=Matrix3d.Identity;

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions ppo = new JigPromptPointOptions();
            ppo.UseBasePoint = true;
            ppo.BasePoint = new Point3d(ptlast.X, ptlast.Y, 0);
            ppo.UserInputControls = UserInputControls.Accept3dCoordinates;

            PromptPointResult ppr = prompts.AcquirePoint(ppo);
            Point3d pt = ppr.Value;
            ptcurrent = new Point2d(pt.X, pt.Y);
            if (ptcurrent == ptlast)
                return SamplerStatus.NoChange;
            ptlast = ptcurrent;
            GetPts(ptStart, ptlast, true);
            if (ppr.Status == PromptStatus.OK)
            {
                /*
                myline.EndPoint = new Point3d(pts[pts.Count-3].X, pts[pts.Count - 3].Y-240,0) ;
                Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, -240, 0));
                Matrix3d mt = Matrix3d.Displacement(vt);
                myline.TransformBy(bmt);
                myline.TransformBy(mt);
                bmt = mt.Inverse();*/
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.Cancel;
        
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            
            //draw.Geometry.Draw(myline);
            for (int i = 0; i < pts.Count; i++)
            {
                if (mypl.NumberOfVertices > i)
                    mypl.SetPointAt(i, pts[i]);
                else
                    mypl.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            while (pts.Count < mypl.NumberOfVertices)
            {
                mypl.RemoveVertexAt(pts.Count - 1);
            }

            draw.Geometry.Draw(mypl);
            return true;
        }

        public ElRecJig(Point3d pt)
        {
            ptStart = ptlast = new Point2d(pt.X, pt.Y);
            myline = new Line();
            myline.StartPoint = pt;
            mypl = new Polyline();
        }

        protected void GetPts(Point2d p0, Point2d p1, bool deletFlag)
        {
            pts.Clear();
            double Width = p0.X - p1.X, Height = p0.Y - p1.Y;
            double ltw = 0.0, lth = 0.0;
            Width = Width > 0 ? Width : -Width;
            Height = Height > 0 ? Height : -Height;
            double num = Math.Ceiling(Width / LTW);
            ltH = Height / num;
            if (ltH > 175.0)
            {
                num++;
                ltH = Height / num;
            }
            if (p0.X < p1.X)
                ltw = -LTW;
            else
                ltw = LTW;
            if (p0.Y < p1.Y)
                lth = -ltH;
            else
                lth = ltH;
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nltw="+ ltw + ",lth="+lth+",num="+num);
            pts.Add(new Point2d(p0.X + ltw * 2, p0.Y));
            pts.Add(p0);
            //pts.Add(new Point2d(p0.X, p0.Y-lth));
            Point2d ptl = Point2d.Origin;
            for (int i = 1; i < num; i++)
            {
                pts.Add(new Point2d(p0.X - (i - 1) * ltw, p0.Y - i * lth));
                ptl = new Point2d(p0.X - i * ltw, p0.Y - i * lth);
                pts.Add(ptl);
            }
            pts.Add(new Point2d(ptl.X, p1.Y));
            pts.Add(p1);
            pts.Add(new Point2d(p1.X - ltw * 2, p1.Y));
        }


        public static Point2d Point3dToPoint2d(Point3d point)
        {
            return new Point2d(point.X,point.Y);
        }
    }
}
