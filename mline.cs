using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace ckx
{
    class mline:EntityJig
    {



        public Point3d ptStart, ptlast, ptcurrent;
        public bool flag = true;
        public int count;
        public double width = 240.0;
        public Point3dCollection pts1 = new Point3dCollection();
        public Point3dCollection pts2 = new Point3dCollection();

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            JigPromptPointOptions ppo = new JigPromptPointOptions("\n 请指定下一点");
            ppo.Keywords.Add(" ", " ", " ");
            ppo.Keywords.Default = " ";
            ppo.UseBasePoint = true;
            ppo.BasePoint = ptlast;
            PromptPointResult ppr = prompts.AcquirePoint(ppo);
            ptcurrent = ppr.Value;
            if (ptcurrent == ptlast) return SamplerStatus.NoChange;
            if (ppr.Status == PromptStatus.Keyword)
            {
                if (ppr.StringResult == " ")
                {
                    count++;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            if (ppr.Status == PromptStatus.OK)
            {
                return SamplerStatus.OK;
            }
            return SamplerStatus.NoChange;
        }

        protected override bool Update()
        {

            if (ptcurrent != null)
            {
                Mline mline = Entity as Mline;
                //pline.SetPointAt(pline.NumberOfVertices -1, new Point2d(ptlast.X, ptlast.Y));
                if (count == 1)
                {
                    //mline.RemoveLastSegment(ptlast);
                    mline.AppendSegment(ptcurrent);
                    ptlast = ptcurrent;
                    return true;
                }
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("c=" + count + ",n=" + mline.NumberOfVertices);

                if (count == mline.NumberOfVertices )
                {

                    mline.RemoveLastSegment(ptlast);
                    mline.AppendSegment(ptcurrent);
                    ptlast = ptcurrent;
                }
                else
                {
                    //mline.RemoveLastSegment(ptlast);
                    mline.AppendSegment(ptcurrent);
                }
            }
            return true;
        }


        public mline(Point3d point, Vector3d normal) : base(new Mline())
        {

            Mline mline = Entity as Mline;
            mline.Normal = normal;
            // MlineStyle mlineStyle = new MlineStyle();
            //mlineStyle.
            mline.ColorIndex = 1;
            mline.Style = Application.DocumentManager.MdiActiveDocument.Database.CmlstyleID;
            mline.AppendSegment(point);
            count = 1;

        }

        public Entity GetEntity()
        {
            return Entity;
        }


        public void GetPoint3d01(Point3d p1, Point3d p2)
        {
            Line l = new Line(p1, p2);
            DBObjectCollection dBObject = l.GetOffsetCurves(-120);
            Line l2 = dBObject[0] as Line;
            pts1.Add(l2.StartPoint);
            pts1.Add(l2.EndPoint);
        }
        public void GetPoint3d02(Point3d p1, Point3d p2)
        {
            Line l = new Line(p1, p2);
            DBObjectCollection dBObject = l.GetOffsetCurves(120);
            Line l2 = dBObject[0] as Line;
            pts2.Add(l2.StartPoint);
            pts2.Add(l2.EndPoint);
        }



    }
}


