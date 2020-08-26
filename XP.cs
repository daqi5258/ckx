using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;

namespace ckx {
    class XP : DrawJig {

        public Point3d p1, p2, ptlast;
        public double r1, r2;
        public double angle1, angle2, anglel;
        public double h, dtq;
        public double length1, length2;


        public Line linel, leftLine, linel_1, linel_2, left, right, l2, l0, linel_1_2, linel_2_2, RightLine;

        public Arc arc1, arc1_1, arc1_2, arc1_3, arc1_2_2, arc1_3_2;
        public Arc arc2, arc2_1, arc2_2, arc2_3, arc2_2_2, arc2_3_2;

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions ppo = new JigPromptPointOptions("\n请输入右上点:");
            ppo.UseBasePoint = true;
            ppo.BasePoint = p1;
            ppo.UserInputControls = UserInputControls.Accept3dCoordinates;

            PromptPointResult ppr = prompts.AcquirePoint(ppo);
            ptlast = ppr.Value;

            if (p2 == ptlast)
                return SamplerStatus.NoChange;
            p2 = ptlast;

            if (ppr.Status == PromptStatus.OK)
            {
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.Cancel;

        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            if (p1 == p2)
                return true;
            Point3d plc1 = new Point3d(p1.X - r1 * Math.Sin(anglel), p1.Y + r1 * Math.Cos(anglel), 0);
            Point3d plc2 = new Point3d(p2.X + r2 * Math.Sin(anglel), p2.Y - r2 * Math.Cos(anglel), 0);

            linel = new Line(plc1, plc2);
          
            arc1 = new Arc();
            arc1.Center = p1;
            arc1.StartAngle = Math.PI / 2 + anglel;
            arc1.EndAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
            arc1.Radius = r1;
            
            arc1_1 = new Arc();
            arc1_1.Center = p1;
            arc1_1.StartAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
            arc1_1.EndAngle = Math.PI / 2 + anglel + angle1;
            arc1_1.Radius = r1;
           

            arc1_2 = new Arc();
            arc1_2.Center = p1;
            arc1_2.StartAngle = Math.PI / 2 + anglel;
            arc1_2.EndAngle = Math.PI / 2 + anglel + angle1;
            arc1_2.Radius = r1 - h / 2;
            arc1_3 = new Arc();
            arc1_3.Center = p1;
            arc1_3.StartAngle = Math.PI / 2 + anglel;
            arc1_3.EndAngle = Math.PI / 2 + anglel + angle1;
            arc1_3.Radius = r1 + h / 2;


            arc2 = new Arc();
            arc2.Center = p2;
            arc2.StartAngle = Math.PI * 3 / 2 + anglel;
            arc2.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r2;
            arc2.Radius = r2;
            
            arc2_1 = new Arc();
            arc2_1.Center = p2;
            arc2_1.StartAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r2;
            arc2_1.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
            arc2_1.Radius = r2;
            
            arc2_2 = new Arc();
            arc2_2.Center = p2;
            arc2_2.StartAngle = Math.PI * 3 / 2 + anglel;
            arc2_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
            arc2_2.Radius = r2 - h / 2;
            arc2_3 = new Arc();
            arc2_3.Center = p2;
            arc2_3.StartAngle = Math.PI * 3 / 2 + anglel;
            arc2_3.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
            arc2_3.Radius = r2 + h / 2;

            linel_1 = new Line(arc1_2.StartPoint, arc2_3.StartPoint);
            linel_2 = new Line(arc1_3.StartPoint, arc2_2.StartPoint);

            l0 = new Line(arc1_2.EndPoint, arc1_3.EndPoint);
            l2 = new Line(arc2_2.EndPoint, arc2_3.EndPoint);
            draw.Geometry.Draw(linel);
            draw.Geometry.Draw(linel_1);
            draw.Geometry.Draw(linel_2);
            draw.Geometry.Draw(arc1);
            draw.Geometry.Draw(arc1_1);
            draw.Geometry.Draw(arc1_2);
            draw.Geometry.Draw(arc1_3);
            draw.Geometry.Draw(arc2);
            draw.Geometry.Draw(arc2_1);
            draw.Geometry.Draw(arc2_2);
            draw.Geometry.Draw(arc2_3);
            draw.Geometry.Draw(l0);
            draw.Geometry.Draw(l2);

            if (dtq > 0)
            {
                arc1_2_2 = new Arc();
                arc1_2_2.Center = p1;
                arc1_2_2.StartAngle = Math.PI / 2 + anglel;
                arc1_2_2.EndAngle = Math.PI / 2 + anglel + angle1;
                arc1_2_2.Radius = r1 - h / 2 + dtq;
                arc1_3_2 = new Arc();
                arc1_3_2.Center = p1;
                arc1_3_2.StartAngle = Math.PI / 2 + anglel;
                arc1_3_2.EndAngle = Math.PI / 2 + anglel + angle1;
                arc1_3_2.Radius = r1 + h / 2 - dtq;
                draw.Geometry.Draw(arc1_3_2);
                draw.Geometry.Draw(arc1_2_2);
                leftLine = new Line(arc1_2_2.StartPoint, arc1_3_2.StartPoint);
                draw.Geometry.Draw(leftLine);



                arc2_2_2 = new Arc();
                arc2_2_2.Center = p2;
                arc2_2_2.StartAngle = Math.PI * 3 / 2 + anglel;
                arc2_2_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                arc2_2_2.Radius = r2 - h / 2 + dtq;
                arc2_3_2 = new Arc();
                arc2_3_2.Center = p2;
                arc2_3_2.StartAngle = Math.PI * 3 / 2 + anglel;
                arc2_3_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                arc2_3_2.Radius = r2 + h / 2 - dtq;
                RightLine = new Line(arc2_2_2.StartPoint, arc2_3_2.StartPoint);
                draw.Geometry.Draw(RightLine);
                draw.Geometry.Draw(arc2_2_2);
                draw.Geometry.Draw(arc2_3_2);
                linel_1_2 = new Line(arc1_2_2.StartPoint, arc2_3_2.StartPoint);
                linel_2_2 = new Line(arc1_3_2.StartPoint, arc2_2_2.StartPoint);
                draw.Geometry.Draw(linel_1_2);
                draw.Geometry.Draw(linel_2_2);
            }





            Arc tmp1 = new Arc();
            tmp1.Center = p1;
            tmp1.StartAngle = Math.PI / 2 + anglel;
            tmp1.EndAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
            tmp1.Radius = r1 - h / 2 + dtq;
            Arc tmp2 = new Arc();
            tmp2.Center = p1;
            tmp2.StartAngle = Math.PI / 2 + anglel;
            tmp2.EndAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
            tmp2.Radius = r1 + h / 2 - dtq;
            left = new Line(tmp1.EndPoint, tmp2.EndPoint);
            draw.Geometry.Draw(left);

            tmp1.Center = p2;
            tmp1.StartAngle = Math.PI * 3 / 2 + anglel;
            tmp1.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r1;
            tmp1.Radius = r2 - h / 2 + dtq;
            tmp2.Center = p2;
            tmp2.StartAngle = Math.PI * 3 / 2 + anglel;
            tmp2.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r1;
            tmp2.Radius = r2 + h / 2 - dtq;
            right = new Line(tmp1.EndPoint, tmp2.EndPoint);
            draw.Geometry.Draw(right);
            try
            {
                arc2_1.Layer = "J平-车行道中心线";
                arc2.Layer = "J平-车行道中心线";
                arc1_1.Layer = "J平-车行道中心线";
                arc1.Layer = "J平-车行道中心线";
                linel.Layer = "J平-车行道中心线";
            }
            catch
            {
               // Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("没有图层:J平-车行道中心线");
            }
            return true;
        }

        public XP(Point3d p1, double r1, double r2, double angle1, double angle2, double anglel, double h, double dtq, double length1, double length2)
        {
            this.p1 = p1;
            p2 = p1;
            this.r1 = r1;
            this.r2 = r2;
            this.angle1 = angle1;
            this.angle2 = angle2;
            this.anglel = anglel;
            this.h = h;
            this.dtq = dtq;
            this.length1 = length1;
            this.length2 = length2;
        }

        public List<Entity> GetEntity()
        {
            List<Entity> ents = new List<Entity>();
            ents.Add(linel);
            ents.Add(linel_1);
            ents.Add(linel_2);
            ents.Add(left);
            ents.Add(right);
            ents.Add(l2);
            ents.Add(l0);
            ents.Add(linel_1_2);
            ents.Add(linel_2_2);
            ents.Add(RightLine);
            ents.Add(leftLine);
            ents.Add(arc1);
            ents.Add(arc1_1);
            ents.Add(arc1_2);
            ents.Add(arc1_3);
            ents.Add(arc1_2_2);
            ents.Add(arc1_3_2);
            ents.Add(arc2);
            ents.Add(arc2_1);
            ents.Add(arc2_2);
            ents.Add(arc2_3);
            ents.Add(arc2_2_2);
            ents.Add(arc2_3_2);
            return ents;
        }
    }
}
