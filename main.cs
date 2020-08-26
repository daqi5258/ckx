using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(ckx.main))]
namespace ckx {
    public static class main {

        [CommandMethod("mystair")]
        public static void Stair()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptPointOptions ppo = new PromptPointOptions("\n请指定插入点:");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            else
            {
                Point3d point = ppr.Value;
                Point2d lastP = Point2d.Origin;

                List<myStair> stairs = new List<myStair>();
                StairForm stairForm = new StairForm();
                Application.ShowModalDialog(stairForm);
                if (stairForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    stairForm.ShowInTaskbar = false;
                    stairs = stairForm.res;
                }
                if (stairs.Count < 1) return;
                //var sortStairs = (from q in stairs orderby q.Num ascending select q).ToList();
                //int start = sortStairs[0].Num;
                Point3dCollection pts = new Point3dCollection();
                foreach (var stair in stairs)
                {
                    ed.WriteMessage("\nnum=" + stair.Num + ",type=" + stair.type);
                    if (stair.type == "双楼梯")
                    {
                        // if (start == stair.Num)
                        //{
                        Vector3d vt2 = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                        point += vt2;
                        pts.Add(point);
                        // ed.WriteMessage("\n"+stair.FloorWidth + "," + stair.FloorHeight + "," + stair.LtW + "," + stair.LtH + "," + stair.LTN + "," + stair.ExtH + "," + stair.ExtW + "," + stair.ExtW2 + "," + stair.ExtH2 + "," + -stair.Cover + "," + stair.SW+","+ stair.Sh);
                        db.DrawStair(point, stair.FloorWidth, stair.FloorHeight, stair.LtW, stair.LtH, stair.LTN, stair.ExtW, stair.ExtW2, stair.ExtH, stair.ExtH2, -stair.Cover, stair.SW, stair.Sh, ref lastP, stair.Num);
                        // }
                        //start++;
                        Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                        point += vt;
                    }
                    else
                    {
                        Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight, 0));
                        point += vt;
                        // start++;
                        db.HalfStair(point, stair.FloorWidth, stair.FloorHeight, stair.LtW, stair.LtH, stair.LTN, stair.ExtW, stair.ExtW2, stair.ExtH, stair.ExtH2, -stair.Cover, stair.SW, stair.Sh, ref lastP, stair.Num);
                    }
                }

                //db.HalfStair(point, 0, 3600, 260, 150, 12, 1600, 1200, 120, 100, -20, 200, 400, ref lastP);


                // Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                // db.AddToModelSpace(pl,"参考线不打印");

            }
        }

        /// <summary>
        /// 楼梯
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point">楼梯设置点</param>
        /// <param name="FloorWidth">楼宽</param>
        /// <param name="FloorHeight">楼高</param>
        /// <param name="LtW">楼梯宽</param>
        /// <param name="LtH">楼梯高</param>
        /// <param name="ExtW">两边延长</param>
        /// <param name="ExtH">楼梯混凝土厚度</param>
        /// <param name="Cover">楼梯面混凝土厚度</param>
        ///  <param name="SH">楼梯面混凝土厚度</param>
        ///  <param name="SW">楼梯面混凝土宽度</param>
        public static void DrawStair(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, double Cover, double SW, double Sh, ref Point2d lastPoint, string num)
        {
            Point2dCollection pts = new Point2dCollection()
                , pts_1 = new Point2dCollection(),
                pts2 = new Point2dCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Point2d p1 = new Point2d(point.X + ExtW, point.Y);
                pts.Add(p1);
                for (int i = 1; i <= LtN; i++)
                {
                    pts.Add(new Point2d(p1.X + (i - 1) * LtW, p1.Y + i * LtH));
                    if (i != LtN)
                    {
                        pts.Add(new Point2d(p1.X + i * LtW, p1.Y + i * LtH));
                    }
                }
                Point2d plast = pts[pts.Count - 1];

                pts_1.Add(p1); pts_1.Add(new Point2d(point.X, point.Y));
                pts_1.Add(new Point2d(point.X, point.Y - ExtH2));
                pts_1.Add(new Point2d(p1.X - SW, point.Y - ExtH2));
                pts_1.Add(new Point2d(p1.X - SW, point.Y - Sh));
                pts_1.Add(new Point2d(p1.X, point.Y - Sh));
                pts_1.Add(new Point2d(p1.X, point.Y - ExtH));
                pts_1.Add(new Point2d(plast.X, plast.Y - LtH - ExtH));
                pts_1.Add(new Point2d(plast.X, plast.Y - Sh));
                pts_1.Add(new Point2d(plast.X + SW, plast.Y - Sh));
                pts_1.Add(new Point2d(plast.X + SW, plast.Y - ExtH2));
                pts_1.Add(new Point2d(plast.X + ExtW2 - SW, plast.Y - ExtH2));
                pts_1.Add(new Point2d(plast.X + ExtW2 - SW, plast.Y - Sh));
                pts_1.Add(new Point2d(plast.X + ExtW2, plast.Y - Sh));
                pts_1.Add(new Point2d(plast.X + ExtW2, plast.Y));
                pts_1.Add(plast);

                pts2.Add(p1);
                for (int i = 1; i <= LtN; i++)
                {
                    pts2.Add(new Point2d(p1.X + (i - 1) * LtW, p1.Y - i * LtH));
                    if (i != LtN)
                    {
                        pts2.Add(new Point2d(p1.X + i * LtW, p1.Y - i * LtH));
                    }
                }

                Polyline pl = new Polyline();
                Polyline pl_1 = new Polyline();
                Polyline pl2 = new Polyline();
                pl2.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                for (int i = 0; i < pts.Count; i++)
                {
                    pl.AddVertexAt(i, pts[i], 0, 0, 0);
                }
                for (int i = 0; i < pts_1.Count; i++)
                {
                    pl_1.AddVertexAt(i, pts_1[i], 0, 0, 0);
                }
                for (int i = 0; i < pts2.Count; i++)
                {
                    pl2.AddVertexAt(i, pts2[i], 0, 0, 0);
                }

                Polyline line = new Polyline();
                line.AddVertexAt(0, new Point2d(pts2[1].X, pts2[1].Y - ExtH), 0, 0, 0);
                line.AddVertexAt(1, new Point2d(pts2[pts2.Count - 1].X, pts2[pts2.Count - 1].Y - ExtH), 0, 0, 0);
                if (lastPoint != Point2d.Origin && lastPoint.X > pts2[pts2.Count - 1].X)
                {
                    line.AddVertexAt(2, new Point2d(lastPoint.X, pts2[pts2.Count - 1].Y - ExtH), 0, 0, 0);
                }
                LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                if (ltt.Has("DASH"))
                {
                    line.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                    line.LinetypeId = ltt["DASH"];
                }
                db.AddToModelSpace(line, "STAIR");
                if (Cover != 0)
                {
                    Polyline pl_c = pl.Clone() as Polyline;
                    pl_c.AddVertexAt(0, new Point2d(point.X, point.Y), 0, 0, 0);
                    pl_c.AddVertexAt(pl_c.NumberOfVertices, new Point2d(plast.X + ExtW2, plast.Y), 0, 0, 0);
                    Polyline cp1 = pl_c.GetOffsetCurves(Cover)[0] as Polyline;
                    cp1.AddVertexAt(0, pl_c.GetPoint2dAt(0), 0, 0, 0);
                    cp1.AddVertexAt(cp1.NumberOfVertices, pl_c.GetPoint2dAt(pl_c.NumberOfVertices - 1), 0, 0, 0);
                    Polyline cp2 = pl2.GetOffsetCurves(Cover)[0] as Polyline;
                    cp2.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                    cp2.AddVertexAt(0, pl2.GetPoint2dAt(0), 0, 0, 0);
                    Point2d ptCp2 = cp2.GetPoint2dAt(cp2.NumberOfVertices - 1);
                    if (lastPoint != Point2d.Origin && lastPoint.X > ptCp2.X)
                    {
                        cp2.RemoveVertexAt(cp2.NumberOfVertices - 1);
                        cp2.AddVertexAt(cp2.NumberOfVertices, new Point2d(ptCp2.X, ptCp2.Y - Cover), 0, 0, 0);
                        cp2.AddVertexAt(cp2.NumberOfVertices, new Point2d(lastPoint.X, cp2.GetPoint2dAt(cp2.NumberOfVertices - 1).Y), 0, 0, 0);
                        cp2.AddVertexAt(cp2.NumberOfVertices, new Point2d(lastPoint.X, pl2.GetPoint2dAt(pl2.NumberOfVertices - 1).Y), 0, 0, 0);
                    }
                    cp2.AddVertexAt(cp2.NumberOfVertices, pl2.GetPoint2dAt(pl2.NumberOfVertices - 1), 0, 0, 0);

                    db.AddToModelSpace(cp1, "STAIR");
                    db.AddToModelSpace(cp2, "STAIR");


                    Point2dCollection ptsFs = new Point2dCollection();
                    ptsFs.Add(new Point2d(cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).X, cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).Y + 900));
                    ptsFs.Add(new Point2d(cp1.GetPoint2dAt(3).X, cp1.GetPoint2dAt(3).Y + 900));
                    //ptsFs.Add(new Point2d(cp1.GetPoint2dAt(3).X, cp2.GetPoint2dAt(1).Y + 900 + LtH));
                    ptsFs.Add(new Point2d(cp2.GetPoint2dAt(1).X, cp2.GetPoint2dAt(1).Y + 900));
                    ptsFs.Add(new Point2d(ptCp2.X, ptCp2.Y + 900 + LtH - Cover));
                    if (lastPoint != Point2d.Origin)
                    {
                        if (lastPoint.X > ptCp2.X)
                            ptsFs.Add(new Point2d(lastPoint.X, ptCp2.Y + 900 + LtH - Cover));
                        ptsFs.Add(lastPoint);
                    }
                    Polyline fs = new Polyline();
                    for (int i = 0; i < ptsFs.Count; i++)
                    {
                        fs.AddVertexAt(i, ptsFs[i], 0, 0, 0);
                    }
                    db.AddToModelSpace(fs, "J通-栏杆、百叶、格栅、配件");

                    Line l01 = new Line(new Point3d(ptCp2.X, ptCp2.Y + 900 + LtH - Cover, 0), new Point3d(ptCp2.X, ptCp2.Y - Cover + LtH, 0));
                    Line l02 = new Line(new Point3d(cp2.GetPoint2dAt(1).X, cp2.GetPoint2dAt(1).Y + 900, 0), new Point3d(cp2.GetPoint2dAt(1).X, cp2.GetPoint2dAt(1).Y, 0));
                    Line l03 = new Line(new Point3d(cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).X, cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).Y + 900, 0), new Point3d(cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).X, cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).Y, 0));
                    Line l04 = new Line(new Point3d(cp1.GetPoint2dAt(3).X, cp1.GetPoint2dAt(3).Y + 900, 0), new Point3d(cp1.GetPoint2dAt(3).X, cp1.GetPoint2dAt(3).Y, 0));

                    db.AddToModelSpace(l01, "J通-栏杆、百叶、格栅、配件");
                    db.AddToModelSpace(l02, "J通-栏杆、百叶、格栅、配件");
                    db.AddToModelSpace(l03, "J通-栏杆、百叶、格栅、配件");
                    db.AddToModelSpace(l04, "J通-栏杆、百叶、格栅、配件");
                    lastPoint = new Point2d(cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).X, cp1.GetPoint2dAt(cp1.NumberOfVertices - 3).Y + 900);
                }

                // Line line = new Line(new Point3d(pts2[1].X, pts2[1].Y - ExtH, 0)
                //    , new Point3d(pts2[pts2.Count-1].X, pts2[pts2.Count - 1].Y - ExtH, 0));

                ObjectIdCollection objs = new ObjectIdCollection();
                objs.Add(db.AddToModelSpace(pl, "STAIR"));
                objs.Add(db.AddToModelSpace(pl_1, "STAIR"));
                db.AddToModelSpace(pl2, "STAIR");



                Hatch hatch = new Hatch();
                hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                hatch.AppendLoop(HatchLoopTypes.Default, objs);
                hatch.EvaluateHatch(true);
                db.AddToModelSpace(hatch, "PUB_HATCH");

                AlignedDimension dim = new AlignedDimension();
                dim.XLine1Point = new Point3d(point.X, point.Y - FloorHeight / 2, 0);
                dim.XLine2Point = new Point3d(point.X, point.Y + FloorHeight / 2, 0);
                dim.DimLinePoint = new Point3d(point.X - 300, point.Y, 0);
                dim.DimensionText = num;
                AddToModelSpace(db, dim, "");

                StairPM(db, point, FloorWidth, FloorHeight, LtW, LtN, ExtW, ExtW2, 50, 1160, num);
                trans.Commit();

            }

        }

        public static void HalfStair(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, double Cover, double SW, double Sh, ref Point2d lastPoint, string num)
        {
            Point2dCollection pts = new Point2dCollection()
                , pts_1 = new Point2dCollection(),
                pts2 = new Point2dCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Point2d p0 = new Point2d(point.X + ExtW, point.Y);
                pts.Add(new Point2d(point.X, point.Y)); pts.Add(p0);
                for (int i = 1; i <= LtN; i++)
                {
                    pts.Add(new Point2d(p0.X + (i - 1) * LtW, p0.Y - i * LtH));
                    if (i != LtN)
                    {
                        pts.Add(new Point2d(p0.X + i * LtW, p0.Y - i * LtH));
                    }
                }

                Polyline pl = new Polyline();
                for (int i = 0; i < pts.Count; i++)
                {
                    pl.AddVertexAt(i, pts[i], 0, 0, 0);
                }
                AddToModelSpace(db, pl, "STAIR");
                Point2d plast = pts[pts.Count - 1];
                if (Cover != 0)
                {
                    Polyline pl_1 = pl.GetOffsetCurves(Cover)[0] as Polyline;
                    pl_1.AddVertexAt(0, new Point2d(point.X, point.Y), 0, 0, 0);
                    pl_1.AddVertexAt(pl_1.NumberOfVertices, plast, 0, 0, 0);
                    AddToModelSpace(db, pl_1, "STAIR");
                }

                Line line = new Line(new Point3d(p0.X, p0.Y - LtH - ExtH, 0), new Point3d(plast.X, plast.Y - ExtH, 0));
                LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                if (ltt.Has("DASH"))
                {
                    line.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                    line.LinetypeId = ltt["DASH"];
                }
                AddToModelSpace(db, line, "STAIR");
                Line l01 = new Line(new Point3d(p0.X - Cover, p0.Y - Cover + 900, 0), new Point3d(p0.X - Cover, p0.Y - Cover, 0));
                Line l02 = new Line(new Point3d(plast.X - Cover, plast.Y - Cover + LtH, 0), new Point3d(plast.X - Cover, plast.Y - Cover + LtH + 900, 0));
                Line l03 = new Line(new Point3d(p0.X - Cover, p0.Y - Cover + 900, 0), new Point3d(plast.X - Cover, plast.Y - Cover + LtH + 900, 0));
                AddToModelSpace(db, l01, "J通-栏杆、百叶、格栅、配件");
                AddToModelSpace(db, l02, "J通-栏杆、百叶、格栅、配件");
                AddToModelSpace(db, l03, "J通-栏杆、百叶、格栅、配件");

                Polyline pl2 = new Polyline();
                pl2.AddVertexAt(0, new Point2d(point.X, point.Y), 0, 0, 0);
                pl2.AddVertexAt(1, new Point2d(point.X, point.Y - ExtH2), 0, 0, 0);
                pl2.AddVertexAt(2, new Point2d(point.X + ExtW - SW, point.Y - ExtH2), 0, 0, 0);
                pl2.AddVertexAt(3, new Point2d(point.X + ExtW - SW, point.Y - Sh), 0, 0, 0);
                pl2.AddVertexAt(4, new Point2d(point.X + ExtW, point.Y - Sh), 0, 0, 0);
                pl2.AddVertexAt(5, p0, 0, 0, 0);
                AddToModelSpace(db, pl2, "STAIR");

                AlignedDimension dim = new AlignedDimension();
                dim.XLine1Point = new Point3d(point.X, point.Y - FloorHeight, 0);
                dim.XLine2Point = new Point3d(point.X, point.Y , 0);
                dim.DimLinePoint = new Point3d(point.X - 300, point.Y, 0);
                dim.DimensionText = num;
                AddToModelSpace(db, dim, "");
                trans.Commit();
            }

        }

        public static void StairPM(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, string num)
        {
            Point2dCollection pts = new Point2dCollection()
                , pts_1 = new Point2dCollection(),
                pts2 = new Point2dCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Point3d ptR = new Point3d(point.X - 1500 - ExtW2, point.Y, 0);
                Point3d ptL = new Point3d(point.X - 1500 - ExtW2 - LtW * (LtN - 2), point.Y, 0);

                Line line1 = new Line(new Point3d(ptR.X, ptR.Y + ExtH, 0), new Point3d(ptL.X, ptL.Y + ExtH, 0));
                Line line2 = new Line(new Point3d(ptR.X, ptR.Y - ExtH, 0), new Point3d(ptL.X, ptL.Y - ExtH, 0));
                Line line3 = new Line(new Point3d(ptR.X, ptR.Y + ExtH + ExtH2, 0), new Point3d(ptL.X, ptL.Y + ExtH + ExtH2, 0));
                Line line4 = new Line(new Point3d(ptR.X, ptR.Y - ExtH - ExtH2, 0), new Point3d(ptL.X, ptL.Y - ExtH - ExtH2, 0));
                AddToModelSpace(db, line1, "STAIR");
                AddToModelSpace(db, line2, "STAIR");
                AddToModelSpace(db, line3, "STAIR");
                AddToModelSpace(db, line4, "STAIR");


                for (int i = 0; i < LtN - 1; i++)
                {
                    Line l1 = new Line(new Point3d(ptR.X - i * LtW, ptR.Y + ExtH, 0), new Point3d(ptR.X - i * LtW, ptR.Y + ExtH + ExtH2, 0));
                    Line l2 = new Line(new Point3d(ptR.X - i * LtW, ptR.Y - ExtH, 0), new Point3d(ptR.X - i * LtW, ptR.Y - ExtH - ExtH2, 0));
                    AddToModelSpace(db, l1, "STAIR");
                    AddToModelSpace(db, l2, "STAIR");
                }
                Arc arc1 = new Arc();
                arc1.Center = new Point3d(ptR.X, ptR.Y + ExtH, 0);
                arc1.StartAngle = 0;
                arc1.EndAngle = Math.PI / 2;
                arc1.Radius = ExtH2;
                arc1.Linetype = "DASH";
                AddToModelSpace(db, arc1, "STAIR");
                Arc arc2 = new Arc();
                arc2.Center = new Point3d(ptR.X, ptR.Y - ExtH, 0);
                arc2.StartAngle = -Math.PI / 2;
                arc2.EndAngle = 0;
                arc2.Radius = ExtH2;
                arc2.Linetype = "DASH";
                AddToModelSpace(db, arc2, "STAIR");
                Arc arc3 = new Arc();
                arc3.Center = new Point3d(ptL.X, ptL.Y + ExtH, 0);
                arc3.StartAngle = Math.PI / 2; ;
                arc3.EndAngle = Math.PI;
                arc3.Radius = ExtH2;
                arc3.Linetype = "DASH";
                AddToModelSpace(db, arc3, "STAIR");
                Arc arc4 = new Arc();
                arc4.Center = new Point3d(ptL.X, ptL.Y - ExtH, 0);
                arc4.StartAngle = Math.PI;
                arc4.EndAngle = Math.PI * 3 / 2;
                arc4.Radius = ExtH2;
                arc4.Linetype = "DASH";
                AddToModelSpace(db, arc4, "STAIR");
                MText t1 = new MText();
                t1.Contents = "下";
                t1.Height = 300;
                t1.Attachment = AttachmentPoint.MiddleCenter;
                t1.Location = new Point3d(ptR.X + LtW, ptR.Y - ExtH - ExtH2 / 2, 0);
                AddToModelSpace(db, t1, "");
                MText t2 = new MText();
                t2.Contents = "上";
                t2.Height = 300;

                t2.Attachment = AttachmentPoint.MiddleCenter;
                t2.Location = new Point3d(ptR.X + LtW, ptR.Y + ExtH + ExtH2 / 2, 0);
                AddToModelSpace(db, t2, "");

                Leader leader1 = new Leader();
                leader1.Annotation = t1.ObjectId;
                leader1.AppendVertex(new Point3d(ptL.X - LtW, ptL.Y - ExtH - ExtH2 / 2, 0));
                leader1.AppendVertex(t1.Location);
                AddToModelSpace(db, leader1, "DIM_SYMB");

                Leader leader2 = new Leader();
                leader2.Annotation = t2.ObjectId;
                leader2.AppendVertex(new Point3d(ptL.X - LtW, ptL.Y + ExtH + ExtH2 / 2, 0));
                leader2.AppendVertex(t2.Location);
                AddToModelSpace(db, leader2, "DIM_SYMB");

                AlignedDimension dim = new AlignedDimension();
                dim.XLine1Point = new Point3d(ptR.X, ptR.Y - FloorHeight / 2, 0);
                dim.XLine2Point = new Point3d(ptL.X, ptL.Y - FloorHeight / 2, 0);
                dim.DimLinePoint = new Point3d((ptR.X + ptL.X) / 2, ptL.Y - FloorHeight / 2, 0);
                dim.DimensionText = num;
                AddToModelSpace(db, dim, "");

                trans.Commit();
            }

        }


        [CommandMethod("xp")]
        public static void xp()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Point3d p1 = new Point3d(-27456.79, -20385.32, 0), p2 = new Point3d(-13453.43, -3185.32, 0);
            double r1 = 8600, r2 = 8600;
            double angle1 = 65.22 / 180.0 * Math.PI,
                angle2 = 43.6 / 180.0 * Math.PI,
                anglel = 0 / 180.0 * Math.PI;
            double h = 7600, dtq = 200;
            double length1 = 3600, length2 = 3600;

            PromptPointOptions ppo = new PromptPointOptions("\n请输入左下点:");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            else
                p1 = ppr.Value;
            XP xp = new XP(p1, r1, r2, angle1, angle2, anglel, h, dtq, length1, length2);
            PromptResult pr = ed.Drag(xp);
            if (pr.Status != PromptStatus.OK) return;
            else
            {
                List<Entity> ents = xp.GetEntity();
                foreach (var ent in ents)
                {
                    AddToModelSpace(db, (Entity)ent.Clone(), "");
                }
            }




            // drawXP(db,p1,p2,r1,r2,angle1,angle2,anglel,h,dtq,length1,length2);
            //drawXP(db, new Point3d(1000,1000,0), new Point3d(10000, 10000, 0), r1, r2, angle1, angle2, 0, h, dtq, length1, length2);
            // drawXP(db, new Point3d(1000, 11000, 0), new Point3d(10000, 20000, 0), r1, r2, angle1, angle2, anglel, h, dtq, length1, length2);

        }
        public static void drawXP(Database db, Point3d p1, Point3d p2, double r1, double r2, double angle1, double angle2, double anglel, double h, double dtq, double length1, double length2)
        {

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                Point3d plc1 = new Point3d(p1.X - r1 * Math.Sin(anglel), p1.Y + r1 * Math.Cos(anglel), 0);
                Point3d plc2 = new Point3d(p2.X + r2 * Math.Sin(anglel), p2.Y - r2 * Math.Cos(anglel), 0);

                Line linel = new Line(plc1, plc2);
                Arc arc1 = new Arc();
                arc1.Center = p1;
                arc1.StartAngle = Math.PI / 2 + anglel;
                arc1.EndAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
                arc1.Radius = r1;
                Arc arc1_1 = new Arc();
                arc1_1.Center = p1;
                arc1_1.StartAngle = Math.PI / 2 + anglel + angle1 - length1 / r1;
                arc1_1.EndAngle = Math.PI / 2 + anglel + angle1;
                arc1_1.Radius = r1;

                Arc arc1_2 = new Arc();
                arc1_2.Center = p1;
                arc1_2.StartAngle = Math.PI / 2 + anglel;
                arc1_2.EndAngle = Math.PI / 2 + anglel + angle1;
                arc1_2.Radius = r1 - h / 2;
                Arc arc1_3 = new Arc();
                arc1_3.Center = p1;
                arc1_3.StartAngle = Math.PI / 2 + anglel;
                arc1_3.EndAngle = Math.PI / 2 + anglel + angle1;
                arc1_3.Radius = r1 + h / 2;


                Arc arc2 = new Arc();
                arc2.Center = p2;
                arc2.StartAngle = Math.PI * 3 / 2 + anglel;
                arc2.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r2;
                arc2.Radius = r2;
                Arc arc2_1 = new Arc();
                arc2_1.Center = p2;
                arc2_1.StartAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r2;
                arc2_1.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                arc2_1.Radius = r2;
                Arc arc2_2 = new Arc();
                arc2_2.Center = p2;
                arc2_2.StartAngle = Math.PI * 3 / 2 + anglel;
                arc2_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                arc2_2.Radius = r2 - h / 2;
                Arc arc2_3 = new Arc();
                arc2_3.Center = p2;
                arc2_3.StartAngle = Math.PI * 3 / 2 + anglel;
                arc2_3.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                arc2_3.Radius = r2 + h / 2;

                Line linel_1 = new Line(arc1_2.StartPoint, arc2_3.StartPoint);
                Line linel_2 = new Line(arc1_3.StartPoint, arc2_2.StartPoint);

                Line l0 = new Line(arc1_2.EndPoint, arc1_3.EndPoint);
                Line l2 = new Line(arc2_2.EndPoint, arc2_3.EndPoint);
                AddToModelSpace(db, linel, "J平-车行道中心线");
                AddToModelSpace(db, linel_1, "参考线不打印");
                AddToModelSpace(db, linel_2, "参考线不打印");
                AddToModelSpace(db, arc1, "J平-车行道中心线");
                AddToModelSpace(db, arc1_1, "J平-车行道中心线");
                AddToModelSpace(db, arc1_2, "参考线不打印");
                AddToModelSpace(db, arc1_3, "参考线不打印");
                AddToModelSpace(db, arc2, "J平-车行道中心线");
                AddToModelSpace(db, arc2_1, "J平-车行道中心线");
                AddToModelSpace(db, arc2_2, "参考线不打印");
                AddToModelSpace(db, arc2_3, "参考线不打印");
                AddToModelSpace(db, l0, "参考线不打印");
                AddToModelSpace(db, l2, "参考线不打印");

                if (dtq > 0)
                {
                    Arc arc1_2_2 = new Arc();
                    arc1_2_2.Center = p1;
                    arc1_2_2.StartAngle = Math.PI / 2 + anglel;
                    arc1_2_2.EndAngle = Math.PI / 2 + anglel + angle1;
                    arc1_2_2.Radius = r1 - h / 2 + dtq;
                    Arc arc1_3_2 = new Arc();
                    arc1_3_2.Center = p1;
                    arc1_3_2.StartAngle = Math.PI / 2 + anglel;
                    arc1_3_2.EndAngle = Math.PI / 2 + anglel + angle1;
                    arc1_3_2.Radius = r1 + h / 2 - dtq;
                    AddToModelSpace(db, arc1_3_2, "");
                    AddToModelSpace(db, arc1_2_2, "");
                    Line leftLine = new Line(arc1_2_2.StartPoint, arc1_3_2.StartPoint);
                    AddToModelSpace(db, leftLine, "");



                    Arc arc2_2_2 = new Arc();
                    arc2_2_2.Center = p2;
                    arc2_2_2.StartAngle = Math.PI * 3 / 2 + anglel;
                    arc2_2_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                    arc2_2_2.Radius = r2 - h / 2 + dtq;
                    Arc arc2_3_2 = new Arc();
                    arc2_3_2.Center = p2;
                    arc2_3_2.StartAngle = Math.PI * 3 / 2 + anglel;
                    arc2_3_2.EndAngle = Math.PI * 3 / 2 + anglel + angle2;
                    arc2_3_2.Radius = r2 + h / 2 - dtq;
                    Line RightLine = new Line(arc2_2_2.StartPoint, arc2_3_2.StartPoint);
                    AddToModelSpace(db, RightLine, "");
                    AddToModelSpace(db, arc2_2_2, "");
                    AddToModelSpace(db, arc2_3_2, "");
                    Line linel_1_2 = new Line(arc1_2_2.StartPoint, arc2_3_2.StartPoint);
                    Line linel_2_2 = new Line(arc1_3_2.StartPoint, arc2_2_2.StartPoint);
                    AddToModelSpace(db, linel_1_2, "");
                    AddToModelSpace(db, linel_2_2, "");
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
                Line left = new Line(tmp1.EndPoint, tmp2.EndPoint);
                AddToModelSpace(db, left, "");

                tmp1.Center = p2;
                tmp1.StartAngle = Math.PI * 3 / 2 + anglel;
                tmp1.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r1;
                tmp1.Radius = r2 - h / 2 + dtq;
                tmp2.Center = p2;
                tmp2.StartAngle = Math.PI * 3 / 2 + anglel;
                tmp2.EndAngle = Math.PI * 3 / 2 + anglel + angle2 - length2 / r1;
                tmp2.Radius = r2 + h / 2 - dtq;
                Line right = new Line(tmp1.EndPoint, tmp2.EndPoint);
                AddToModelSpace(db, right, "");

                trans.Commit();

            }

        }











        /// <summary>
        /// closed due to xclip
        /// </summary>
        [CommandMethod("tmirror")]
        public static void My_Mirror()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                PromptSelectionResult psr = ed.SelectImplied();
                SelectionSet ss;
                if (psr.Status != PromptStatus.OK || psr.Value == null)
                {
                    PromptSelectionOptions pso = new PromptSelectionOptions();
                    pso.MessageForAdding = "\n请选择需要镜像的对象:";
                    psr = ed.GetSelection(pso);
                    if (psr.Status == PromptStatus.OK)
                    {
                        ss = psr.Value;
                    }
                    else
                        return;
                }
                else
                    ss = psr.Value;

                PromptPointOptions ppo = new PromptPointOptions("\n请指定基点");
                ppo.AllowNone = true;
                List<Entity> ents = new List<Entity>();
                foreach (var id in ss.GetObjectIds())
                {
                    Entity ent = id.GetObject(OpenMode.ForRead) as Entity;
                    ents.Add((Entity)ent.Clone());
                }
                PromptPointResult pprc = ed.GetPoint(ppo);
                if (pprc.Status != PromptStatus.OK) return;
                else
                {
                    Point3d p = pprc.Value;
                    EntityChangeTools entChangeJig = new EntityChangeTools(ents, p, "J");
                    PromptResult pr = ed.Drag(entChangeJig);
                    if (pr.Status != PromptStatus.OK) return;
                    else
                    {
                        foreach (Entity ent in ents)
                        {
                            if (ent.GetType() == typeof(MText))
                            {
                                MText text = ent as MText;
                                double length = text.Width;
                                Point3d pL = text.Location;
                                ed.WriteMessage("\np=" + p + ",l=" + length + ",ro=" + text.Rotation);
                                if (text.Rotation == 0)
                                {
                                    Line3d l = new Line3d(new Point3d(pL.X , pL.Y, 0), new Point3d(pL.X , 0, 0));
                                    Matrix3d mtl = Matrix3d.Mirroring(l);
                                    text.TransformBy(mtl);
                                }
                                else if (text.Rotation == Math.PI / 2 || text.Rotation == Math.PI*3 / 2)
                                {
                                    Line3d l = new Line3d(new Point3d(pL.X, pL.Y + length / 2, 0), new Point3d(0, pL.Y + length / 2, 0));
                                    Matrix3d mtl = Matrix3d.Mirroring(l);
                                    text.TransformBy(mtl);
                                }
                            }
                            AddToModelSpace(db, ent, "0");
                        }
                    }
                }

                trans.Commit();
            }

        }









        public static ObjectId AddToModelSpace(this Database db, Entity ent, String LayerName)
        {
            ObjectId objectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                using (BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                {
                    LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                    if (lt.Has(LayerName))
                    {
                        ent.Layer = LayerName;
                    }
                    objectId = btr.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);

                }
                trans.Commit();
            }
            return objectId;
        }



        /*
        public static IDictionary<string, object> GetOPMProperties(ObjectId id)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            IntPtr pUnk = ObjectPropertyManagerPropertyUtility.GetIUnknownFromObjectId(id);
            if (pUnk != IntPtr.Zero)
            {
                using (CollectionVector properties = ObjectPropertyManagerProperties.GetProperties(id, false, false))
                {
                    int cnt = properties.Count();
                    if (cnt != 0)
                    {
                        using (CategoryCollectable category = properties.Item(0) as CategoryCollectable)
                        {
                            CollectionVector props = category.Properties;
                            int propCount = props.Count();
                            for (int j = 0; j < propCount; j++)
                            {
                                using (PropertyCollectable prop = props.Item(j) as PropertyCollectable)
                                {
                                    if (prop == null)
                                        continue;
                                    object value = null;
                                    if (prop.GetValue(pUnk, ref value) && value != null)
                                    {
                                        if (!map.ContainsKey(prop.Name))
                                            map[prop.Name] = value;
                                    }
                                }
                            }
                        }
                    }
                }
                Marshal.Release(pUnk);
            }
            return map;
        }

    */

    }
}
