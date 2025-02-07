using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace CopyLinetypeFromEntity
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
                // Prompt the user to select a line or polyline
                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a line or polyline: ");
                peo.SetRejectMessage("\nOnly lines or polylines are allowed.");
                peo.AddAllowedClass(typeof(Line), true);
                peo.AddAllowedClass(typeof(Polyline), true);

                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK) return;

                // Get the selected entity
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                // Get the linetype of the selected entity
                string originalLinetypeName = ent.Linetype;

                // Open the LinetypeTable for read
                LinetypeTable linetypeTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);

                // Check if the linetype exists
                if (!linetypeTable.Has(originalLinetypeName))
                {
                    ed.WriteMessage($"\nLinetype '{originalLinetypeName}' does not exist.");
                    return;
                }

                // Get the original linetype
                LinetypeTableRecord originalLtr = (LinetypeTableRecord)tr.GetObject(
                    linetypeTable[originalLinetypeName], OpenMode.ForRead);

                // Create a new linetype name with "_r" suffix
                string newLinetypeName = originalLinetypeName + "_r";

                // Check if the new linetype name already exists
                if (linetypeTable.Has(newLinetypeName))
                {
                    ed.WriteMessage($"\nLinetype '{newLinetypeName}' already exists.");
                    return;
                }

                // Create a copy of the original linetype
                LinetypeTableRecord newLtr = new LinetypeTableRecord
                {
                    Name = newLinetypeName,
                    AsciiDescription = originalLtr.AsciiDescription,
                    PatternLength = originalLtr.PatternLength,
                    NumDashes = originalLtr.NumDashes
                };

                // Copy the dash elements
                for (int i = 0; i < originalLtr.NumDashes; i++)
                {
                    newLtr.SetDashLengthAt(i, originalLtr.DashLengthAt(i));
                    newLtr.SetShapeStyleAt(i, originalLtr.ShapeStyleAt(i));
                    newLtr.SetShapeNumberAt(i, originalLtr.ShapeNumberAt(i));
                    newLtr.SetShapeOffsetAt(i, originalLtr.ShapeOffsetAt(i));
                    newLtr.SetShapeRotationAt(i, originalLtr.ShapeRotationAt(i));
                    newLtr.SetShapeScaleAt(i, originalLtr.ShapeScaleAt(i));
                    newLtr.SetTextAt(i, originalLtr.TextAt(i));
                    // newLtr.SetTextStyleAt(i, originalLtr.TextStyleAt(i));
                    // newLtr.SetTextHeightAt(i, originalLtr.TextHeightAt(i));
                    // newLtr.SetTextOffsetAt(i, originalLtr.TextOffsetAt(i));
                    // newLtr.SetTextRotationAt(i, originalLtr.TextRotationAt(i));
                }

                // Add the new linetype to the LinetypeTable
                linetypeTable.UpgradeOpen();
                linetypeTable.Add(newLtr);
                tr.AddNewlyCreatedDBObject(newLtr, true);

                ed.WriteMessage($"\nLinetype '{newLinetypeName}' created successfully.");

                tr.Commit();
            }
        }
    }
}