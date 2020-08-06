using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;


namespace ckx
{
    class PolyJig : EntityJig
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

            //ed.PointFilter += new PointFilterEventHandler(m_GetMousePoint);
            bool et = false;
            JigPromptPointOptions ppo = new JigPromptPointOptions("\n 请指定下一点");
            ppo.Keywords.Add(" "," ","空格下一个");
            ppo.Keywords.Default = " ";
            ppo.UseBasePoint = true;
            ppo.BasePoint = ptlast;
            PromptPointResult ppr = prompts.AcquirePoint(ppo);
            ptlast = ppr.Value;
            if (ppr.Status == PromptStatus.Keyword)
            {

                if (ppr.StringResult==" ")
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
                //count++;
                return SamplerStatus.OK;
            }

          





            return SamplerStatus.NoChange;
            // throw new NotImplementedException();
        }

        protected override bool Update()
        {
            
            if (ptlast != null)
            {
                Polyline pline = Entity as Polyline;
                //pline.SetPointAt(pline.NumberOfVertices -1, new Point2d(ptlast.X, ptlast.Y));
                if(count==pline.NumberOfVertices)
                    pline.AddVertexAt(count, new Point2d(ptlast.X, ptlast.Y), 0, 0, 0);
               else
                   pline.SetPointAt(pline.NumberOfVertices - 1, new Point2d(ptlast.X, ptlast.Y));
            }
            return true;
        }


        public PolyJig(Point3dCollection pts, Vector3d normal) : base(new Polyline())
        {
            
            Polyline pline = Entity as Polyline;
            for (int i=0;i<pts.Count;i++)
            {
                pline.AddVertexAt(i, new Point2d(pts[i].X, pts[i].Y), 0, 0, 0);
                ptStart = pts[i];
                ptlast = pts[i];
            }
            pline.SetDatabaseDefaults();
            pline.Normal = normal;
            count = pts.Count;

        }

        public Entity GetEntity()
        {
            return Entity;
        }

        
       public void GetPoint3d01(Point3d p1 , Point3d p2)
       {
            Line l = new Line(p1,p2);
            DBObjectCollection dBObject = l.GetOffsetCurves(-120) ;
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
