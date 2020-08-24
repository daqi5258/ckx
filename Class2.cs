using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace ckx
{

    class asd
    {
        public static void SolidJig()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = ed.Document.Database;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    JigPline jig = new JigPline();
                    Point3dCollection pts;
                    PromptResult res = jig.DragMe(out pts);
                    if (res.Status == PromptStatus.OK)
                    {
                        BlockTableRecord btr = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                        Polyline3d pLine = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                        DBObjectCollection dbObj = new DBObjectCollection();
                        dbObj.Add(pLine);
                        Region region = Region.CreateFromCurves(dbObj)[0] as Region;
                        btr.AppendEntity(region);
                        tr.AddNewlyCreatedDBObject(region, true);
                        Solid3d solid = new Solid3d();
                        btr.AppendEntity(solid);
                        tr.AddNewlyCreatedDBObject(solid, true);
                        JigSolid solJig = new JigSolid(solid, ed.CurrentUserCoordinateSystem, region, pts[0]);
                        btr.AppendEntity(pLine);
                        tr.AddNewlyCreatedDBObject(pLine, true);
                        res = ed.Drag(solJig);
                        switch (res.Status)
                        {
                            case PromptStatus.OK:

                                break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch { }
        }

    }
    public class JigPline : DrawJig
    {
        public Point3d startPt;
        public Point3dCollection pts;
        public PromptResult DragMe(out Point3dCollection o_pnts)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            while (true)
            {
                PromptResult jigRes = ed.Drag(this);
                if (jigRes.Status != PromptStatus.Keyword)
                {
                    o_pnts = pts;
                    return jigRes;
                }
            }
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions pointOpts = new JigPromptPointOptions();
            pointOpts.Keywords.Add("Insertion");
            pointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.NullResponseAccepted);
            pointOpts.Message = "\nSelect start point";
            PromptPointResult jigRes = prompts.AcquirePoint(pointOpts);
            Point3d pt = jigRes.Value;
            if (pt == startPt)
                return SamplerStatus.NoChange;
            startPt = pt;
            if (jigRes.Status == PromptStatus.OK || jigRes.Status == PromptStatus.Keyword)
                return SamplerStatus.OK;
            return SamplerStatus.Cancel;
        }
        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            pts = new Point3dCollection();
            pts.Add(startPt + new Vector3d(-3, 0, 0));
            pts.Add(startPt + new Vector3d(3, 0, 0));
            pts.Add(startPt + new Vector3d(3, 0, 6));
            pts.Add(startPt + new Vector3d(2.5, 0, 6));
            pts.Add(startPt + new Vector3d(2.5, 0, 1));
            pts.Add(startPt + new Vector3d(-2.5, 0, 1));
            pts.Add(startPt + new Vector3d(-2.5, 0, 6));
            pts.Add(startPt + new Vector3d(-3, 0, 6));
            pts.Add(startPt + new Vector3d(-3, 0, 0));
            return draw.Geometry.Polygon(pts);
        }
    }

    public class JigSolid : EntityJig
    {
        Matrix3d _ucs;
        Point3d _pt;
        Point3d _endPt;
        double _dist;
        Region _reg;
        Solid3d _sol;
        SweepOptions _swpOpts;
        Polyline _pLine = new Polyline();

        public JigSolid(Solid3d sol, Matrix3d ucs, Region region, Point3d insertionPt) : base(region)
        {
            _ucs = ucs;
            _pt = insertionPt;
            _reg = region;
            _sol = sol;
            _swpOpts = new SweepOptions();
            _pLine = new Polyline();
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions ptOpts = new JigPromptPointOptions();
            ptOpts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.NoZeroResponseAccepted
                | UserInputControls.GovernedByOrthoMode | UserInputControls.NoNegativeResponseAccepted);
            ptOpts.UseBasePoint = true;
            ptOpts.BasePoint = _pt;
            ptOpts.Cursor = CursorType.RubberBand;
            ptOpts.Message = "\nSpecify distance :";
            PromptPointResult ppr = prompts.AcquirePoint(ptOpts);
            if (ppr.Status == PromptStatus.OK)
            {
                Point3d tmp = ppr.Value.TransformBy(_ucs.Inverse());
                if (tmp.DistanceTo(_pt) < Tolerance.Global.EqualPoint)
                    return SamplerStatus.NoChange;
            }
            _dist = _pt.DistanceTo(new Point3d(_pt.X, ppr.Value.Y, _pt.Z));
            _endPt = ppr.Value;
            return SamplerStatus.OK;
        }
        protected override bool Update()
        {
            try
            {
                //_sol.ExtrudeAlongPath(_reg, new Line(_pt, new Point3d(_pt.X, _endPt.Y + 20, _pt.Z))  , 0);
                //_sol.Extrude(_reg, _dist, 0);
                _sol.CreateExtrudedSolid(_reg, _pt.GetVectorTo(_endPt), _swpOpts);
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }


}
