using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows.OPM;
using OPMNetSample;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(ckx.main))]
namespace ckx
{
    public static class main
    {

        /// <summary>
        /// 楼梯参考线,3点
        /// </summary>
        [CommandMethod("ckx")]
        public static void getCKL()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                Point3dCollection pts = new Point3dCollection();
                PromptPointOptions ppo = new PromptPointOptions("请选择初始点:");
                PromptPointResult ppr;

                ppr = doc.Editor.GetPoint(ppo);
                Point3d p1 = new Point3d(ppr.Value.X, ppr.Value.Y, ppr.Value.Z);
                if (ppr.Status == PromptStatus.Cancel) return;
                ppo.Message = "请选择第二点:";
                ppr = doc.Editor.GetPoint(ppo);
                Point3d p2 = new Point3d(ppr.Value.X, ppr.Value.Y, ppr.Value.Z);
                if (ppr.Status == PromptStatus.Cancel) return;
                ppo.Message = "请选择终点:";
                ppr = doc.Editor.GetPoint(ppo);
                Point3d p3 = new Point3d(ppr.Value.X, ppr.Value.Y, ppr.Value.Z);
                if (ppr.Status == PromptStatus.Cancel) return;

                PromptDoubleOptions pdo = new PromptDoubleOptions("请输入高度:");
                pdo.DefaultValue = 2300;
                double pyY = pdo.DefaultValue; ;
                PromptDoubleResult pdr = doc.Editor.GetDouble(pdo);
                if (pdr.Status == PromptStatus.OK)
                    pyY = pdr.Value;
                else if (pdr.Status == PromptStatus.Cancel) return;

                pdo = new PromptDoubleOptions("请输入延展长度:");
                pdo.DefaultValue = 350;
                double ycX = pdo.DefaultValue;
                pdr = doc.Editor.GetDouble(pdo);
                if (pdr.Status == PromptStatus.OK)
                    ycX = pdr.Value;
                else if (pdr.Status == PromptStatus.Cancel) return;
                double h1 = p2.Y - p1.Y;



                Point3d p0 = new Point3d(p2.X - ycX, p2.Y, p2.Z);
                Point3d p4 = new Point3d(p3.X + ycX, p3.Y, p3.Z);
                Point3d p5 = new Point3d(p0.X, p0.Y - h1 - (pyY - 2100), p0.Z);
                Point3d p7 = new Point3d(p4.X, p4.Y - (pyY - 2100), p4.Z);
                Point3d p8 = new Point3d(p4.X - ycX + 2000, p4.Y - (pyY - 2100), p4.Z);
                Point3d p6 = new Point3d(p0.X + ycX - 2000, p0.Y - h1 - (pyY - 2100), p0.Z);

                if (p2.X > p3.X)
                {
                    p0 = new Point3d(p2.X + ycX, p2.Y, p2.Z);
                    p4 = new Point3d(p3.X - ycX, p3.Y, p3.Z);
                    p5 = new Point3d(p0.X, p0.Y - h1 - (pyY - 2100), p0.Z);
                    p7 = new Point3d(p4.X, p4.Y - (pyY - 2100), p4.Z);
                    p6 = new Point3d(p0.X - ycX + 2000, p0.Y - h1 - (pyY - 2100), p0.Z);
                    p8 = new Point3d(p4.X + ycX - 2000, p4.Y - (pyY - 2100), p4.Z);
                }
                pts.Add(p6); pts.Add(p5); pts.Add(p0); pts.Add(p2); pts.Add(p3);
                pts.Add(p4); pts.Add(p7); pts.Add(p8);
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                ObjectId clId = db.Clayer;
                LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                if (!lt.Has("参考线不打印"))
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = "参考线不打印";
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 195);
                    ltr.IsPlottable = false;

                    LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                    if (!ltt.Has("DASHED"))
                    {
                        ltt.UpgradeOpen();
                        db.LoadLineTypeFile("DASHED", "acadiso.lin");
                    }
                    if (ltt.Has("DASHED"))
                    {
                        ltr.LinetypeObjectId = ltt["DASHED"];
                    }
                    lt.UpgradeOpen();
                    lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                    ltr.DowngradeOpen();
                    lt.DowngradeOpen();
                }
                db.Clayer = lt["参考线不打印"];
                ObjectId objId = btr.AppendEntity(pl);
                trans.AddNewlyCreatedDBObject(pl, false);

                Move2(objId, new Point3d(0, 0, 0), new Point3d(0, -pyY, 0));
                db.Clayer = clId;
                trans.Commit();
            }

        }
        /// <summary>
        /// 删除参考线图层和实体
        /// </summary>
        [CommandMethod("dtckx")]
        public static void deleteCKX()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TypedValue[] typedValue = new TypedValue[] {
                    new TypedValue((int)DxfCode.LayerName,"参考线不打印")
                };
                SelectionFilter filter = new SelectionFilter(typedValue);
                PromptSelectionResult psr = doc.Editor.SelectAll(filter);
                SelectionSet ss = psr.Value;
                if (ss == null) return;
                doc.Editor.WriteMessage("参考线有:" + ss.Count.ToString() + ",");

                foreach (var id in psr.Value.GetObjectIds())
                {
                    Entity ent = id.GetObject(OpenMode.ForWrite) as Entity;
                    ent.Erase();
                }
                LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                if (lt.Has("参考线不打印"))
                {
                    LayerTableRecord ltr = lt["参考线不打印"].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.Erase();
                }
                trans.Commit();
            }
        }

        [CommandMethod("right")]
        public static void CheckRight()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Editor ed = doc.Editor;
                PromptPointOptions ppo = new PromptPointOptions("\n请选择位置");
                PromptPointResult ppr = ed.GetPoint(ppo);
                if (ppr.Status == PromptStatus.OK)
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    ObjectId clId = db.Clayer;
                    LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                    Point3d p0 = new Point3d(ppr.Value.X - 100, ppr.Value.Y, ppr.Value.Z);
                    Point3d p1 = new Point3d(ppr.Value.X, ppr.Value.Y - 100, ppr.Value.Z);
                    Point3d p2 = new Point3d(ppr.Value.X + 200, ppr.Value.Y + 100, ppr.Value.Z);
                    Point3dCollection pts = new Point3dCollection();
                    pts.Add(p0); pts.Add(p1); pts.Add(p2);
                    Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, false);

                    if (!lt.Has("参考线不打印"))
                    {
                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = "参考线不打印";
                        ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 195);
                        ltr.IsPlottable = false;

                        LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                        if (!ltt.Has("DASHED"))
                        {
                            ltt.UpgradeOpen();
                            db.LoadLineTypeFile("DASHED", "acadiso.lin");
                        }
                        if (ltt.Has("DASHED"))
                        {
                            ltr.LinetypeObjectId = ltt["DASHED"];
                        }
                        lt.UpgradeOpen();
                        lt.Add(ltr);
                        trans.AddNewlyCreatedDBObject(ltr, true);
                        ltr.DowngradeOpen();
                        lt.DowngradeOpen();
                    }

                    db.Clayer = lt["参考线不打印"];
                    ObjectId objId = btr.AppendEntity(pl);
                    trans.AddNewlyCreatedDBObject(pl, true);
                    db.Clayer = clId;

                }
                else if (ppr.Status == PromptStatus.Cancel)
                {

                    return;
                }
                trans.Commit();

            }
        }

        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="sourcePt"></param>
        /// <param name="targetPt"></param>
        /// <returns></returns>
        public static ObjectId Move2(ObjectId objectId, Point3d sourcePt, Point3d targetPt)
        {
            Vector3d vt = targetPt.GetVectorTo(sourcePt);
            Matrix3d mt = Matrix3d.Displacement(vt);
            Entity ent = objectId.GetObject(OpenMode.ForWrite) as Entity;
            // Line l = new Line(sourcePt, targetPt);
            // AddToCKXModelSpace(objectId.Database,l);
            Entity ent2 = ent.GetTransformedCopy(mt);
            //ent2.ColorIndex = 1;
            ObjectId objectId1 = AddToCKXModelSpace(objectId.Database, ent2);
            return objectId1;

        }
        /// <summary>
        /// 车位统计和刷新(两点式)
        /// </summary>
        [CommandMethod("cwtj")]
        public static void InitMyTable()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBDictionary dict = db.TableStyleDictionaryId.GetObject(OpenMode.ForRead) as DBDictionary;
                ObjectId objid = ObjectId.Null;
                if (dict.Contains("ColorTable"))
                {
                    objid = dict.GetAt("ColorTable");
                }
                else
                {
                    TableStyle ts = new TableStyle();
                    ts.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, 8), (int)RowType.TitleRow);//标题行,0行
                    //外边框
                    ts.SetGridLineWeight(LineWeight.LineWeight100, (int)GridLineType.OuterGridLines, 1);
                    ts.SetGridLineWeight(LineWeight.LineWeight100, (int)GridLineType.OuterGridLines, 2);
                    ts.SetGridLineWeight(LineWeight.LineWeight100, (int)GridLineType.OuterGridLines, 4);
                    ts.SetGridLineWeight(LineWeight.LineWeight100, (int)GridLineType.HorizontalBottom, (int)RowType.HeaderRow);
                    ts.SetGridLineWeight(LineWeight.LineWeight100, (int)GridLineType.HorizontalTop, (int)RowType.DataRow);
                    ts.SetTextHeight(500, 1);
                    ts.SetTextHeight(500, 4);
                    ts.SetTextHeight(800, 2);
                    ts.SetAlignment(CellAlignment.MiddleCenter, 1);
                    ts.SetAlignment(CellAlignment.MiddleCenter, 2);
                    ts.SetAlignment(CellAlignment.MiddleCenter, 4);
                    dict.UpgradeOpen();
                    objid = dict.SetAt("ColorTable", ts);
                    trans.AddNewlyCreatedDBObject(ts, true);
                }
                PromptPointResult ppr1 = ed.GetPoint("\n请输入第一点:");
                if (ppr1.Status != PromptStatus.OK) return;
                Point3d p1 = ppr1.Value;

                PromptCornerOptions pco = new PromptCornerOptions("\n请输入第二点:", p1);
                pco.UseDashedLine = true;
                PromptPointResult ppr2 = ed.GetCorner(pco);
                if (ppr2.Status != PromptStatus.OK) return;
                if (ppr2.Status == PromptStatus.OK)
                {
                    Point3dCollection pts = new Point3dCollection();
                    Point3d p2 = ppr2.Value;
                    pts.Add(p1); pts.Add(new Point3d(p1.X, p2.Y, 0)); pts.Add(p2); pts.Add(new Point3d(p2.X, p1.Y, 0));

                    Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                    db.AddToCKXModelSpace(pl);
                    PromptSelectionResult psr = ed.SelectCrossingPolygon(pts);
                    SelectionSet ss = psr.Value;
                    if (ss == null) { ed.WriteMessage("\n 该区域没有相关块,请重新操作!"); return; }
                    List<BlockReference> brs = new List<BlockReference>();
                    foreach (var id in ss.GetObjectIds())
                    {
                        Entity ent = id.GetObject(OpenMode.ForRead) as Entity;
                        if (ent.GetType() == typeof(BlockReference))
                        {
                            BlockReference br = (BlockReference)ent;
                            brs.Add(br);
                        }
                    }
                    if (brs.Count < 1) return;
                    var blocks = (from b in brs// db.GetEntsInDatabase<BlockReference>()
                                  where b.Name.IndexOf("车位") > -1
                                  group b by b.Name into g
                                  let id = g.First().BlockTableRecord
                                  orderby g.Count() descending
                                  select new { Name = g.Key, number = g.Count(), Id = id });


                    PromptPointResult pprc = ed.GetPoint("\n请选择表格插入点");
                    if (pprc.Status != PromptStatus.OK) return;
                    Table mytable = new Table();
                    mytable.Position = pprc.Value;
                    mytable.SetSize(blocks.Count() + 3, 4);
                    mytable.SetRowHeight(1000);
                    mytable.Columns[0].Width = 5000;
                    mytable.Columns[1].Width = 9000;
                    mytable.Columns[2].Width = 4000;
                    mytable.Columns[3].Width = 5000;

                    mytable.TableStyle = objid;// addTableStyle(db,"ColorTable");
                    mytable.Cells[0, 0].TextString = "块表统计结果";
                    mytable.Cells[1, 0].TextString = "序号";
                    mytable.Cells[1, 1].TextString = "名称";
                    mytable.Cells[1, 2].TextString = "数量";
                    mytable.Cells[1, 3].TextString = "缩略图";
                    int i = 2;
                    foreach (var block in blocks)
                    {
                        mytable.Cells[i, 0].TextString = (i - 1).ToString();
                        mytable.Cells[i, 1].TextString = block.Name;
                        mytable.Cells[i, 2].TextString = block.number.ToString();
                        mytable.Cells[i, 3].Contents.Add();
                        mytable.Cells[i, 3].Contents[0].BlockTableRecordId = block.Id;
                        i++;
                    }
                    mytable.Cells[i, 0].TextString = " 区域起始点:";
                    mytable.Cells[i, 1].TextString = p1.ToString();
                    mytable.Cells[i, 2].TextString = " 区域截止点:";
                    mytable.Cells[i, 3].TextString = p2.ToString();
                    AddToCKXModelSpace(db, mytable);
                    trans.Commit();
                }

            }
        }

        [CommandMethod("cwsx")]
        public static void RefreshTable()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TypedValue[] typedValue = new TypedValue[] {
                    new TypedValue((int)DxfCode.LayerName,"参考线不打印"),
                };
                SelectionFilter filter = new SelectionFilter(typedValue);
                PromptSelectionResult psr = ed.SelectAll(filter);
                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet ss = psr.Value;
                    foreach (var id in ss.GetObjectIds())
                    {
                        Entity ent1 = id.GetObject(OpenMode.ForWrite) as Entity;
                        Table myTable = new Table();
                        if (ent1.GetType() == typeof(Table))
                        {
                            myTable = (Table)ent1;
                        }
                        Object obj1 = myTable.Cells[myTable.Rows.Count - 1, 1].Value;
                        Object obj2 = myTable.Cells[myTable.Rows.Count - 1, 3].Value;
                        if (obj1 == null || obj2 == null) continue;
                        Point3d p1 = StringToPoint3d(obj1.ToString());
                        Point3d p2 = StringToPoint3d(obj2.ToString());
                        int Row = myTable.Rows.Count;
                        if (myTable.Rows.Count > 3)
                            myTable.DeleteRows(2, myTable.Rows.Count - 3);

                        if (myTable != null)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            pts.Add(p1); pts.Add(new Point3d(p1.X, p2.Y, 0)); pts.Add(p2); pts.Add(new Point3d(p2.X, p1.Y, 0));

                            PromptSelectionResult psr2 = ed.SelectCrossingWindow(p1, p2);
                            SelectionSet ss2 = psr2.Value;
                            if (ss2 == null) return;
                            List<BlockReference> brs = new List<BlockReference>();
                            foreach (var id2 in ss2.GetObjectIds())
                            {
                                Entity ent = id2.GetObject(OpenMode.ForRead) as Entity;
                                if (ent.GetType() == typeof(BlockReference))
                                {
                                    BlockReference br = (BlockReference)ent;
                                    brs.Add(br);
                                }
                            }
                            if (brs.Count < 1) continue;

                            var blocks = (from b in brs// db.GetEntsInDatabase<BlockReference>()
                                          where b.Name.IndexOf("车位") > -1
                                          group b by b.Name into g
                                          let id3 = g.First().BlockTableRecord
                                          orderby g.Count() descending
                                          select new { Name = g.Key, number = g.Count(), Id = id3 });
                            int i = 2;
                            myTable.SetSize(blocks.Count() + 3, 4);
                            foreach (var block in blocks)
                            {
                                myTable.Cells[i, 0].TextString = (i - 1).ToString();
                                myTable.Cells[i, 1].TextString = block.Name;
                                myTable.Cells[i, 2].TextString = block.number.ToString();
                                myTable.Cells[i, 3].Contents.Add();
                                myTable.Cells[i, 3].Contents[0].BlockTableRecordId = block.Id;
                                i++;
                            }
                            myTable.Cells[i, 0].TextString = " 区域起始点:";
                            myTable.Cells[i, 1].TextString = p1.ToString();
                            myTable.Cells[i, 2].TextString = " 区域截止点:";
                            myTable.Cells[i, 3].TextString = p2.ToString();
                        }
                    }
                }
                trans.Commit();
            }

        }

        public static ObjectId AddToCKXModelSpace(this Database db, Entity ent)
        {
            ObjectId objectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                using (BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                {
                    ObjectId clId = db.Clayer;
                    LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                    TextStyleTable tst = db.TextStyleTableId.GetObject(OpenMode.ForRead) as TextStyleTable;
                    //LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;


                    if (!lt.Has("参考线不打印"))
                    {
                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = "参考线不打印";
                        ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 195);
                        ltr.IsPlottable = false;
                        lt.UpgradeOpen();
                        lt.Add(ltr);
                        trans.AddNewlyCreatedDBObject(ltr, true);
                        ltr.DowngradeOpen();
                        lt.DowngradeOpen();
                    }


                    if (!tst.Has("属性"))
                    {
                        TextStyleTableRecord tstr = new TextStyleTableRecord();
                        tstr.Name = "属性";
                        tstr.FileName = "宋体";
                        tstr.TextSize = 300.0;
                        tst.UpgradeOpen();
                        tst.Add(tstr);
                        trans.AddNewlyCreatedDBObject(tstr, true);
                        tst.DowngradeOpen();
                    }


                    ent.Layer = "参考线不打印";
                    db.Clayer = lt["参考线不打印"];
                    db.Textstyle = tst["属性"];

                    objectId = btr.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);
                    db.Clayer = clId;
                }
                trans.Commit();
            }
            return objectId;
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


        /// <summary>
        /// string转换为ObjectId
        /// </summary>
        /// <param name="strId">string</param>
        /// <returns></returns>
        public static ObjectId StringToObjectId(string strId)
        {
            if (strId.IndexOf("(") < 0) return ObjectId.Null;
            strId = strId.Substring(1, strId.Length - 2);
            Console.WriteLine(strId);
            long intId = Convert.ToInt64(strId);//这里的strId是一个纯数字字符串，将其转换成64的long类型，32的会报错
            IntPtr init = new IntPtr(intId);//将long类型的intId转换成IntPtr类型的整数
            ObjectId obj = new Autodesk.AutoCAD.DatabaseServices.ObjectId(init);//在这就直接转化成ObjectId形式的数据了
            return obj;
        }

        public static Point3d StringToPoint3d(String p)
        {
            if (p == null) return new Point3d(0, 0, 0);
            else
            {
                p = p.Substring(1, p.Length - 2);
                String[] loc = p.Split(',');
                Double[] dloc = new double[3];
                dloc[0] = double.Parse(loc[0]);
                dloc[1] = double.Parse(loc[1]);
                dloc[2] = double.Parse(loc[2]);
                return new Point3d(dloc);
            }
        }


        [CommandMethod("polyjig")]
        public static void GetPolyline()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            Vector3d vt = mt.CoordinateSystem3d.Zaxis;
            PromptPointOptions ppo = new PromptPointOptions("\n 请指定起点");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            Point3d pcenter = ppr.Value;
            Point3dCollection pts = new Point3dCollection();
            pts.Add(pcenter);
            PolyJig polyJig = new PolyJig(pts, vt);

            for (; ; )
            {
                PromptResult pr = ed.Drag(polyJig);
                ed.WriteMessage("st=" + pr.Status);
                if (pr.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (pr.Status == PromptStatus.OK)
                {
                    AddToCKXModelSpace(db, polyJig.GetEntity());
                    break;
                }
            }
        }
        [CommandMethod("wall")]
        public static void mlineJig()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            Vector3d vt = mt.CoordinateSystem3d.Zaxis;
            double Top = 120.0, But = 120.0;
            PromptDoubleOptions pdoT = new PromptDoubleOptions("请输入左(上)宽:");
            pdoT.Keywords.Add("120");
            pdoT.DefaultValue = Top;
            PromptDoubleResult pdr = ed.GetDouble(pdoT);
            if (pdr.Status != PromptStatus.OK) return;
            else
                Top = pdr.Value;
            PromptDoubleOptions pdoB = new PromptDoubleOptions("请输入右(下)宽:");
            pdoB.Keywords.Add("120");
            pdoB.DefaultValue = But;
            PromptDoubleResult pdrB = ed.GetDouble(pdoB);
            if (pdrB.Status != PromptStatus.OK) return;
            else
                But = pdrB.Value;

            PromptPointOptions ppo = new PromptPointOptions("\n 请指定起点");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            Point3d pcenter = ppr.Value;
            createmlinestyle("WAll(" + Top + "_" + But + ")", Top, But);
            mline polyJig = new mline(pcenter, vt);

            for (; ; )
            {
                PromptResult pr = ed.Drag(polyJig);

                if (pr.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (pr.Status == PromptStatus.OK)
                {
                    AddToCKXModelSpace(db, polyJig.GetEntity());
                    break;
                }
            }
        }

        [CommandMethod("mlinedraw")]
        public static void AddMline()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction Tx = db.TransactionManager.StartTransaction())
            {
                createmlinestyle("Wall(120.0_120.0)", 120.0, 120.0);

                Mline line = new Mline();
                DBDictionary mlineDic = (DBDictionary)Tx.GetObject(db.MLStyleDictionaryId, OpenMode.ForRead);
                //createmlinestyle("mlineType");

                MlineStyle mlineStyle1 = new MlineStyle();
                if (mlineDic.Contains("mlineType"))
                {
                    mlineStyle1 = mlineDic["mlineType"] as MlineStyle;
                }
                if (mlineStyle1 == null)
                    line.Style = db.CmlstyleID;
                else
                    line.Style = mlineStyle1.ObjectId;
                line.Justification = MlineJustification.Zero;
                line.Normal = Vector3d.ZAxis;
                line.AppendSegment(new Point3d(0, 0, 0));
                line.AppendSegment(new Point3d(100, 100, 0));
                line.AppendSegment(new Point3d(200, 100, 0));
                line.AppendSegment(new Point3d(150, 150, 0));
                line.AppendSegment(new Point3d(150, 0, 0));
                // MlineStyle mls = new MlineStyle();
                // mls.s
                ObjectId ModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);
                BlockTableRecord model = Tx.GetObject(ModelSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                model.AppendEntity(line);
                Tx.AddNewlyCreatedDBObject(line, true);
                Tx.Commit();
            }
        }
        /// <summary>
        /// 多段线样式设置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="top">上部高度</param>
        /// <param name="but">下部高度</param>
        public static void createmlinestyle(String name, double top, double but)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;
            Database db = doc.Database;
            using (Transaction Tx = db.TransactionManager.StartTransaction())
            {
                DBDictionary mlineDic = (DBDictionary)Tx.GetObject(db.MLStyleDictionaryId, OpenMode.ForRead);
                if (!mlineDic.Contains(name))
                {
                    mlineDic.UpgradeOpen();
                    MlineStyle mlineStyle = new MlineStyle();
                    mlineStyle.EndAngle = 3.14159 * 0.5;//下面的线截止角度
                    mlineStyle.StartAngle = 3.14159 * 0.5;//上面的线截止角度
                    mlineStyle.StartSquareCap = true;//前部直线封闭
                    mlineStyle.EndSquareCap = true;//后部直线封闭

                    mlineStyle.Name = name;
                    /*
                    Color red = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    ObjectId ctId = LoadLineType(db, "DASHED");
                    mlineStyle.Elements.Add(new MlineStyleElement(0, red, ctId), true);
                     mlineStyle.StartRoundCap = false;//前部外弧
                    mlineStyle.ShowMiters = false;//连接线,中间和端点
                    mlineStyle.EndSquareCap = false;//后部外弧
                    mlineStyle.EndInnerArcs = false;//内弧
                    */
                    Color blue = Color.FromColorIndex(ColorMethod.ByAci, 5);
                    ObjectId hID = LoadLineType(db, "Continuous");
                    mlineStyle.Elements.Add(new MlineStyleElement(top, blue, hID), true);
                    mlineStyle.Elements.Add(new MlineStyleElement(-but, blue, hID), true);
                    db.CmlstyleID = mlineDic.SetAt(name, mlineStyle);
                    Tx.AddNewlyCreatedDBObject(mlineStyle, true);
                }
                else
                    db.CmlstyleID = (ObjectId)mlineDic[name];
                Tx.Commit();
            }
        }
        public static ObjectId LoadLineType(Database db, string typeName)
        {
            LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
            if (!ltt.Has(typeName))
            {
                db.LoadLineTypeFile(typeName, "acad.lin");
            }
            return ltt[typeName];
        }

        /// <summary>
        /// 统计喷头数量
        /// </summary>
        [CommandMethod("pt")]
        public static void GetPLT()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                PromptSelectionOptions pso = new PromptSelectionOptions();
                pso.MessageForAdding = "\n选择需要统计喷头的区域";
                TypedValue[] filter = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start,"LWPOLYLINE")
                };
                List<pointInPolyline> pips = new List<pointInPolyline>();
                SelectionFilter sFilter = new SelectionFilter(filter);
                PromptSelectionResult psr = ed.GetSelection(pso, sFilter);
                if (psr.Status != PromptStatus.OK) return;
                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet ss = psr.Value;
                    //DBObjectCollection pls = new DBObjectCollection();
                    foreach (var id in ss.GetObjectIds())
                    {
                        Polyline pl = id.GetObject(OpenMode.ForWrite) as Polyline;
                        pl.Closed = true;
                        //pls.Add(pl);
                        double x1, x2, y1, y2;
                        x1 = x2 = pl.GetPoint3dAt(0).X;
                        y1 = y2 = pl.GetPoint3dAt(0).Y;
                        Point3dCollection pts = new Point3dCollection();
                        Point3d ptlast = new Point3d(0, 0, 0);
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            if (!pts.Contains(pl.GetPoint3dAt(i)))
                                pts.Add(pl.GetPoint3dAt(i));
                            if (pl.GetPoint3dAt(i).X < x1)
                                x1 = pl.GetPoint3dAt(i).X;
                            if (pl.GetPoint3dAt(i).X > x2)
                                x2 = pl.GetPoint3dAt(i).X;
                            if (pl.GetPoint3dAt(i).Y < y1)
                                y1 = pl.GetPoint3dAt(i).Y;
                            if (pl.GetPoint3dAt(i).Y > y2)
                                y2 = pl.GetPoint3dAt(i).Y;
                        }
                        if (pl != null)
                        {
                            TypedValue[] filter2 = new TypedValue[] {
                                new TypedValue((int)DxfCode.LayerName,"EQUIP_喷淋")
                            };
                            SelectionFilter sFilter2 = new SelectionFilter(filter2);
                            PromptSelectionResult psr2 = ed.SelectCrossingPolygon(pts, sFilter2);
                            if (psr2.Status == PromptStatus.OK)
                            {
                                SelectionSet ss2 = psr2.Value;
                                /*
                                foreach (var id2 in ss2.GetObjectIds())
                                {
                                    Entity ent = id2.GetObject(OpenMode.ForWrite) as Entity;
                                    
                                    if (ent.ColorIndex > 255)
                                        ent.ColorIndex = 1;
                                    else
                                        ent.ColorIndex += 1;

                                    if (ent.ColorIndex > 1)
                                        ent.Highlight();
                                   
                                } */
                                DBText text = new DBText();
                                text.Position = new Point3d((x1 + x2) / 2, (y1 + y2) / 2, 0);
                                text.Height = 800;
                                if (ss2 == null)
                                {
                                    text.TextString = "该区域没有个喷头";
                                }
                                else
                                {
                                    text.TextString = "该区域有" + ss2.Count + "个喷头";
                                    pointInPolyline pip = new pointInPolyline();
                                    pip.objId = pl.ObjectId;
                                    pip.num = ss2.Count;
                                    pips.Add(pip);
                                }
                                text.HorizontalMode = TextHorizontalMode.TextCenter;
                                text.VerticalMode = TextVerticalMode.TextVerticalMid;
                                text.AlignmentPoint = text.Position;
                                db.AddToCKXModelSpace(text);
                            }
                            else
                            {
                                DBText text = new DBText();
                                text.Position = new Point3d((x1 + x2) / 2, (y1 + y2) / 2, 0);
                                text.Height = 800;
                                text.TextString = "该区域没有个喷头";
                                text.HorizontalMode = TextHorizontalMode.TextCenter;
                                text.VerticalMode = TextVerticalMode.TextVerticalMid;
                                text.AlignmentPoint = text.Position;
                                db.AddToCKXModelSpace(text);
                            }

                        }
                    }
                }
                trans.Commit();
            }
        }
        struct pointInPolyline
        {
            public ObjectId objId { get; set; }
            public int num { get; set; }
            public ObjectId parentId { get; set; }
        }
        /// <summary>
        /// 统计多段线面积,按图层分开
        /// </summary>
        [CommandMethod("areatj")]
        public static void getArea()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<Region> regs = new List<Region>();
                PromptSelectionOptions pso = new PromptSelectionOptions();
                pso.MessageForAdding = "\n选择需要统计面积的区域";
                TypedValue[] filter = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator,"<or"),
                    new TypedValue((int)DxfCode.Start,"LWPOLYLINE"),//指polyline
                    new TypedValue((int)DxfCode.Start,"MLINE"),
                    //new TypedValue((int)DxfCode.Start,"POLYLINE"),//指polyline3d
                    new TypedValue((int)DxfCode.Operator,"or>")
                };
                List<string> layers = new List<string>();
                List<PlineLayerArea> infs = new List<PlineLayerArea>();

                SelectionFilter sFilter = new SelectionFilter(filter);
                PromptSelectionResult psr = ed.GetSelection(pso, sFilter);
                if (psr.Status != PromptStatus.OK) return;
                if (psr.Status == PromptStatus.OK)
                {

                    SelectionSet ss = psr.Value;
                    DBObjectCollection dbObj = new DBObjectCollection();
                    foreach (var id in ss.GetObjectIds())
                    {
                        Entity pl = id.GetObject(OpenMode.ForWrite) as Entity;
                        dbObj.Add(pl);
                        if (!layers.Contains(pl.Layer))
                            layers.Add(pl.Layer);
                    }
                    foreach (var layer in layers)
                    {
                        DBObjectCollection plTmp = new DBObjectCollection();
                        foreach (Entity pl in dbObj)
                        {
                            if (pl.Layer == layer)
                            {
                                plTmp.Add(pl);
                            }
                        }
                        double area = GetAreaSum(plTmp, ref regs);
                        if (area > 0.0)
                        {
                            PlineLayerArea inf = new PlineLayerArea();
                            inf.layer = layer;
                            inf.Area = area;
                            infs.Add(inf);
                        }
                    }
                }
                // r1.BooleanOperation(BooleanOperationType.BoolUnite, r4);//去除重复的总面积
                // r2.BooleanOperation(BooleanOperationType.BoolIntersect, r5);//R2,R5的交面积
                // r3.BooleanOperation(BooleanOperationType.BoolSubtract, r6);//R3里面去除在R6的部分

                String areaInf = "";
                double areaSum = 0.0;
                foreach (PlineLayerArea inf in infs)
                {
                    areaInf += inf.layer + "图层面积为:" + inf.Area / 100000.0 + "㎡ ";
                    areaSum += inf.Area;
                }
                areaInf += "总面积是:" + areaSum / 100000.0 + "㎡";
                ed.WriteMessage(areaInf);
                if (areaSum == 0.0) return;
                PromptPointOptions ppo = new PromptPointOptions("\n请指定基点或退出");
                ppo.AllowNone = true;

                PromptPointResult pprc = ed.GetPoint(ppo);
                if (pprc.Status != PromptStatus.OK) return;
                else
                {
                    Point3d p = pprc.Value;
                    EntityChangeTools entChangeJig = new EntityChangeTools(regs, p, "Y");
                    PromptResult pr = ed.Drag(entChangeJig);
                    if (pr.Status != PromptStatus.OK) return;
                    else
                    {
                        foreach (Region reg in regs)
                            db.AddToCKXModelSpace(reg);
                    }
                }
                trans.Commit();
            }
        }
        struct PlineLayerArea
        {
            public string layer { get; set; }
            public double Area { get; set; }
        }
        public static double GetAreaSum(DBObjectCollection objs, ref List<Region> regs)
        {
            try
            {
                if (objs == null)
                {
                    return 0.0;
                }
                DBObjectCollection areaOBJ = Region.CreateFromCurves(objs);
                foreach (Region reg in areaOBJ)
                {
                    regs.Add(reg);
                }
                Region r0 = areaOBJ[0] as Region;
                if (areaOBJ.Count > 1)
                {
                    for (int i = 1; i < areaOBJ.Count; i++)
                    {
                        Region reg = areaOBJ[i] as Region;
                        r0.BooleanOperation(BooleanOperationType.BoolUnite, reg);
                    }
                }
                return r0.Area;
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 建筑按照模板图层拆分
        /// </summary>
        [CommandMethod("JZCF")]
        public static void GetExportEntity()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                String path = db.GetCustomProperties("ExportPath");
                Point3dCollection pts = new Point3dCollection();
                if (path.Length < 1)
                {
                    path = @"C:\Users\jdq\Desktop\Jo\1#";
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "请选择导出文件的路径";   // 设置窗体的标题
                    if (dialog.ShowDialog() == DialogResult.OK)   // 窗体打开成功
                    {
                        path = dialog.SelectedPath;  // 获取文件的路径
                    }
                }

                PromptPointResult ppr1 = ed.GetPoint("\n请输入第一点:");
                if (ppr1.Status != PromptStatus.OK) return;
                Point3d p1 = ppr1.Value;
                Point3d p2 = Point3d.Origin;
                PromptCornerOptions pco = new PromptCornerOptions("\n请输入第二点:", p1);
                pco.UseDashedLine = true;
                PromptPointResult ppr2 = ed.GetCorner(pco);
                if (ppr2.Status != PromptStatus.OK) return;
                else
                    p2 = ppr2.Value;
                Point3d p0 = new Point3d(p1.X, p2.Y, 0);
                Point3d p3 = new Point3d(p2.X, p1.Y, 0);
                pts.Add(p0); pts.Add(p1); pts.Add(p3); pts.Add(p2);
                Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                db.AddToCKXModelSpace(pl);
                String Folder;
                PromptStringOptions pso = new PromptStringOptions("请输入文件夹名称");
                PromptResult pr = ed.GetString(pso);
                if (pr.Status != PromptStatus.OK) return;
                else
                {
                    Folder = pr.StringResult;
                }
                db.SetCustomProperties("ExportPath", @path);
                db.SetCustomProperties(Folder, p1 + "," + p2);
                doc.ExportEntity(path, Folder, p1, p2);
                trans.Commit();
            }
        }
        /// <summary>
        /// 刷新拆分信息
        /// </summary>
        [CommandMethod("SXCF")]
        public static void RefreshExportEntity()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            string ExportPath = "";
            List<custProper> custPropers = new List<custProper>();
            var inf = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            if (inf.CustomPropertyTable == null) { ed.WriteMessage("没有记录,无法刷新,请先创建!"); return; }
            else
            {
                var table = inf.CustomPropertyTable;
                ed.WriteMessage("count=" + table.Count);
                ICollection keys = table.Keys;
                foreach (var key in keys)
                {
                    if (key.ToString() == "ExportPath")
                        ExportPath = table[key].ToString();
                    else
                    {
                        custProper cp = new custProper();
                        cp.key = key.ToString();
                        cp.value = table[key].ToString();
                        custPropers.Add(cp);
                    }
                }
                if (custPropers.Count < 1) { ed.WriteMessage("没有选择拆分区域,无法刷新!"); return; }
                if (ExportPath != "")
                {
                    foreach (var cp in custPropers)
                    {
                        String ps = cp.value;
                        String[] p = ps.Split(')');
                        String p1 = p[0].Substring(1, p[0].Length - 1);
                        String p2 = p[1].Substring(2);
                        Point3d pt1 = new Point3d(stringTodouble(p1));
                        Point3d pt2 = new Point3d(stringTodouble(p2));
                        //ed.WriteMessage("p1="+p1+",p2="+p2);
                        doc.ExportEntity(ExportPath, cp.key, pt1, pt2);
                    }
                }
            }


        }
        /// <summary>
        /// 两点间区域拆分
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="exportPath">导出的路径(大)</param>
        /// <param name="folder">当前的文件夹名称</param>
        /// <param name="p1">左点</param>
        /// <param name="p2">右点</param>
        public static void ExportEntity(this Document doc, string exportPath, string folder, Point3d p1, Point3d p2)
        {
            Editor ed = doc.Editor;
            Database db = doc.Database;
            /*
            string[] layerJ0 = new string[]
               { "Defpoints","J11-图框及其他信息|图框","J11-图框及其他信息|J文-引注、说明(提资隐藏、建筑打印)"
                ,"J11-图框及其他信息|PUB_TEXT","J11-图框及其他信息|J内框"};
            string[] layerJ1 = new string[]
                {"Defpoints","DIM_ELEV","PUB_TEXT","DIM_SYMB","J文-作法"
                ,"J文-空间名称(提资保留、建筑打印)","WALL","PUB_HATCH","阳台"
                ,"WINDOW","3T_WOOD","3T_BAR","WINDOW_TEXT","3T_GLASS"
                ,"STAIR-台阶","STAIR","J平-排水沟、分水线","J平-阳台、空调位、门窗套"
                ,"J通-栏杆、百叶、格栅、配件","J通-线角、成品构件","SURFACE"
                ,"J文 - (提资保留、建筑不打印)"," - TIANCHONG","EVTR"};
            string[] layerJ2 = new string[]
                { "Defpoints","AXIS","AXIS_TEXT","DOTE" };
            string[] layerJ3 = new string[]
                {"Defpoints","DIM_LEAD","PUB_TEXT","J文-引注、说明(提资隐藏、建筑打印)","J文-作法"
                ,"J平-户内设备配件","J文-(提资保留、建筑不打印)"," - TIANCHONG" };
            string[] layerJ4 = new string[] { };
            string[] layerJ5 = new string[]
                { "Defpoints","PUB_DIM","DIM_LEAD","PUB_TEXT" };
            string[] layerJ6 = new string[]
                { "Defpoints","DIM_IDEN","PUB_TEXT","J文-引注、说明(提资隐藏、建筑打印)"  };
            string[] layerJ7 = new string[] { };
            string[] layerJ8 = new string[] { };
            string[] layerJ9 = new string[] { };
            string[] layerJ10 = new string[] { };
            string[] layerJ11 = new string[]
                { "Defpoints","图框","J文-引注、说明(提资隐藏、建筑打印)","J内框","PUB_TEXT" };
            string[] layerJ12 = new string[]
                {  };
            string[] layerJ13 = new string[]
                {"Defpoints", "DIM_SYMB", "PUB_TEXT", "J平-板留洞、管井梁线"  };
            string[] layerJ14 = new string[] { "Defpoints" };
            */

            string[] layerJ0 = new string[]
                {};
            string[] layerJ1 = new string[]
                {"0","WALL","PUB_HATCH","STAIR","STAIR-台阶","ROOF","DIM_MODI"
                ,"WINDOW","DIM_SYMB","PUB_TEXT","J平-家具","J平-洁具、厨具"
                ,"J平-空调","J平-阳台、空调位、门窗套","J平-排水沟、分水线"
                ,"J平-范围线","J平-配件","J通-线角、成品构件","J通-空洞折线、玻璃线"
                ,"J通-栏杆、百叶、格栅、配件","J通-墙留洞","J文-空间名称(提资保留、建筑打印)"
                ,"J文-作法","J文-(提资保留、建筑不打印)","00","-TIANCHONG" };
            string[] layerJ2 = new string[]
                { "DOTE","AXIS","AXIS_TEXT","AXIS内-设备专业关闭"};
            string[] layerJ3 = new string[]
                { "DIM_LEAD","PUB_TEXT","J平-面积框(建筑不打印)","J平-户内设备配件"
                ,"J平-燃气热水器、燃气表","J文-作法","J文-引注、说明(提资隐藏、建筑打印)"
                ,"J文-表格(提资隐藏、建筑打印)"};
            string[] layerJ4 = new string[]
            { };//
            string[] layerJ5 = new string[]
                { "DIM_ELEV","PUB_DIM","PUB_TEXT","DIM_LEAD","J通-墙留洞"};
            string[] layerJ6 = new string[]
                {  "DIM_IDEN","PUB_TEXT","DIM_IDEN副","J6-大样索引","J6-大样索引建筑自用"};
            string[] layerJ7 = new string[]
            {  };
            string[] layerJ8 = new string[]
            { };
            string[] layerJ9 = new string[]
            { };
            string[] layerJ10 = new string[]
            { };
            string[] layerJ11 = new string[]
                {"TK","图框","J文-引注、说明(提资隐藏、建筑打印)","J文-作法"};
            string[] layerJ12 = new string[]
                { };
            string[] layerJ13 = new string[]
                {"J平-板留洞、管井梁线","J通-空洞折线、玻璃线","J平-板留洞投影线","DIM_SYMB","PUB_TEXT"};
            string[] layerJ14 = new string[] { };

            //建筑楼梯图
            string[] jzltxt = new string[]
            {
                "Defpoints","J1-建筑各层平面图|J文-空间名称(提资保留、建筑打印)","J1-建筑各层平面图|PUB_HATCH"
                ,"J1-建筑各层平面图|WALL","J1-建筑各层平面图|WINDOW","J1-建筑各层平面图|STAIR"
                ,"J1-建筑各层平面图|J通-栏杆、百叶、格栅、配件","J1-建筑各层平面图|J平-阳台、空调位、门窗套"
                ,"J1-建筑各层平面图|J通-线角、成品构件","J1-建筑各层平面图|J平-排水沟、分水线","PUB_DIM"
                ,"PUB_TEXT","DIM_SYMB","J通-空洞折线、玻璃线","J文-空间名称(提资保留、建筑打印)","PUB_HATCH"
                ,"WALL","WINDOW","STAIR","J平-洁具、厨具","J通-栏杆、百叶、格栅、配件","J平-板留洞、管井梁线"
                ,"J平-空调","J平-家具","J平-阳台、空调位、门窗套","J通-线角、成品构件","ROOF"
                ,"J平-排水沟、分水线","DIM_MODI","DOTE","AXIS","AXIS_TEXT","AXIS内-设备专业关闭","J文-作法"
                ,"J平-户内设备配件","J平-燃气热水器、燃气表","J文-引注、说明(提资隐藏、建筑打印)"
                ,"J通-墙留洞","DIM_LEAD","DIM_IDEN","J平-楼梯平面详图扶手(提资隐藏、建筑打印）"
                ,"DOTE(提资隐藏、建筑打印）","J标-楼梯剖面详图尺寸（提资隐藏、建筑打印)","DIM_ELEV"
                ,"J平 - 楼梯详图标高（提资隐藏，建筑打印）","J文 - (提资隐藏、建筑不打印)"
                ,"混凝土结构构件轮廓 | AXIS","混凝土结构构件轮廓 | TK","混凝土结构构件轮廓 | WALL"
                ,"混凝土结构构件轮廓 | STAIR","混凝土结构构件轮廓 | AXIS_DIM","混凝土结构构件轮廓|COLU"
                ,"混凝土结构构件轮廓|REIN","混凝土结构构件轮廓|DIM","混凝土结构构件轮廓|THIN"
                ,"混凝土结构构件轮廓|BASE","混凝土结构构件轮廓 | pile","混凝土结构构件轮廓 | HATCH"
                ,"混凝土结构构件轮廓 | BEAM","混凝土结构构件轮廓 | BASE_DIM","混凝土结构构件轮廓 | AXIS_NUM"
                ,"混凝土结构构件轮廓 | GZ","混凝土结构构件轮廓 | HATCH_降板","混凝土结构构件轮廓 | DIM_模板"
                ,"混凝土结构构件轮廓 | HATCH_X","混凝土结构构件轮廓 | DIM_截面","混凝土结构构件轮廓 | HATCH_S"
                ,"混凝土结构构件轮廓 | LJ","混凝土结构构件轮廓 | GZ_1","混凝土结构构件轮廓 | PILE_DIM"
                ,"混凝土结构构件轮廓 | Detail","混凝土结构构件轮廓 | HATCH_剖面","混凝土结构构件轮廓 | AXIS_次轴线"
                ,"混凝土结构构件轮廓 | HATCH_边线","混凝土结构构件轮廓 | TEXT","J1 - 建筑各层平面图 | J文 - 作法"
                ,"J1 - 建筑各层平面图 | J文 - (提资保留、建筑不打印)","J1 - 建筑各层平面图 | -TIANCHONG"
                ,"J1 - 建筑各层平面图 | SURFACE","J1 - 建筑各层平面图 | WINDOW_TEXT","J1 - 建筑各层平面图 | 3T_BAR"
                ,"J1 - 建筑各层平面图 | 3T_GLASS","J1 - 建筑各层平面图 | 3T_WOOD","J1 - 建筑各层平面图 | PUB_TEXT"
                ,"J11 - 图框及其他信息 | 图框","J11 - 图框及其他信息 | J文 - 引注、说明(提资隐藏、建筑打印)"
                ,"J11 - 图框及其他信息 | PUB_TEXT","混凝土结构构件轮廓|结构意见","混凝土结构构件轮廓|Hole"
                ,"混凝土结构构件轮廓|DIM_ELEV","混凝土结构构件轮廓|图框","混凝土结构构件轮廓 | RF_砼墙"
                ,"J1 - 建筑各层平面图 | DIM_SYMB","J13 - 结构板洞及其他 | J平 - 板留洞、管井梁线"
                ,"J13 - 结构板洞及其他 | DIM_SYMB","J13-结构板洞及其他|PUB_TEXT","J11-图框及其他信息|J内框"
                ,"结构建筑楼梯图|AXIS","结构建筑楼梯图|TK","结构建筑楼梯图|WALL","结构建筑楼梯图 | AXIS_TEXT"
                ,"结构建筑楼梯图 | STAIR","结构建筑楼梯图 | DIM_IDEN","结构建筑楼梯图 | AXIS_DIM"
                ,"结构建筑楼梯图 | COLU","结构建筑楼梯图 | REIN","结构建筑楼梯图 | DIM","结构建筑楼梯图 | THIN"
                ,"结构建筑楼梯图 | BASE","结构建筑楼梯图 | pile","结构建筑楼梯图 | HATCH","结构建筑楼梯图 | BEAM"
                ,"结构建筑楼梯图 | BASE_DIM","结构建筑楼梯图 | AXIS_NUM","结构建筑楼梯图 | GZ"
                ,"结构建筑楼梯图 | HATCH_降板","结构建筑楼梯图 | DIM_模板","结构建筑楼梯图 | HATCH_X"
                ,"结构建筑楼梯图 | DIM_截面","结构建筑楼梯图|HATCH_S","结构建筑楼梯图|0修定-结构","结构建筑楼梯图|LJ"
                ,"结构建筑楼梯图|GZ_1","结构建筑楼梯图|PILE_DIM","结构建筑楼梯图 | Detail","结构建筑楼梯图 | HATCH_剖面"
                ,"结构建筑楼梯图 | AXIS_次轴线","结构建筑楼梯图 | HATCH_边线","结构建筑楼梯图 | THIN_1"
                ,"结构建筑楼梯图 | TEXT","结构建筑楼梯图 | PILE_DASH","结构建筑楼梯图 | PILE_DE"
                ,"结构建筑楼梯图 | 外部参照","结构建筑楼梯图|21000","结构建筑楼梯图|结构意见","结构建筑楼梯图|Hole"
                ,"结构建筑楼梯图|DIM_ELEV","结构建筑楼梯图|图框","结构建筑楼梯图 | RF_砼墙"
                ,"J1 - 建筑各层平面图 | DIM_ELEV","J1 - 建筑各层平面图 | STAIR - 台阶"
                ,"J1 - 建筑各层平面图 | 阳台","J1 - 建筑各层平面图 | EVTR","统一图签 | ASHADE"
                ,"统一图签 | 图框","统一图签 | qm","统一图签 | J3 - 说明、图表$0$签名","统一图签 | 签名"
                ,"J通 - 粉刷线，立面分仓缝.材质分界线","材料一","J通 - 材料填充"," - TIANCHONG"
            };
            //建筑平面图
            string[] jzpm = new string[]
                { "DIM_LEAD","WINDOW_TEXT","PUB_DIM","PUB_TEXT","DIM_SYMB","电子签名","图框" };
            /*
            {"Defpoints","J文-引注、说明(提资隐藏、建筑打印)","G1-结构墙柱|HATCH","G1-结构墙柱|HATCH_X"
            ,"G2-结构构造柱|GZ","G3-结构梁板|THIN","G3-结构梁板|BEAM","G1-结构墙柱|WALL"
            ,"G3-结构梁板|HATCH_降板","G3-结构梁板|DIM_模板","G3-结构梁板|LJ","PUB_TEXT"
            ,"J内框","D1-电气提资1|TEL_TEXT","D1-电气提资1|WIRE-避雷1","D1-电气提资1|WIRE-接地1"
            ,"D1-电气提资1|WIRE-照明","D1-电气提资1|WIRE-应急","D1-电气提资1|WIRE-消防电话"
            ,"D1-电气提资1|CABLETRAY_FIRE","D1-电气提资1|CABLETRAY_HIDE","D1-电气提资1|CABLETRAY_DOTE"
            ,"D1-电气提资1|CABLETRAY","D1-电气提资1|WIRE-消防电源监控JK","D1-电气提资1|CABLETRAY_WEAK"
            ,"D1-电气提资1|WIRE-消防广播B","D1-电气提资1|WIRE-消防电话F2","D1-电气提资1|WIRE-信号线+电源线"
            ,"D1-电气提资1|WIRE-消防电话F1","D1-电气提资1|WIRE-消防手控线","D1-电气提资1|WIRE-消防防火门监控"
            ,"D1-电气提资1|E-电气提资-强电箱","D1-电气提资1|a-J平-户内设备配件","D1-电气提资1|E-电气提资-弱电箱"
            ,"D1-电气提资1|E-电气提资-地下室进出线","D1-电气提资1|E-电气提资-PUB_DIM","D1-电气提资1|PUB_TEXT"
            ,"D1-电气提资1|E-电气提资-DIM_LEAD","D1-电气提资1|E-电气提资-建筑开洞","D1-电气提资1|E-电气提资-电缆沟"
            ,"D1-电气提资1|E-电气提资-可视对讲机","D1-电气提资1|E-电气提资-结构开洞","D1-电气提资1|TEL_DIM"
            ,"D1-电气提资1|TEL_LEAD","D1-电气提资1|WIRE-避雷","D1-电气提资1|LWIRE","D1-电气提资1|WIRE-接地"
            ,"D1-电气提资1|SHTLN","N1-提资建筑结构|Hole","N1-提资建筑结构|DIM-AC","N1-提资建筑结构|JGHole"
            ,"S1-立管、地漏、消火栓、设备及基础提资|ASHADE","S1-立管、地漏、消火栓、设备及基础提资|A-雨水立管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-YYLINE","S1-立管、地漏、消火栓、设备及基础提资|a-LINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|A-给水立管","S1-立管、地漏、消火栓、设备及基础提资|A-污水立管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-YLINE","S1-立管、地漏、消火栓、设备及基础提资|a-PLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-GLINE","S1-立管、地漏、消火栓、设备及基础提资|a-XXLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-GGLINE","S1-立管、地漏、消火栓、设备及基础提资|a-ZLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-XHLINE","S1-立管、地漏、消火栓、设备及基础提资|a-PPLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|A-喷淋立管","S1-立管、地漏、消火栓、设备及基础提资|a-喷头"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-RLINE","S1-立管、地漏、消火栓、设备及基础提资|a-RRLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|A-热水立管","S1-立管、地漏、消火栓、设备及基础提资|a-ZZLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-楼面线","S1-立管、地漏、消火栓、设备及基础提资|a-RHLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-ZZLINEZ","S1-立管、地漏、消火栓、设备及基础提资|TWT_DIM"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-基础","S1-立管、地漏、消火栓、设备及基础提资|PIPE-给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-热给水","S1-立管、地漏、消火栓、设备及基础提资|PIPE-热回水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-污水","S1-立管、地漏、消火栓、设备及基础提资|PIPE-雨水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-消防","S1-立管、地漏、消火栓、设备及基础提资|PIPE-喷淋"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-给水","S1-立管、地漏、消火栓、设备及基础提资|DIM_给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|TWT_TEXT","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|DIM_热给水","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热回水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-污水","S1-立管、地漏、消火栓、设备及基础提资|DIM_污水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-雨水","S1-立管、地漏、消火栓、设备及基础提资|DIM_雨水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-消防","S1-立管、地漏、消火栓、设备及基础提资|DIM_消防"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-喷淋","S1-立管、地漏、消火栓、设备及基础提资|DIM_喷淋"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VALVE_喷淋","S1-立管、地漏、消火栓、设备及基础提资|EQUIP_地漏"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_雨水斗","S1-立管、地漏、消火栓、设备及基础提资|EQUIP_喷淋"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-给水中","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-给水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-消防中","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-消防高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-喷淋中","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-喷淋高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热给水中","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热给水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热回水中","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-热回水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-给水中","S1-立管、地漏、消火栓、设备及基础提资|PIPE-给水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-消防高","S1-立管、地漏、消火栓、设备及基础提资|PIPE-消防中"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-喷淋中","S1-立管、地漏、消火栓、设备及基础提资|PIPE-喷淋高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-热给水中","S1-立管、地漏、消火栓、设备及基础提资|PIPE-热给水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-热回水中","S1-立管、地漏、消火栓、设备及基础提资|PIPE-热回水高"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VALVE_给水","S1-立管、地漏、消火栓、设备及基础提资|VALVE_消防"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-给排水提资说明","S1-立管、地漏、消火栓、设备及基础提资|a-水专业喷淋套管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-水专业给排水套管","S1-立管、地漏、消火栓、设备及基础提资|a-潜污泵"
            ,"S1-立管、地漏、消火栓、设备及基础提资|TEXT_给水","S1-立管、地漏、消火栓、设备及基础提资|EQUIP_给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_消防","S1-立管、地漏、消火栓、设备及基础提资|TEXT_消防"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_喷头","S1-立管、地漏、消火栓、设备及基础提资|VALVE_热给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_热给水","S1-立管、地漏、消火栓、设备及基础提资|TEXT_热给水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_热回水","S1-立管、地漏、消火栓、设备及基础提资|VALVE_热回水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|TEXT_热回水","S1-立管、地漏、消火栓、设备及基础提资|TEXT_喷淋"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_污水","S1-立管、地漏、消火栓、设备及基础提资|TEXT_污水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_雨水","S1-立管、地漏、消火栓、设备及基础提资|TEXT_雨水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-废水","S1-立管、地漏、消火栓、设备及基础提资|A-热回水立管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|A-消火栓立管","S1-立管、地漏、消火栓、设备及基础提资|a-RHHLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-给排水云线","S1-立管、地漏、消火栓、设备及基础提资|EQUIP_消火栓"
            ,"S1-立管、地漏、消火栓、设备及基础提资|x-不提取","S1-立管、地漏、消火栓、设备及基础提资|a-WLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-废水","S1-立管、地漏、消火栓、设备及基础提资|DIM_废水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|A-废水立管","S1-立管、地漏、消火栓、设备及基础提资|a-WWLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-冷凝阳台水立管","S1-立管、地漏、消火栓、设备及基础提资|a-KYYLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-KNLINE","S1-立管、地漏、消火栓、设备及基础提资|PIPE-凝结"
            ,"S1-立管、地漏、消火栓、设备及基础提资|a-LQLINE","S1-立管、地漏、消火栓、设备及基础提资|a-LQQLINE"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-冷却","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-冷却"
            ,"S1-立管、地漏、消火栓、设备及基础提资|DIM_冷却","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-凝结"
            ,"S1-立管、地漏、消火栓、设备及基础提资|DIM_凝结","S1-立管、地漏、消火栓、设备及基础提资|A-冷却水立管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP_凝结","S1-立管、地漏、消火栓、设备及基础提资|TEXT_凝结"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VALVE_雨水","S1-立管、地漏、消火栓、设备及基础提资|EQUIP_冷却"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VALVE_冷却","S1-立管、地漏、消火栓、设备及基础提资|TEXT_冷却"
            ,"S1-立管、地漏、消火栓、设备及基础提资|-提资给电气（水不打印）","S1-立管、地漏、消火栓、设备及基础提资|-提资给电气（水打印）"
            ,"S1-立管、地漏、消火栓、设备及基础提资|-提资给土建（水打印）","S1-立管、地漏、消火栓、设备及基础提资|EQUIP-消防"
            ,"S1-立管、地漏、消火栓、设备及基础提资|EQUIP-照明","S1-立管、地漏、消火栓、设备及基础提资|-提资给土建（水不打印）"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-市政","S1-立管、地漏、消火栓、设备及基础提资|PIPE-直饮"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PIPE-热媒","S1-立管、地漏、消火栓、设备及基础提资|VPIPE-市政"
            ,"S1-立管、地漏、消火栓、设备及基础提资|DIM_市政","S1-立管、地漏、消火栓、设备及基础提资|DIM_热回水"
            ,"S1-立管、地漏、消火栓、设备及基础提资|VPIPE-直饮","S1-立管、地漏、消火栓、设备及基础提资|DIM_直饮"
            ,"S1-立管、地漏、消火栓、设备及基础提资|PUB_TEXT","S1-立管、地漏、消火栓、设备及基础提资|WALL"
            ,"S1-立管、地漏、消火栓、设备及基础提资|J文-引注、说明(提资隐藏、建筑打印)"
            ,"S1-立管、地漏、消火栓、设备及基础提资|J平-立管、雨水口图例","S1-立管、地漏、消火栓、设备及基础提资|ROOF"
            ,"S1-立管、地漏、消火栓、设备及基础提资|J标-尺寸(细部)","S1-立管、地漏、消火栓、设备及基础提资|a-消防立管"
            ,"S1-立管、地漏、消火栓、设备及基础提资|00","S1-立管、地漏、消火栓、设备及基础提资|0-雨水立管"
            ,"项目中心文件|J内框","项目中心文件|建设用地范围线","中心文件","PLAN_建筑附属轮廓","PLAN_阳台"
            ,"PLAN_建筑基底轮廓","PLAN_规划净用地","PLAN_规划总用地","PLAN_其他","PLAN_雨棚","PLAN_局部公建轮廓"
            ,"PLAN_楼顶间","PLAN_HATCH","D1-电气提资1|TEL_HATCH","统一图签|ASHADE","统一图签|图框","统一图签|qm"
            ,"统一图签|J3-说明、图表$0$签名","统一图签|签名","DIM_LEAD","WINDOW_TEXT","图框","电子签名"
            ,"DIM_SYMB","PUB_DIM","J文-作法","PUB_HATCH","J平-洁具、厨具","SURFACE","AXIS","DOTE","AXIS_TEXT"
            ,"DIM_IDEN副","J文-空间名称(提资保留、建筑打印)","WALL","WINDOW","STAIR","J通-栏杆、百叶、格栅、配件"
            ,"J平-阳台、空调位、门窗套","-TIANCHONG","3T_BAR","3T_GLASS","3T_WOOD","DIM_ELEV","J平-空调"
            ,"J文-(提资保留、建筑不打印)","J平-户内设备配件","J平-板留洞、管井梁线","J通-线角、成品构件"
            ,"J平-排水沟、分水线","STAIR-台阶","J通-空洞折线、玻璃线","J平-家具","ROOF","DIM_MODI"
            ,"AXIS内-设备专业关闭","J平-范围线","FURNITURE","J平-配件","J通-墙留洞","J平-燃气热水器、燃气表"
            ,"J平-面积框(建筑不打印)","J3-标记、说明层$0$J3-标记、说明层$0$DIM_IDEN","DIM_IDEN","J通-石材"
            ,"Z制图要求","_HATCH","_BALCONY","AE-FURN-MOVE","3","W11-给排水参照_地下一层给排水_SEN14FQW$0$VPIPE-给水"
            ,"W11-给排水参照_地下一层给排水_SEN14FQW$0$EQUIP_消火栓","L栏杆","K看线","J平-立管、雨水口图例"
            ,"J1-建筑各层平面图$0$ROOF","J1-建筑各层平面图$0$J通-金属"
            };
            */
            //建筑专项设计节点图
            string[] jzzx = new string[]
                {"Defpoints","混凝土结构构件轮廓|AXIS","混凝土结构构件轮廓|TK","混凝土结构构件轮廓|WALL"
                ,"混凝土结构构件轮廓|STAIR","混凝土结构构件轮廓|AXIS_DIM","混凝土结构构件轮廓|COLU"
                ,"混凝土结构构件轮廓|REIN","混凝土结构构件轮廓|DIM","混凝土结构构件轮廓|THIN"
                ,"混凝土结构构件轮廓|BASE","混凝土结构构件轮廓|pile","混凝土结构构件轮廓|HATCH"
                ,"混凝土结构构件轮廓|BEAM","混凝土结构构件轮廓|BASE_DIM","混凝土结构构件轮廓|AXIS_NUM"
                ,"混凝土结构构件轮廓|GZ","混凝土结构构件轮廓|HATCH_降板","混凝土结构构件轮廓|DIM_模板"
                ,"混凝土结构构件轮廓|HATCH_X","混凝土结构构件轮廓|DIM_截面","混凝土结构构件轮廓|HATCH_S"
                ,"混凝土结构构件轮廓|LJ","混凝土结构构件轮廓|GZ_1","混凝土结构构件轮廓|PILE_DIM"
                ,"混凝土结构构件轮廓|Detail","混凝土结构构件轮廓|HATCH_剖面","混凝土结构构件轮廓|AXIS_次轴线"
                ,"混凝土结构构件轮廓|HATCH_边线","混凝土结构构件轮廓|TEXT","混凝土结构构件轮廓|结构意见"
                ,"混凝土结构构件轮廓|Hole","混凝土结构构件轮廓|DIM_ELEV","混凝土结构构件轮廓|图框"
                ,"混凝土结构构件轮廓|RF_砼墙","统一图签|ASHADE","统一图签|图框","统一图签|qm"
                ,"统一图签|J3-说明、图表$0$签名","统一图签|签名","WINDOW","J通-材料填充"
                ,"J通-粉刷线","DOTE","PUB_DIM","J通-栏杆、百叶、格栅、配件","DIM_ELEV","PUB_TEXT"
                ,"J文-引注、说明(提资隐藏、建筑打印)","AXIS","AXIS_TEXT","DIM_SYMB","材料一","PUB_HATCH"
                ,"HATCH_边线","WALL","J通-粉刷线，立面分仓缝.材质分界线","DIM_IDEN","-TIANCHONG"
                ,"A-detl-XH","J通-其它材料","J标-尺寸(细部)","建筑信息" };
            //立剖面图
            string[] lpm = new string[]
                {"Defpoints","J通-栏杆、百叶、格栅、配件","J立-材料填充1","J立-门窗框线"
                ,"J立-材料填充2(瓦)","J立-材料填充4(备用)","J立-门窗洞口","J立-轮廓加粗","J立-材料填充3(涂)"
                ,"J立-立管","J通-空洞折线、玻璃线","J通-石材","-TIANCHONG","J文-空间名称(提资保留、建筑打印)"
                ,"DIM_ELEV","DOTE","AXIS","AXIS_TEXT","PUB_DIM","J文-引注、说明(提资隐藏、建筑打印)"
                ,"J通-材料1","统一图签|ASHADE","统一图签|图框","统一图签|qm","统一图签|J3-说明、图表$0$签名"
                ,"统一图签|签名","PUB_TEXT","Z制图要求","J立-轮廓不加粗","J立-分仓缝","_ARWALL-1"
                ,"_MPFIXTURE","_LINE" };
            //设计说明
            string[] sj = new string[]
                {"Defpoints","统一图签|ASHADE","统一图签|图框","统一图签|qm"
                ,"统一图签|J3-说明、图表$0$签名","统一图签|签名" };

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Point3dCollection pts = new Point3dCollection();
                Point3d p0 = new Point3d(p1.X, p2.Y, 0);
                Point3d p3 = new Point3d(p2.X, p1.Y, 0);
                pts.Add(p0); pts.Add(p1); pts.Add(p3); pts.Add(p2);

                PromptSelectionResult psr = ed.SelectCrossingPolygon(pts);
                if (psr.Status != PromptStatus.OK) return;
                SelectionSet ss = psr.Value;
                if (ss == null) { ed.WriteMessage("\n 该区域没有相关块,请重新操作!"); return; }

                if (ss.Count > 0)
                {

                    if (!Directory.Exists(exportPath + @"\" + folder))
                    {
                        ed.WriteMessage("文件夹不存在,重新创建:" + exportPath + @"\" + folder);
                        Directory.CreateDirectory(exportPath + @"\" + folder);
                    }

                    ObjectIdCollection selectIdsJ0 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ1 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ2 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ3 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ4 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ5 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ6 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ7 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ8 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ9 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ10 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ11 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ12 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ13 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJ14 = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJZltxt = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJZpm = new ObjectIdCollection();
                    ObjectIdCollection selectIdsJZzx = new ObjectIdCollection();
                    ObjectIdCollection selectIdsLpm = new ObjectIdCollection();
                    ObjectIdCollection selectIdsSj = new ObjectIdCollection();
                    List<ObjectIdCollection> ids = new List<ObjectIdCollection>();

                    LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                    if (!lt.Has("参考线不打印"))
                    {
                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = "参考线不打印";
                        ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 195);
                        ltr.IsPlottable = false;
                        lt.UpgradeOpen();
                        lt.Add(ltr);
                        trans.AddNewlyCreatedDBObject(ltr, true);
                        ltr.DowngradeOpen();
                        lt.DowngradeOpen();
                    }
                    foreach (var id in ss.GetObjectIds())
                    {
                        Entity ent = id.GetObject(OpenMode.ForRead) as Entity;
                        if (ent.Layer == null)
                            ent.LayerId = lt["参考线不打印"];
                        //ed.WriteMessage("type="+ent.GetType());
                        if (layerJ0.Contains(ent.Layer))
                            selectIdsJ0.Add(ent.ObjectId);
                        if (layerJ1.Contains(ent.Layer))
                            selectIdsJ1.Add(ent.ObjectId);
                        if (layerJ2.Contains(ent.Layer))
                            selectIdsJ2.Add(ent.ObjectId);
                        if (layerJ3.Contains(ent.Layer))
                            selectIdsJ3.Add(ent.ObjectId);
                        if (layerJ4.Contains(ent.Layer))
                            selectIdsJ4.Add(ent.ObjectId);
                        if (layerJ5.Contains(ent.Layer))
                            selectIdsJ5.Add(ent.ObjectId);
                        if (layerJ6.Contains(ent.Layer))
                            selectIdsJ6.Add(ent.ObjectId);
                        if (layerJ7.Contains(ent.Layer))
                            selectIdsJ7.Add(ent.ObjectId);
                        if (layerJ8.Contains(ent.Layer))
                            selectIdsJ8.Add(ent.ObjectId);
                        if (layerJ9.Contains(ent.Layer))
                            selectIdsJ9.Add(ent.ObjectId);
                        if (layerJ10.Contains(ent.Layer))
                            selectIdsJ10.Add(ent.ObjectId);
                        if (layerJ11.Contains(ent.Layer))
                            selectIdsJ11.Add(ent.ObjectId);
                        //if ( ent.Layer=="0" && ent.Layer=="00")
                        if (layerJ12.Contains(ent.Layer))
                            selectIdsJ12.Add(ent.ObjectId);
                        if (layerJ13.Contains(ent.Layer))
                            selectIdsJ13.Add(ent.ObjectId);
                        if (layerJ14.Contains(ent.Layer))
                            selectIdsJ14.Add(ent.ObjectId);
                        if (jzltxt.Contains(ent.Layer))
                            selectIdsJZltxt.Add(ent.ObjectId);
                        if (jzpm.Contains(ent.Layer))
                            selectIdsJZpm.Add(ent.ObjectId);
                        if (jzzx.Contains(ent.Layer))
                            selectIdsJZzx.Add(ent.ObjectId);
                        if (lpm.Contains(ent.Layer))
                            selectIdsLpm.Add(ent.ObjectId);
                        if (sj.Contains(ent.Layer))
                            selectIdsSj.Add(ent.ObjectId);

                    }
                    ids.Add(selectIdsJ0); ids.Add(selectIdsJ1); ids.Add(selectIdsJ2);
                    ids.Add(selectIdsJ3); ids.Add(selectIdsJ4); ids.Add(selectIdsJ5);
                    ids.Add(selectIdsJ6); ids.Add(selectIdsJ7); ids.Add(selectIdsJ8);
                    ids.Add(selectIdsJ9); ids.Add(selectIdsJ10); ids.Add(selectIdsJ11);
                    ids.Add(selectIdsJ12); ids.Add(selectIdsJ13); ids.Add(selectIdsJ14);
                    ids.Add(selectIdsJZltxt); ids.Add(selectIdsJZpm); ids.Add(selectIdsJZzx);
                    ids.Add(selectIdsLpm); ids.Add(selectIdsSj);

                    for (int i = 0; i < ids.Count; i++)
                    {
                        String FilePath = exportPath + @"\" + folder + @"\";
                        if (ids[i].Count < 1) continue;
                        switch (i)
                        {
                            case 0:
                                FilePath += "J0-条件图.dwg"; break;
                            case 1:
                                FilePath += "J1-建筑各层平面图.dwg"; break;
                            case 2:
                                FilePath += "J2-轴线轴号.dwg"; break;
                            case 3:
                                FilePath += "J3-标记、说明层.dwg"; break;
                            case 5:
                                FilePath += "J5-孔洞.dwg"; break;
                            case 6:
                                FilePath += "J6-大样索引.dwg"; break;
                            case 11:
                                FilePath += "J11-图框及其他信息.dwg"; break;
                            case 13:
                                FilePath += "J13-结构板洞及其他.dwg"; break;
                            case 14:
                                FilePath += "J14-一层平面图.dwg"; break;
                            case 15:
                                FilePath += "建筑楼梯详图.dwg"; break;
                            case 16:
                                FilePath += "建筑平面成图.dwg"; break;
                            case 17:
                                FilePath += "建筑专项设计节点图.dwg"; break;
                            case 18:
                                FilePath += "立剖面图.dwg"; break;
                            case 19:
                                FilePath += "设计说明.dwg"; break;
                            default:
                                FilePath += "J" + i + ".dwg"; break;
                        }
                        Database db2 = new Database(false, true);
                        try
                        {
                            db2 = db.Wblock(ids[i], db.Ucsorg);
                            db2.SaveAs(FilePath, DwgVersion.Current);
                            ed.WriteMessage("\n导出成功:" + FilePath + ",items=" + ids[i].Count);
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception e)
                        {
                            ed.WriteMessage("\ni=" + i + "," + e);
                        }


                    }
                }
            }
        }
        public static void SetCustomProperties(this Database db, string key, string value)
        {
            var inf = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            //如有则删除旧值
            if (inf.CustomPropertyTable.Contains(key))
                inf.CustomPropertyTable[key] = value;
            else
                inf.CustomPropertyTable.Add(key, value);
            db.SummaryInfo = inf.ToDatabaseSummaryInfo();
        }
        public static String GetCustomProperties(this Database db, string key)
        {
            var inf = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            if (inf.CustomPropertyTable[key] != null)
            {
                return inf.CustomPropertyTable[key].ToString();
            }
            else
                return "";

        }
        public struct custProper
        {
            public string key { get; set; }
            public string value { get; set; }
        }
        public static double[] stringTodouble(string point)
        {
            double[] pts = new double[3] { 0, 0, 0 };
            string[] pt = point.Split(',');
            for (int i = 0; i < 3; i++)
            {
                if (pt[i] != null)
                    pts[i] = double.Parse(pt[i]);
            }
            return pts;
        }

        /// <summary>
        /// 图纸截取
        /// </summary>
        [CommandMethod("jzss")]
        public static void GetScreenShot()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                String path = db.GetCustomProperties("ExportPath");
                Point3dCollection pts = new Point3dCollection();
                if (path.Length < 1)
                {
                    path = @"C:\Users\jdq\Desktop\Jo\1#";
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "请选择导出截图文件的路径";   // 设置窗体的标题
                    if (dialog.ShowDialog() == DialogResult.OK)   // 窗体打开成功
                    {
                        path = dialog.SelectedPath;  // 获取文件的路径
                    }
                }

                PromptPointResult ppr1 = ed.GetPoint("\n请输入第一点:");
                if (ppr1.Status != PromptStatus.OK) return;
                Point3d p1 = ppr1.Value;
                Point3d p2 = Point3d.Origin;
                PromptCornerOptions pco = new PromptCornerOptions("\n请输入第二点:", p1);
                pco.UseDashedLine = true;
                PromptPointResult ppr2 = ed.GetCorner(pco);
                if (ppr2.Status != PromptStatus.OK) return;
                else
                    p2 = ppr2.Value;
                Point3d p0 = new Point3d(p1.X, p2.Y, 0);
                Point3d p3 = new Point3d(p2.X, p1.Y, 0);
                pts.Add(p0); pts.Add(p1); pts.Add(p3); pts.Add(p2);
                Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                db.AddToCKXModelSpace(pl);
                String Folder;
                PromptStringOptions pso = new PromptStringOptions("\n请输入接图文件名称:");
                PromptResult pr = ed.GetString(pso);
                if (pr.Status != PromptStatus.OK) return;
                else
                {
                    Folder = pr.StringResult;
                }
                db.SetCustomProperties("ExportPath", @path);
                db.SetCustomProperties(Folder, p1 + "," + p2);
                doc.ScreenShot(path, Folder, p1, p2);
                trans.Commit();
            }
        }
        /// <summary>
        /// 刷新图纸截取信息
        /// </summary>
        [CommandMethod("SXss")]
        public static void RefreshScreenShot()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            string ExportPath = "";
            List<custProper> custPropers = new List<custProper>();
            var inf = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            if (inf.CustomPropertyTable == null) { ed.WriteMessage("\n没有记录,无法刷新,请先创建!"); return; }
            else
            {
                var table = inf.CustomPropertyTable;
                ed.WriteMessage("count=" + table.Count);
                ICollection keys = table.Keys;
                foreach (var key in keys)
                {
                    if (key.ToString() == "ExportPath")
                        ExportPath = table[key].ToString();
                    else
                    {
                        custProper cp = new custProper();
                        cp.key = key.ToString();
                        cp.value = table[key].ToString();
                        custPropers.Add(cp);
                    }
                }
                if (custPropers.Count < 1) { ed.WriteMessage("\n没有选择接图区域,无法刷新!"); return; }
                if (ExportPath != "")
                {
                    foreach (var cp in custPropers)
                    {
                        String ps = cp.value;
                        String[] p = ps.Split(')');
                        String p1 = p[0].Substring(1, p[0].Length - 1);
                        String p2 = p[1].Substring(2);
                        Point3d pt1 = new Point3d(stringTodouble(p1));
                        Point3d pt2 = new Point3d(stringTodouble(p2));
                        //ed.WriteMessage("p1="+p1+",p2="+p2);
                        doc.ScreenShot(ExportPath, cp.key, pt1, pt2);
                    }
                }
            }


        }
        public static void ScreenShot(this Document doc, string exportPath, string fileName, Point3d p1, Point3d p2)
        {
            String filePath = exportPath + @"\" + fileName + ".dwg";
            Point3dCollection pts = new Point3dCollection();
            Point3d p0 = new Point3d(p1.X, p2.Y, 0);
            Point3d p3 = new Point3d(p2.X, p1.Y, 0);
            pts.Add(p0); pts.Add(p1); pts.Add(p3); pts.Add(p2);
            Editor ed = doc.Editor;
            Database db = doc.Database;
            Point2dCollection pts2 = Pts3DTo2D(pts);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                PromptSelectionResult psr = ed.SelectCrossingPolygon(pts);
                if (psr.Status != PromptStatus.OK) return;
                else
                {
                    String Name = "区域截取" + DateTime.Now.ToString("HH_mm_ss");
                    ObjectId objectId = ObjectId.Null;
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = new BlockTableRecord();
                    btr.Name = Name;
                    SelectionSet ss = psr.Value;
                    //LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (var id in ss.GetObjectIds())
                    {
                        Entity ent = id.GetObject(OpenMode.ForRead) as Entity;
                        Entity entC = ent.Clone() as Entity;
                        /* if (lt.Has(ent.Layer))
                         {
                             LayerTableRecord ltr = lt[ent.Layer].GetObject(OpenMode.ForRead) as LayerTableRecord;
                             //if (!ltr.IsFrozen && !ltr.IsLocked)
                                 btr.AppendEntity(entC);
                         }*/
                        btr.AppendEntity(entC);
                    }
                    if (btr == null) return;
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    trans.AddNewlyCreatedDBObject(btr, true);
                    BlockReference br = new BlockReference(new Point3d(0, 0, 0), bt[Name]);
                    objectId = db.AddToCKXModelSpace(br);
                    ObjectId newId = db.clipBlockTest(objectId, pts2);
                    try
                    {
                        Database db2 = new Database(false, true);
                        ObjectIdCollection obj = new ObjectIdCollection();
                        obj.Add(objectId);
                        db2 = db.Wblock(obj, new Point3d(0, 0, 0));
                        Transaction tr = db2.TransactionManager.StartTransaction();
                        using (tr)
                        {
                            BlockTable bt2 = (BlockTable)tr.GetObject(db2.BlockTableId, OpenMode.ForRead);
                            BlockTableRecord btr2 = (BlockTableRecord)tr.GetObject(bt2[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                            foreach (ObjectId objId in btr2)
                            {
                                Entity ent = (Entity)tr.GetObject(objId, OpenMode.ForRead);
                                db2.clipBlockTest(ent.ObjectId, pts2);
                            }
                        }
                        db2.SaveAs(@filePath, DwgVersion.Current);
                        ed.WriteMessage("\n导出" + filePath + " 成功!");
                    }
                    catch
                    {
                        ed.WriteMessage("\n导出" + filePath + " 失败!");
                    }

                    br.Erase();
                    btr.Erase();
                    bt.DowngradeOpen();
                }
                trans.Commit();
            }
        }
        public static Point2dCollection Pts3DTo2D(Point3dCollection pts)
        {
            Point2dCollection pts2d = new Point2dCollection();
            for (int i = 0; i < pts.Count; i++)
            {
                pts2d.Add(new Point2d(pts[i].X, pts[i].Y));
            }
            return pts2d;
        }


        /// <summary>
        /// 刷新同名块属性
        /// </summary>
        [CommandMethod("sxk")]
        public static void ReGenBlock()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                bool flag = true;
                string brName = "";
                AttributeCollection atts = null;
                do
                {
                    PromptEntityOptions peo = new PromptEntityOptions("\n请选择参考块:");
                    PromptEntityResult per = ed.GetEntity(peo);
                    if (per.Status != PromptStatus.OK) return;
                    else
                    {
                        Entity ent = per.ObjectId.GetObject(OpenMode.ForRead) as Entity;
                        if (ent.GetType() == typeof(BlockReference))
                        {
                            BlockReference br = ent as BlockReference;
                            Point3d x = br.Position;
                            brName = br.Name;
                            flag = false;
                            atts = br.AttributeCollection;
                        }
                    }
                } while (flag);
                List<AttributeReference> ars = new List<AttributeReference>();
                foreach (ObjectId att in atts)
                {
                    AttributeReference ar = att.GetObject(OpenMode.ForRead) as AttributeReference;
                    ars.Add(ar);
                    ed.WriteMessage("ar v=" + ar.TextString);
                }
                PromptSelectionOptions pso = new PromptSelectionOptions();
                pso.MessageForAdding = "\n请选择要刷新的块:";
                TypedValue[] filter = new TypedValue[] { new TypedValue((int)DxfCode.BlockName, brName) };
                SelectionFilter sFilter = new SelectionFilter(filter);
                PromptSelectionResult psr = ed.GetSelection(pso, sFilter);
                if (psr.Status != PromptStatus.OK) return;
                else
                {
                    SelectionSet ss = psr.Value;
                    foreach (var id in ss.GetObjectIds())
                    {
                        BlockReference brTmp = id.GetObject(OpenMode.ForWrite) as BlockReference;
                        Point3d x = brTmp.Position;
                        foreach (ObjectId bratt in brTmp.AttributeCollection)
                        {
                            AttributeReference ar = bratt.GetObject(OpenMode.ForWrite) as AttributeReference;
                            foreach (var arT in ars)
                            {
                                if (ar.Tag == arT.Tag)
                                {
                                    ar.TextString = arT.TextString;
                                }
                            }
                        }
                    }
                }
                trans.Commit();
            }
        }

        [CommandMethod("bhk")]
        /// <summary>
        /// 块自动编号
        /// </summary>
        public static void AutoNumBlock()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                string start;
                PromptResult pr = ed.GetString("\n请输入起始车位编号:");
                if (pr.Status != PromptStatus.OK) return;
                else
                    start = pr.StringResult;
                int num = 1;
                int size = 1;
                string pre = "", last = "";
                if (Regex.IsMatch(start, @"^\d*$") || start.Length == 0)
                {
                    num = int.Parse(start);
                }
                else
                {
                    string st = Regex.Replace(start, @"[^0-9]+", "");
                    num = int.Parse(st);
                    size = st.Length;
                    pre = start.Substring(0, start.IndexOf(st));
                    last = start.Substring(pre.Length + st.Length);
                    ed.WriteMessage("pre=" + pre + ",num=" + st + ",last=" + last);
                }

                PromptSelectionOptions pso = new PromptSelectionOptions();
                pso.MessageForAdding = "\n请选择要标号的车位:";
                TypedValue[] filter = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Operator,"<or"),
                    new TypedValue((int)DxfCode.BlockName, "cw"),
                    new TypedValue((int)DxfCode.BlockName, "A$C68CD7CA3"),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    // new TypedValue((int)DxfCode.Start, "BlockReference")
                };
                SelectionFilter sFilter = new SelectionFilter(filter);
                PromptSelectionResult psr = ed.GetSelection(pso, sFilter);
                if (psr.Status != PromptStatus.OK) return;
                else
                {
                    SelectionSet ss = psr.Value;
                    List<PointAndAtt> p2as = new List<PointAndAtt>();
                    foreach (var id in ss.GetObjectIds())
                    {
                        BlockReference brTmp = id.GetObject(OpenMode.ForWrite) as BlockReference;
                        PointAndAtt paa = new PointAndAtt();
                        paa.point = brTmp.Position;
                        paa.atts = brTmp.AttributeCollection;
                        p2as.Add(paa);
                        /*
                        foreach (ObjectId bratt in brTmp.AttributeCollection)
                        {
                            AttributeReference ar = bratt.GetObject(OpenMode.ForWrite) as AttributeReference;
                            foreach (var arT in ars)
                            {
                                if (ar.Tag == arT.Tag)
                                {
                                    ar.TextString = arT.TextString;
                                }
                            }
                        }*/
                    }

                    var inf = (from q in p2as orderby (int)q.point.Y, (int)q.point.X select q).ToList();
                    for (int i = 0; i < p2as.Count; i++)
                    {
                        foreach (ObjectId bratt in inf[i].atts)
                        {
                            AttributeReference ar = bratt.GetObject(OpenMode.ForWrite) as AttributeReference;
                            if (ar.Tag == "NUMBER")
                            {
                                string cnum = num + "";
                                if (size - cnum.Length == 1)
                                    ar.TextString = pre + "0" + num + last;
                                else if (size - cnum.Length == 2)
                                    ar.TextString = pre + "00" + num + last;
                                else if (size - cnum.Length == 3)
                                    ar.TextString = pre + "000" + num + last;
                                else if (size <= cnum.Length)
                                    ar.TextString = pre + num + last;
                                num++;
                                ed.WriteMessage("\nnum=" + num + inf[i].point);
                            }
                        }
                    }
                }
                trans.Commit();
            }
        }
        struct PointAndAtt
        {
            public Point3d point { get; set; }
            public AttributeCollection atts { get; set; }
        }

        [CommandMethod("test")]
        public static void test()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            /*
            Line l = new Line(Point3d.Origin, new Point3d(100, 100, 0));
            Point3dCollection pts = new Point3dCollection();
            pts.Add(new Point3d(20, 20, 0));
            DBObjectCollection objs =l.GetSplitCurves(pts);
            //db.AddToCKXModelSpace(l);
            foreach (Entity obj in objs)
            {
                db.AddToCKXModelSpace(obj);
            }
            db.SetCustomProperties();
            */
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                /*//查询图纸出现的图层
                LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                if (lt != null)
                {
                    String layerString = "";
                    foreach (var layer in lt)
                    {
                        LayerTableRecord ltr = layer.GetObject(OpenMode.ForRead) as LayerTableRecord;
                        layerString += "\"" + ltr.Name + "\",";
                    }
                    ed.WriteMessage("layer=" + layerString); ;
                }*/

                double ltWidth = 260.0, ltHeight = 0.0;
                PromptDoubleOptions pdo = new PromptDoubleOptions("\n请输入每个楼梯宽:");
                pdo.Keywords.Add("260", "260mm", "260mm");
                pdo.DefaultValue = 260.0;

                PromptDoubleResult pdr = ed.GetDouble(pdo);
                if (pdr.Status != PromptStatus.OK) return;
                else
                    ltWidth = pdr.Value;
                PromptDoubleOptions pdo2 = new PromptDoubleOptions("\n请输入每个楼梯高度:");
                pdo2.Keywords.Add("175", "175mm", "175mm");
                pdo2.DefaultValue = 175.0;
                PromptDoubleResult pdr2 = ed.GetDouble(pdo2);
                if (pdr2.Status != PromptStatus.OK) return;
                else
                    ltHeight = pdr2.Value;
                Point3dCollection pts = new Point3dCollection();
                PromptPointResult ppr1 = ed.GetPoint("\n请输入第一点:");
                if (ppr1.Status != PromptStatus.OK) return;
                Point3d p1 = ppr1.Value;
                /*
                LT lt = new LT(new Point2d(p1.X, p1.Y), ltWidth, ltHeight);
                PromptResult pr = ed.Drag(lt);
                if (pr.Status != PromptStatus.OK && pr.Status != PromptStatus.Keyword) return;
                else if (pr.Status == PromptStatus.OK)
                {
                    db.AddToModelSpace(lt.GetEntity(), "STAIR");
                }*/


                ElRecJig jig = new ElRecJig(p1);
                PromptResult pr = ed.Drag(jig);
                if (pr.Status != PromptStatus.OK) return;
                else
                {
                    db.AddToModelSpace(jig.mypl, "STAIR");
                    //db.AddToModelSpace(jig.myline, "STAIR") ;
                }
                trans.Commit();
            }

        }
        [CommandMethod("opm")]
        public static void Initialize()
        {
            CustomProp custProp = null;
            Assembly.LoadFrom("asdkOPMNetExt64.dll");
            // Add the Dynamic Property
            Dictionary classDict = SystemObjects.ClassDictionary;
            RXClass lineDesc = (RXClass)classDict.At("AcDbLine");
            IPropertyManager2 pPropMan = (IPropertyManager2)xOPM.xGET_OPMPROPERTY_MANAGER(lineDesc);
            custProp = new CustomProp();
            pPropMan.AddProperty((object)custProp);
        }


        /// <summary>
        /// 图纸的图形特性
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static void SetCustomProperties(this Database db)
        {
            if (db.SummaryInfo.Author != null) return;
            var inf = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            inf.Author = "贾大齐";
            inf.Comments = "图形特性备注";
            inf.HyperlinkBase = "超链接 http://www.arnova.com.cn/";
            inf.Keywords = "关键字";
            inf.LastSavedBy = "最后修改人";
            inf.RevisionNumber = "V1.0";
            inf.Subject = "项目";
            inf.Title = "图形特性";
            inf.CustomPropertyTable.Add("自定义", "值");
            inf.CustomPropertyTable.Add("语言", "中文");
            db.SummaryInfo = inf.ToDatabaseSummaryInfo();
        }

        /// <summary>
        /// 框选区域
        /// </summary>
        const string filterDictName = "ACAD_FILTER";
        const string spatialName = "SPATIAL";
        public static ObjectId clipBlockTest(this Database db, ObjectId brId, Point2dCollection pts)
        {
            ObjectId objId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference br = (BlockReference)brId.GetObject(OpenMode.ForRead);
                SpatialFilterDefinition sfd = new SpatialFilterDefinition(pts, Vector3d.ZAxis, 0.0, 0.0, 0.0, true);
                SpatialFilter sf = new SpatialFilter();
                sf.Definition = sfd;
                if (br.ExtensionDictionary == ObjectId.Null)
                {
                    br.UpgradeOpen();
                    br.CreateExtensionDictionary();
                    br.DowngradeOpen();
                }
                DBDictionary xDict = (DBDictionary)tr.GetObject(br.ExtensionDictionary, OpenMode.ForWrite);
                if (xDict.Contains(filterDictName))
                {
                    DBDictionary fDict = (DBDictionary)tr.GetObject(xDict.GetAt(filterDictName), OpenMode.ForWrite);
                    if (fDict.Contains(spatialName))
                        fDict.Remove(spatialName);
                    objId = fDict.SetAt(spatialName, sf);
                }
                else
                {
                    DBDictionary fDict = new DBDictionary();
                    xDict.SetAt(filterDictName, fDict);
                    tr.AddNewlyCreatedDBObject(fDict, true);
                    objId = fDict.SetAt(spatialName, sf);
                }
                tr.AddNewlyCreatedDBObject(sf, true);
                tr.Commit();
            }
            return objId;
        }






        [CommandMethod("stair")]
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

                List<myStair> stairs = new List<myStair>();
                StairForm stairForm = new StairForm();
                Application.ShowModalDialog(stairForm);
                if (stairForm.DialogResult == DialogResult.OK)
                {
                    stairForm.ShowInTaskbar = false;
                    stairs = stairForm.res;
                }
                var sortStairs = (from q in stairs orderby q.Num ascending select q).ToList();
                int start = sortStairs[0].Num;
                bool first = true;
                foreach (var stair in sortStairs)
                {
                    if (start == stair.Num)
                    {
                        if (!first)
                        {
                            Vector3d vt2 = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                            point += vt2;
                        }
                        first = false;
                        db.DrawStair(point, stair.FloorWidth, stair.FloorHeight / 2, stair.LtW, stair.LtH,stair.LTN, stair.ExtW,stair.ExtW2, stair.ExtH, -stair.Cover, stair.SW, stair.Sh);
                    }
                    start++;
                    Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, stair.FloorHeight / 2, 0));
                    point += vt;
                }
                /*
                Vector3d vt = Point3d.Origin.GetVectorTo(new Point3d(0, 2929.45, 0));
                db.DrawStair(ppr.Value, 2340, 2929.45/2, 260, 175, 3150, 120, -4,400,200);
                Vector3d vt2 = Point3d.Origin.GetVectorTo(new Point3d(0, 2950, 0));
                db.DrawStair(ppr.Value +  (vt+vt2)/2, 2280, 1475, 260, 175, 1550, 120, -3, 400, 200);
                db.DrawStair(ppr.Value + (vt + vt2) / 2 + vt2, 2280, 1475, 260, 175, 1550, 120, -2, 400, 200);
                db.DrawStair(ppr.Value + (vt + vt2) / 2 + 2 * vt2, 2280, 1475, 260, 175, 1550, 120, -8, 400, 200);
                db.DrawStair(ppr.Value + (vt + vt2) / 2 + 3 * vt2, 2280, 1475, 260, 175, 1550, 120, -5, 400, 200);
            */
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
        public static void DrawStair(this Database db, Point3d point, double FloorWidth, double FloorHeight, double LtW, double LtH,double LtN, double ExtW, double ExtW2, double ExtH, double Cover, double SW, double Sh)
        {
            Point2dCollection pts = new Point2dCollection(), pts_1 = new Point2dCollection(),
                pts2 = new Point2dCollection();

            double num = Math.Ceiling(FloorWidth / LtW);
            LtH = FloorHeight / num;
            Point2d p0 = new Point2d(point.X - ExtW, point.Y);
            Point2d p1 = new Point2d(point.X, point.Y);
            //pts.Add(p0);
            pts.Add(p1);
            Point2d ptl = Point2d.Origin;
            for (int i = 1; i <= num; i++)
            {
                pts.Add(new Point2d(p1.X + (i - 1) * LtW, p1.Y + i * LtH));
                if (i != num)
                {
                    ptl = new Point2d(p1.X + i * LtW, p1.Y + i * LtH);
                    pts.Add(ptl);
                }
                else
                {
                    ptl = new Point2d(p1.X + (i - 1) * LtW, p1.Y + i * LtH);
                }
            }




            Point2d p3 = new Point2d(ptl.X + ExtW, ptl.Y);
            Point2d p4 = new Point2d(p3.X, p3.Y - ExtH);
            Point2d p5 = new Point2d(p4.X - ExtW, p4.Y);
            Point2d p6 = new Point2d(p1.X, p1.Y - ExtH);
            Point2d p7 = new Point2d(p0.X, p0.Y - ExtH);
            //pts.Add(p3); pts.Add(p4); pts.Add(p5); pts.Add(p6); pts.Add(p7);
            pts_1.Add(p1); pts_1.Add(p0); pts_1.Add(new Point2d(p0.X, p0.Y - ExtH));
            pts_1.Add(new Point2d(p1.X - SW, p1.Y - ExtH)); pts_1.Add(new Point2d(p1.X - SW, p1.Y - Sh)); pts_1.Add(new Point2d(p1.X, p1.Y - Sh));
            pts_1.Add(new Point2d(p1.X, p1.Y - LtH)); pts_1.Add(new Point2d(ptl.X, ptl.Y - LtH - ExtH));
            pts_1.Add(new Point2d(ptl.X, ptl.Y - Sh)); pts_1.Add(new Point2d(ptl.X + SW, ptl.Y - Sh)); pts_1.Add(new Point2d(ptl.X + SW, ptl.Y - ExtH));
            pts_1.Add(new Point2d(ptl.X + ExtW - SW, ptl.Y - ExtH)); pts_1.Add(new Point2d(ptl.X + ExtW - SW, ptl.Y - Sh)); pts_1.Add(new Point2d(ptl.X + ExtW, ptl.Y - Sh));
            pts_1.Add(new Point2d(ptl.X + ExtW, ptl.Y)); pts_1.Add(ptl);

            pts2.Add(p1);
            for (int i = 1; i < num; i++)
            {
                pts2.Add(new Point2d(p1.X + (i - 1) * LtW, p1.Y - i * LtH));
                ptl = new Point2d(p1.X + i * LtW, p1.Y - i * LtH);
                pts2.Add(ptl);
            }
            Point2d p2_1 = new Point2d(ptl.X, ptl.Y - LtH);
            pts2.Add(p2_1);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Polyline pl = new Polyline();
                //pl.Closed = true;
                for (int i = 0; i < pts.Count; i++)
                {
                    pl.AddVertexAt(i, pts[i], 0, 0, 0);
                }
                Polyline pl_1 = new Polyline();
                for (int i = 0; i < pts_1.Count; i++)
                {
                    pl_1.AddVertexAt(i, pts_1[i], 0, 0, 0);
                }
               
                if (Cover != 0)
                {
                    Polyline cpl1 = new Polyline();
                    DBObjectCollection dbo = pl.GetOffsetCurves(Cover);
                    if (dbo.Count > 0)
                    {
                        cpl1 = dbo[0] as Polyline;
                        cpl1.AddVertexAt(0, pl.GetPoint2dAt(0), 0, 0, 0);
                        cpl1.AddVertexAt(cpl1.NumberOfVertices, pl.GetPoint2dAt(pl.NumberOfVertices - 1), 0, 0, 0);
                    }
                    db.AddToModelSpace(cpl1, "STAIR");
                }
                //cpl1.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);

                Polyline pl2 = new Polyline();
                pl2.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                for (int i = 0; i < pts2.Count; i++)
                {
                    pl2.AddVertexAt(i, pts2[i], 0, 0, 0);
                }

                Line line = new Line(new Point3d(p1.X, p1.Y - ExtH - LtH, 0), new Point3d(p2_1.X, p2_1.Y - ExtH, 0));
                LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                if (ltt.Has("DASH"))
                {
                    line.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                    line.LinetypeId = ltt["DASH"];
                }
                
                if (Cover != 0)
                {
                    Polyline cpl2 = new Polyline();
                    cpl2.Color = Color.FromColorIndex(ColorMethod.ByBlock, 0);
                    DBObjectCollection dbo = pl2.GetOffsetCurves(Cover);
                    if (dbo.Count > 0)
                    {
                        cpl2 = dbo[0] as Polyline;
                        cpl2.AddVertexAt(0, pl2.GetPoint2dAt(0), 0, 0, 0);
                        cpl2.AddVertexAt(cpl2.NumberOfVertices, pl2.GetPoint2dAt(pl2.NumberOfVertices - 1), 0, 0, 0);
                    }

                    db.AddToModelSpace(cpl2, "STAIR");
                }
               
                db.AddToModelSpace(pl, "STAIR");
                db.AddToModelSpace(pl_1, "STAIR");
                db.AddToModelSpace(pl2, "STAIR");
                db.AddToModelSpace(line, "STAIR");


                //pl2.JoinEntity(cpl2);
                trans.Commit();
            }
        }




    }
}
