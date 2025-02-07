using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace ForceReverseLinetype
{
    public class ForceReverse
    {
        [CommandMethod("Forcereverse")]
        public void ReverseLinetype()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a polyline or line: ");
                peo.SetRejectMessage("\nOnly polylines or lines are allowed.");
                peo.AddAllowedClass(typeof(Polyline), true);
                peo.AddAllowedClass(typeof(Line), true);
                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK) return;
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForWrite);
                string originalLinetype = ent.Linetype;
                if (ModifyLinetypeRotation(db, originalLinetype))
                {
                    ent.Linetype = originalLinetype;
                    Application.ShowAlertDialog($"Linetype '{originalLinetype}' shape/text rotated by 180 degrees.");
                }
                else
                {
                    Application.ShowAlertDialog($"Linetype '{originalLinetype}' does not contain shape/text elements.");
                }

                tr.Commit();
            }
        }
        private bool ModifyLinetypeRotation(Database db, string linetypeName)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LinetypeTable linetypeTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite);

                if (!linetypeTable.Has(linetypeName))
                {
                    Application.ShowAlertDialog($"Linetype '{linetypeName}' not found.");
                    return false;
                }
                LinetypeTableRecord ltr = (LinetypeTableRecord)tr.GetObject(linetypeTable[linetypeName], OpenMode.ForWrite);
                bool modified = false;
                double rotationAngle = Math.PI; // 180 degrees
                for (int i = 0; i < ltr.NumDashes; i++)
                {
                    if (ltr.ShapeStyleAt(i) != ObjectId.Null) // Check if dash has a shape
                    {
                        double currentRotation = ltr.ShapeRotationAt(i);
                        ltr.SetShapeRotationAt(i, (Math.Abs(currentRotation) < 0.001) ? rotationAngle : 0);
                        modified = true;
                    }
                }
                if (modified)
                    tr.Commit();
                return modified;
            }
        }
    }
}
