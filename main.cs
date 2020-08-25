using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal.PropertyInspector;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
                var sortStairs = (from q in stairs orderby q.Num ascending select q).ToList();
                int start = sortStairs[0].Num;
                Point3dCollection pts = new Point3dCollection();
                foreach (var stair in sortStairs)
                {
                    ed.WriteMessage("\nnum="+stair.Num+",type="+stair.type);
                    if (stair.type == "双楼梯")
                    {
                        if (start == stair.Num)
                        {
                            Vector3d vt2 = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                            point += vt2;
                            pts.Add(point);
                            // ed.WriteMessage("\n"+stair.FloorWidth + "," + stair.FloorHeight + "," + stair.LtW + "," + stair.LtH + "," + stair.LTN + "," + stair.ExtH + "," + stair.ExtW + "," + stair.ExtW2 + "," + stair.ExtH2 + "," + -stair.Cover + "," + stair.SW+","+ stair.Sh);
                            db.DrawStair(point, stair.FloorWidth, stair.FloorHeight, stair.LtW, stair.LtH, stair.LTN, stair.ExtW, stair.ExtW2, stair.ExtH, stair.ExtH2, -stair.Cover, stair.SW, stair.Sh, ref lastP);
                        }
                        start++;
                        Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                        point += vt;
                    }
                    else
                    {
                        Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight , 0));
                        point += vt;
                        start++;
                        db.HalfStair(point, stair.FloorWidth, stair.FloorHeight, stair.LtW, stair.LtH, stair.LTN, stair.ExtW, stair.ExtW2, stair.ExtH, stair.ExtH2, -stair.Cover, stair.SW, stair.Sh, ref lastP);
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
        public static void DrawStair(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, double Cover, double SW, double Sh, ref Point2d lastPoint)
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
                trans.Commit();

            }

        }

        public static void HalfStair(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, double Cover, double SW, double Sh, ref Point2d lastPoint)
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
                trans.Commit();
            }

        }

        public static void StairPM(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH, double LtN, double ExtW, double ExtW2, double ExtH, double ExtH2, double Cover, double SW, double Sh, ref Point2d lastPoint)
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
