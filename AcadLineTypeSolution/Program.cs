using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using System.Text.RegularExpressions;
using System;

namespace AcadLineTypeSolution
{
    public class CopyLinetypeCommand
    {
        [CommandMethod("CopyLinetypeFromEntity")]
        public void CopyLinetypeFromEntity()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityResult res = ed.GetEntity("\nSelect an entity to copy linetype:");
                if (res.Status != PromptStatus.OK) return;

                Entity ent = tr.GetObject(res.ObjectId, OpenMode.ForRead) as Entity;
                if (ent == null)
                {
                    ed.WriteMessage("\nInvalid selection. Please select a valid entity.");
                    return;
                }
                string linetype = ent.Linetype;
                if(linetype.ToUpper() == "BYLAYER") {
                    Application.ShowAlertDialog("Please make sure linetype of the selected entity is not BYLAYER while copying the linetype.");
                    return;
                }
                LinetypeTable linetypeTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                if (!linetypeTable.Has(linetype))
                {
                    ed.WriteMessage($"\nLinetype '{linetype}' not found in database.");
                    return;
                }
                LinetypeTableRecord existingLinetype = (LinetypeTableRecord)tr.GetObject(linetypeTable[linetype], OpenMode.ForRead);
                string newLinetypeName = linetype + $"_copyOf{linetype}";
                LinetypeTable ltTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite);
                if (ltTable.Has(newLinetypeName))
                {
                    ed.WriteMessage($"\nLinetype '{newLinetypeName}' already exists.");
                    return;
                }
                LinetypeTableRecord newLinetype = new LinetypeTableRecord
                {
                    Name = newLinetypeName,
                    AsciiDescription = existingLinetype.AsciiDescription,
                    NumDashes = existingLinetype.NumDashes,
                    PatternLength = existingLinetype.PatternLength
                };
                for (int i = 0; i < existingLinetype.NumDashes; i++)
                {
                    newLinetype.SetDashLengthAt(i, existingLinetype.DashLengthAt(i));
                    newLinetype.SetShapeStyleAt(i, existingLinetype.ShapeStyleAt(i));
                    newLinetype.SetShapeOffsetAt(i, existingLinetype.ShapeOffsetAt(i));
                    newLinetype.SetShapeScaleAt(i, existingLinetype.ShapeScaleAt(i));
                    newLinetype.SetShapeRotationAt(i, existingLinetype.ShapeRotationAt(i));
                }
                if (existingLinetype.NumDashes > 1)
                {
                    newLinetype.SetShapeIsUcsOrientedAt(1, false);
                    newLinetype.SetTextAt(1, existingLinetype.TextAt(1));
                }
                ltTable.Add(newLinetype);
                tr.AddNewlyCreatedDBObject(newLinetype, true);
                tr.Commit();
                ed.WriteMessage($"\nLinetype '{newLinetypeName}' successfully created.");
            }
        }
    }
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
                if(originalLinetype.ToUpper() == "BYLAYER")
                {
                    Application.ShowAlertDialog("Please ensure that the selected polyline or line must not have BYLAYER as its linetype.");
                }
                string lineTypeNamePatternFormat = @"^(?<text>.+)_copyOf\k<text>$";
                if(Regex.IsMatch(originalLinetype, lineTypeNamePatternFormat)) {
                    Application.ShowAlertDialog($"Linetype '{originalLinetype}' already reversible using inbuilt reverse command.");
                    return;
                }
                if (ModifyLinetypeRotation(db, originalLinetype))
                {
                    ent.Linetype = originalLinetype;
                    ed.WriteMessage($"\nLinetype '{originalLinetype}' shape/text rotated by 180 degrees.");
                }
                else
                {
                    ed.WriteMessage($"\nLinetype '{originalLinetype}' does not contain shape/text elements.");
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
