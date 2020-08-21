using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
[assembly: ExtensionApplication(typeof(ckx.RightCheck))]
[assembly: CommandClass(typeof(ckx.main))]
namespace ckx {
    class RightCheck : IExtensionApplication {
        private static bool RightStatus = false;
        private int num = 3;
        [DllImport("acad.exe", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static private int ads_queueexpr(string strExpr);
        public void Initialize()
        {

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            string hostName = computerProperties.HostName;
            string domainName = computerProperties.DomainName;
            if (domainName == "hc.com")
            {
                RightStatus = true;
            }
            if (!RightStatus)
            {
                Thread thread = new Thread(et);
                thread.Start();
                // thread.Join();
            }
            else
                main.RightsCheck();
            throw new NotImplementedException();
        }
        public void et()
        {
            try
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("警告,非法使用和创财产,程序退出!") ;
            }
            catch
            {
                Thread.Sleep(1000*60);
            }
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("警告,非法使用和创财产,程序退出!");
            if(num==0)
                Application.Quit();
            //Application.DocumentManager.MdiActiveDocument.SendStringToExecute("._quit", true, false, false);
            Thread.Sleep(1000);
        }
        public void Terminate()
        {
            Console.WriteLine(RightStatus);

            throw new NotImplementedException();
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
    }
}
