using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;

namespace ckx
{
    class EntityChangeTools : DrawJig
    { 
        public Entity[] selectEntity;
        public Matrix3d mtBack = Matrix3d.Identity;
        public String Matrix3dType;
        public Point3d pStart, pEnd;
        public bool first = true;
        public double dis;
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            JigPromptPointOptions optJip = new JigPromptPointOptions("\n请指定第二点:");
            optJip.Cursor = CursorType.RubberBand;
            Point3d p2 = pStart.TransformBy(mt);
            optJip.BasePoint = p2;
            optJip.UseBasePoint = true;
            PromptPointResult ppr = prompts.AcquirePoint(optJip);
            Point3d cpt = ppr.Value;
            if (cpt != pEnd)
            {
                pEnd = cpt;
                if (first)
                {
                    dis = new Point2d(pStart.X, pStart.Y).GetDistanceTo(new Point2d(cpt.X, cpt.Y));
                }
                if (dis > 0)
                {
                    first = false;
                }

                Matrix3d mirrorMt = Matrix3d.Displacement(pStart.GetVectorTo(cpt));
                switch (Matrix3dType)
                {
                    case "J":
                        Line3d ml = new Line3d(cpt, pStart);
                        mirrorMt = Matrix3d.Mirroring(ml);
                        break;
                    case "Y":
                        mirrorMt = Matrix3d.Displacement(pStart.GetVectorTo(cpt));
                        break;
                    case "X":
                        Vector2d vt = new Point2d(pStart.X, pStart.Y) - new Point2d(cpt.X, cpt.Y);
                        mirrorMt = Matrix3d.Rotation(vt.Angle, Vector3d.ZAxis, pStart);
                        break;
                    case "S":
                        double dis2 = new Point2d(pStart.X, pStart.Y).GetDistanceTo(new Point2d(cpt.X, cpt.Y));

                        if (dis == 0.0)
                        {
                            mirrorMt = Matrix3d.Scaling(2, pStart);
                        }
                        else
                            mirrorMt = Matrix3d.Scaling((double)dis2 / dis, pStart);

                        break;

                }

                for (int i = 0; i < selectEntity.Length; i++)
                {
                    selectEntity[i].TransformBy(mtBack);
                    selectEntity[i].TransformBy(mirrorMt);

                }
                mtBack = mirrorMt.Inverse();
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.NoChange;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            for (int i = 0; i < selectEntity.Length; i++)
            {
                draw.Geometry.Draw(selectEntity[i]);
            }
            return true;
        }
        /// <summary>
        /// 操作实体
        /// </summary>
        /// <param name="select">对象集</param>
        /// <param name="p1">起始点</param>
        /// <param name="MatrixType">操作方式:J 镜像 Y 移动 X 旋转 S 缩放</param>
        public EntityChangeTools(Region[] select, Point3d p1, String MatrixType)
        {
            //selectEntity = select;
            selectEntity = select;
            pStart = p1;
            Matrix3dType = MatrixType;
        }


        public EntityChangeTools(List<Region> selectRegion, Point3d p1, String MatrixType)
        {
            Region[] regs = new Region[selectRegion.Count];
            for (int i=0;i<selectRegion.Count;i++)
            {
                regs[i] = selectRegion[i];
            }
            selectEntity = regs;
            pStart = p1;
            Matrix3dType = MatrixType;
        }
    }
}
