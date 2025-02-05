;;AIM: reverse the linetype which is not previously set to be reversible by creating a custom linetype and applying the same on the selected polyline only.
;;Date: 2025.01.31
;;Author: Suman Kumar, sumankumar91356865@gmail.com
(defun c:forcereverse ( / ent entData linetypeName newLinetypeName linetypeExists scaleFactor textLength segmentLength spaceLength offsetLength)
  (setq ent (car (entsel "\nSelect a polyline or line: ")))
  (if ent
    (progn
      (setq entData (entget ent))
      (setq linetypeName (cdr (assoc 6 entData)))
      (if linetypeName
        (progn
          (if (wcmatch linetypeName "*_r")
            (progn
              (command "_.reverse" ent "")
              (alert (strcat "Entity reversed using existing linetype, i.e. " linetypeName "."))
            )
            (progn
              (setq _TEXTTOEMBED linetypeName)
              (setq newLinetypeName (strcat linetypeName "_r"))
              (setq linetypeExists (tblsearch "LTYPE" newLinetypeName))
              (if (not linetypeExists)
                (progn
                  (generate-dynamic-linetype)
                )
              )
              (entmod (subst (cons 6 newLinetypeName) (assoc 6 entData) entData))
              (command "_.reverse" ent "")
              (alert (strcat "Linetype applied: " newLinetypeName ". Please change the linetype scale if required."))
            )
          )
        )
        (alert "The selected entity does not have a linetype.")
      )
    )
    (alert "No entity selected.")
  )
  (princ)
)
(defun calculate-bounding-box (text / doc mspace obj minb maxb pts)
  (setq doc (vla-get-ActiveDocument (vlax-get-acad-object)))
  (setq mspace (vla-get-ModelSpace doc))
  (vla-addtext mspace text (vlax-3d-point '(0 0 0)) 1.0)
  (setq obj (vlax-ename->vla-object (entlast)))
  (vla-put-ScaleFactor obj 1.0)
  (if (vlax-method-applicable-p obj 'getboundingbox)
    (progn
      (vla-GetBoundingBox obj 'minb 'maxb)
      (setq minb (vlax-safearray->list minb))
      (setq maxb (vlax-safearray->list maxb))
      (setq pts (list (trans minb 0 1) (trans maxb 0 1)))
    )
  )
  (entdel (entlast))
  (list (caar pts) (caadr pts))
)
(defun generate-dynamic-linetype (/ pts cdate scaleFactor textWidth segmentLength spaceLength offsetLength tempFile tempFilePath tempFileHandle isMetric)
  (setq scaleFactor 0.1)
  (setq textWidth (* scaleFactor 1.0))
  (setq segmentLength 2.5)
  (setq spaceLength 0.25)
  (setq offsetLength (- segmentLength textWidth))
  (setq pts (calculate-bounding-box _TEXTTOEMBED))
  (setq textWidth (abs (- (cadr pts) (car pts))))
  (setq isMetric (= 1 (getvar "measurement")))
  (setq tempFile (vl-filename-mktemp))
  (setq tempFilePath (vl-filename-directory tempFile))
  (vl-file-delete tempFile)
  (setq tempFileHandle (open (strcat tempFilePath "\\generated_linetype.lin") "w"))
  (write-line (strcat "*" newLinetypeName ", Custom dynamic linetype") tempFileHandle)
  (write-line 
    (strcat
      "A," (if isMetric "12.7,-5.08" ".5,-.2") ",[\""
      _TEXTTOEMBED "\"," (getvar "TEXTSTYLE") ",S=" (if isMetric "2.54" ".1") ",R=1.0,"  
      (if isMetric "X=-2.54,Y=-1.27" "X=-0.1,Y=-.05") "],-" 
      (rtos (- (* textWidth (if isMetric 2.54 0.1)) (if isMetric 1.14 0.04)) 2 2)
    ) 
    tempFileHandle
  )
  (close tempFileHandle)
  (command "_-linetype" "_lo" newLinetypeName (strcat tempFilePath "\\generated_linetype.lin") "")
  (vl-file-delete (strcat tempFilePath "\\generated_linetype.lin"))
)
(princ)
