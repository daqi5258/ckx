using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ckx
{
    class LT : EntityJig
    {

        public Point2d ptStart, ptlast, ptcurrent;
        public double LTW = 260.0, ltH = 175.5;
        public Point2dCollection pts = new Point2dCollection();
        public DBObjectCollection dbo = new DBObjectCollection();

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
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.Cancel;
        }

        protected override bool Update()
        {
            Polyline pl = (Polyline)Entity;
            for (int i = 0; i < pts.Count; i++)
            {
                if (pl.NumberOfVertices > i)
                    pl.SetPointAt(i, pts[i]);
                else
                    pl.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            while (pts.Count < pl.NumberOfVertices)
            {
                pl.RemoveVertexAt(pts.Count - 1);
            }
            return true;
        }

        public LT(Point2d p0, double ltw, double lth) : base(new Polyline())
        {
          
            ptStart = ptlast = p0;
            LTW = ltw;
            ltH = lth;
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

        public Entity GetEntity()
        {
            return Entity;
        }
    }
}
