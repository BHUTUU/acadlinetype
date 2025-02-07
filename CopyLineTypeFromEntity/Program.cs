using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using System;
using Autodesk.AutoCAD.Geometry;

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
                PromptEntityResult res = ed.GetEntity("Select the entity to check its linetype:");
                if (res.Status != PromptStatus.OK)
                    return;
                Entity ent = tr.GetObject(res.ObjectId, OpenMode.ForRead) as Entity;
                if (ent == null)
                    return;
                string linetype = ent.Linetype;
                LinetypeTable linetypeTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                LinetypeTableRecord existingLinetypeRecord = null;
                foreach (ObjectId ltId in linetypeTable)
                {
                    LinetypeTableRecord linetypeTableRecord = (LinetypeTableRecord)tr.GetObject(ltId, OpenMode.ForRead);
                    if (linetypeTableRecord.Name == linetype)
                    {
                        existingLinetypeRecord = linetypeTableRecord;
                        // ed.WriteMessage($"\nLinetype is '{existingLinetypeRecord.Name}' and ID '{ltId}'");
                        break;
                    }
                }
                string existingLinetypeName = existingLinetypeRecord.Name;
                string newLinetypeName = existingLinetypeName+"_r";
                LinetypeTable ltTable = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite);
                LinetypeTableRecord existingLinetype = (LinetypeTableRecord)tr.GetObject(ltTable[existingLinetypeName], OpenMode.ForRead);
                LinetypeTableRecord newLinetype = new LinetypeTableRecord();
                newLinetype.Name = newLinetypeName;
                newLinetype.AsciiDescription = existingLinetype.AsciiDescription;
                newLinetype.NumDashes = existingLinetype.NumDashes;
                newLinetype.PatternLength = existingLinetype.PatternLength;
                for (int i = 0; i < existingLinetype.NumDashes; i++)
                {
                    newLinetype.SetDashLengthAt(i, existingLinetype.DashLengthAt(i));
                }
                TextStyleTable tt =
                  (TextStyleTable)tr.GetObject(
                    db.TextStyleTableId,
                    OpenMode.ForRead
                  );
                newLinetype.SetShapeStyleAt(1, existingLinetype.ShapeStyleAt(1));
                newLinetype.SetShapeNumberAt(1, 0);
                newLinetype.SetShapeOffsetAt(1,existingLinetype.ShapeOffsetAt(1));
                newLinetype.SetShapeScaleAt(1, existingLinetype.ShapeScaleAt(1));
                newLinetype.SetShapeIsUcsOrientedAt(1, false);
                newLinetype.SetShapeRotationAt(1, 0);
                newLinetype.SetTextAt(1, existingLinetype.TextAt(1));
                ltTable.Add(newLinetype);
                tr.AddNewlyCreatedDBObject(newLinetype, true);
                tr.Commit();
            }
        }
    }
}