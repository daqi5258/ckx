using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ckx
{
    class mline : EntityJig
    {



        public Point3d  ptlast, ptcurrent;
        public bool flag = true;
        public int count;
        public double width = 240.0;
        public Point3dCollection pts = new Point3dCollection();

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Mline mline = Entity as Mline;
            JigPromptPointOptions ppo = new JigPromptPointOptions("\n 请指定下一点");
            ppo.Keywords.Add(" ", " ", " ");
            ppo.Keywords.Default = " ";
            ppo.UseBasePoint = true;
            if (mline.NumberOfVertices > 1)
                ppo.BasePoint = mline.VertexAt(mline.NumberOfVertices-2); 
            else
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

                if (count == 1)
                {

                    mline.AppendSegment(ptcurrent);
                    ptlast = ptcurrent;
                    count = 2;
                    pts.Add(ptlast);
                    return true;
                }
                //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("c=" + count + ",n=" + mline.NumberOfVertices);

                if (count == mline.NumberOfVertices)
                {
                    pts.RemoveAt(count);
                    mline.RemoveLastSegment(ptlast);
                    mline.AppendSegment(ptcurrent);
                    ptlast = ptcurrent;
                    pts.Add(ptlast);
                }
                else if (count == (mline.NumberOfVertices + 1))
                {
                    //pts.RemoveAt(count);
                    mline.AppendSegment(ptcurrent);
                    pts.Add(ptlast);
                }

            }
            return true;
        }


        public mline(Point3d point, Vector3d normal) : base(new Mline())
        { 
            Mline mline = Entity as Mline;
            mline.Normal = normal;
            mline.ColorIndex = 1;
            mline.Style = Application.DocumentManager.MdiActiveDocument.Database.CmlstyleID;
            mline.AppendSegment(point);
            pts.Add(point);
            ptlast = point;
            count = 1;
        }

        public Entity GetEntity()
        {
            return Entity;
        }




       
    }
}


