using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ckx
{
    public static class main
    {


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


        [CommandMethod("getPolyline")]
        public static void getPolyline()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTableRecord ltrs = db.Clayer.GetObject(OpenMode.ForRead) as LayerTableRecord;
                doc.Editor.WriteMessage("图层:" + ltrs.Name);

                TypedValue[] typedValue = new TypedValue[] {
                    new TypedValue((int)DxfCode.LayerName,ltrs.Name),
                    new TypedValue((int)DxfCode.Start,"Polyline")
                };
                SelectionFilter filter = new SelectionFilter(typedValue);
                PromptSelectionResult psr = doc.Editor.SelectAll(filter);
                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet ss = psr.Value;
                    doc.Editor.WriteMessage("参考线有:" + ss.Count + ",");
                }

                trans.Commit();

            }
        }

        [CommandMethod("getLine")]
        public static void GetEntity()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TypedValue[] filter = new TypedValue[] {
               // new TypedValue((int)DxfCode.Operator,"<or"),
                //new TypedValue((int)DxfCode.Start,"Line"),
                new TypedValue((int)DxfCode.Start,"LWPOLYLINE"),//二维多断线用LWPOLYLINE,3d的才用polyline
               // new TypedValue((int)DxfCode.Operator,"or>")
                };

                SelectionFilter sFilter = new SelectionFilter(filter);
                PromptSelectionResult psr = ed.GetSelection(sFilter);
                if (psr.Status != PromptStatus.OK) { ed.WriteMessage("psr.Status=" + psr.Status); }
                int count = 0;
                if (psr == null)
                    return;
                SelectionSet ss = psr.Value;

                PromptPointOptions ppo = new PromptPointOptions("请选择偏移方向:");
                PromptPointResult ppr = ed.GetPoint(ppo);
                Point3d pe = new Point3d(0, 0, 0);
                if (ppr.Status != PromptStatus.OK) { ed.WriteMessage("psr.Status=" + ppr.Status); return; }
                if (ppr.Status == PromptStatus.OK)
                {
                    pe = new Point3d(ppr.Value.X, ppr.Value.Y, ppr.Value.Z);
                }
                List<Entity> ents = new List<Entity>();
                foreach (var objId in ss.GetObjectIds())
                {

                    Polyline pl = objId.GetObject(OpenMode.ForRead) as Polyline;
                    Point3d ps = new Point3d(0, 0, 0);

                    Vector3d vt = ps.GetVectorTo(pe);
                    Matrix3d mt = Matrix3d.Displacement(vt);
                    Entity ent = pl.GetTransformedCopy(mt);
                    ents.Add(ent);
                    db.AddToModelSpace(ent);
                    count++;
                }

                doc.SendStringToExecute("_select " + ss[0].ObjectId + " ", true, false, false);
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
            // AddToModelSpace(objectId.Database,l);
            Entity ent2 = ent.GetTransformedCopy(mt);
            //ent2.ColorIndex = 1;
            ObjectId objectId1 = AddToModelSpace(objectId.Database, ent2);
            return objectId1;

        }



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
                /*
             PromptSelectionOptions pso = new PromptSelectionOptions();
             pso.MessageForAdding = "\n选择需要统计车库的区域";
             PromptSelectionResult psr = ed.GetSelection(pso);
             if (psr.Status != PromptStatus.OK) return;
                
                PromptEntityResult per = ed.GetEntity("\n请选择多段线:");
                if (per.Status != PromptStatus.OK) return;
                Polyline pl = trans.GetObject(per.ObjectId,OpenMode.ForRead) as Polyline;
                  if (pl!=null)
                {
                    Point3dCollection pts = new Point3dCollection();
                    for (int num = 0; num < pl.NumberOfVertices; num++)
                    {
                        pts.Add(pl.GetPoint3dAt(num));
                    }
                */

                PromptPointResult ppr1 = ed.GetPoint("\n请输入第一点:");
                if (ppr1.Status != PromptStatus.OK) return;
                Point3d p1 = ppr1.Value;

                PromptPointResult ppr2 = ed.GetPoint("\n请输入第二点:");
                if (ppr2.Status != PromptStatus.OK) return;
                if (ppr2.Status == PromptStatus.OK)
                {
                    Point3dCollection pts = new Point3dCollection();
                    Point3d p2 = ppr2.Value;
                    pts.Add(p1); pts.Add(new Point3d(p1.X, p2.Y, 0)); pts.Add(p2); pts.Add(new Point3d(p2.X, p1.Y, 0));

                    Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                    db.AddToModelSpace(pl);
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
                                  where (b.Name.IndexOf("车位") > -1 || b.Name.IndexOf("A$C") > -1)
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
                    AddToModelSpace(db, mytable);
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
                        if(myTable.Rows.Count>3)
                            myTable.DeleteRows(2, myTable.Rows.Count-3);
                        
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
                                          where (b.Name.IndexOf("车位") > -1 || b.Name.IndexOf("A$C") > -1)
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

        public static ObjectId AddToModelSpace(this Database db, Entity ent)
        {
            ObjectId objectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                using (BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                {
                    ObjectId clId = db.Clayer;
                    LayerTable lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
                    if (!lt.Has("参考线不打印"))
                    {
                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = "参考线不打印";
                        ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 195);
                        ltr.IsPlottable = false;

                        LinetypeTable ltt = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
                        /*
                        if (!ltt.Has("DASHED"))
                        {
                            ltt.UpgradeOpen();
                            db.LoadLineTypeFile("DASHED", "acadiso.lin");
                        }
                        if (ltt.Has("DASHED"))
                        {
                            ltr.LinetypeObjectId = ltt["DASHED"];
                        }
                        */

                        lt.UpgradeOpen();
                        lt.Add(ltr);
                        trans.AddNewlyCreatedDBObject(ltr, true);
                        ltr.DowngradeOpen();
                        lt.DowngradeOpen();
                    }
                    ent.Layer = "参考线不打印";
                    db.Clayer = lt["参考线不打印"];

                    objectId = btr.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);
                    db.Clayer = clId;
                }
                trans.Commit();
            }
            return objectId;
        }

        /// <summary>
        /// 实体打包到块
        /// </summary>
        /// <param name="db"></param>
        /// <param name="BlockName">块名</param>
        /// <param name="ents">实体集</param>
        /// <returns></returns>
        public static ObjectId AddBlockTableRecord(this Database db, string BlockName, List<Entity> ents)
        {
            BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            if (!bt.Has(BlockName))
            {
                BlockTableRecord btr = new BlockTableRecord();
                btr.Name = BlockName;
                ents.ForEach(ent => btr.AppendEntity(ent));
                bt.UpgradeOpen();
                bt.Add(btr);
                db.TransactionManager.AddNewlyCreatedDBObject(btr, true);
            }
            return bt[BlockName];
        }
        /// <summary>
        /// 实体打包到块2
        /// </summary>
        /// <param name="db"></param>
        /// <param name="BlockName">块名</param>
        /// <param name="ents">实体</param>
        /// <returns></returns>
        //public static ObjectId AddBlockTableRecord(this Database db, string BlockName, params Entity[] ents)
        //{
        //    return AddBlockTableRecord(db,BlockName,ents.ToList());
        //}

        /// <summary>
        /// 插入块参照
        /// </summary>
        /// <param name="objectId">插入对象空间 db.currentSpaceId</param>
        /// <param name="Layer">对象图层</param>
        /// <param name="block">块名</param>
        /// <param name="position">插入点</param>
        /// <param name="scale">放大倍数</param>
        /// <param name="rotataAng">选择角度</param>
        /// <returns></returns>
        public static ObjectId InsertBlock(this ObjectId objectId, string Layer, string block, Point3d position, Scale3d scale, double rotataAng)
        {
            ObjectId blockId;
            Database db = objectId.Database;
            BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            if (!bt.Has(block)) return ObjectId.Null;
            BlockTableRecord btr = objectId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
            BlockReference br = new BlockReference(position, bt[block]);
            br.ScaleFactors = scale;
            try
            {
                br.Layer = Layer;
            }
            catch
            {
                br.Layer = "0";
            }

            br.Rotation = rotataAng;
            blockId = btr.AppendEntity(br);
            db.TransactionManager.AddNewlyCreatedDBObject(br, true);
            btr.DowngradeOpen();
            return blockId;

        }

        [CommandMethod("insertblock")]
        public static void InsertBlockItem()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId objId = db.CurrentSpaceId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                objId.InsertBlock("参考线不打印", "test", Point3d.Origin, new Scale3d(2), 0);
                trans.Commit();
            }
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


        [CommandMethod("polydraw")]
        public static void GetSelectPolyline()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            Vector3d vt = mt.CoordinateSystem3d.Zaxis;
            PromptPointOptions ppo = new PromptPointOptions("\n 请指定起点");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            Point3d pcenter = ppr.Value;
            Polyline pl = new Polyline();
            pl.AddVertexAt(0, new Point2d(pcenter.X, pcenter.Y), 0, 0, 0);
            bool flag = true;
            while (flag)
            {
                PromptPointOptions ppo2 = new PromptPointOptions("\n 请指定下一点");
                ppo2.Keywords.Add("o");
                ppo2.Keywords.Default = "o";
                ppo2.UseBasePoint = true;
                ppo2.BasePoint = pcenter;
                PromptPointResult ppr2 = ed.GetPoint(ppo2);
                if (ppr2.Status != PromptStatus.OK || ppr2.StringResult == "o")
                {
                    ed.WriteMessage("s=" + ppr2.Status);
                    flag = false;
                    break;
                }
                Point3d p2 = ppr2.Value;
                pcenter = p2;
                pl.AddVertexAt(pl.NumberOfVertices, new Point2d(p2.X, p2.Y), 0, 0, 0);
            }



            AddToModelSpace(db, pl);
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
                    AddToModelSpace(db, polyJig.GetEntity());
                    break;
                }
            }

           
        }
        [CommandMethod("mlinejig")]
        public static void mlineJig()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            Vector3d vt = mt.CoordinateSystem3d.Zaxis;
            PromptPointOptions ppo = new PromptPointOptions("\n 请指定起点");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;
            Point3d pcenter = new Point3d(0, 0,0);//ppr.Value;
            mline polyJig = new mline(pcenter, vt);

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
                    AddToModelSpace(db, polyJig.GetEntity());
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
                Mline line = new Mline();
                line.Style = db.CmlstyleID;
                line.Normal = Vector3d.ZAxis;
                line.AppendSegment(new Point3d(0, 0, 0));
                line.AppendSegment(new Point3d(10, 10, 0));
                line.AppendSegment(new Point3d(20, 10, 0));
                line.AppendSegment(new Point3d(15, 15, 0));
                line.AppendSegment(new Point3d(15, 0, 0));
                // MlineStyle mls = new MlineStyle();
                // mls.s
                ObjectId ModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);
                BlockTableRecord model = Tx.GetObject(ModelSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                model.AppendEntity(line);
                Tx.AddNewlyCreatedDBObject(line, true);
                Tx.Commit();
            }
        }



        
    }
}
