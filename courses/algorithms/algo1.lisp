(defun read-data (file-name)
  (with-open-file (in file-name)
    (loop for line = (read in nil 'eof)
          until (eq line 'eof)
          collect line)))

(defun small-list-p (list)
  (or (null list) (null (rest list))))

(defun split-in-half (list)
  (let ((mid (ceiling (length list) 2)))
    (values (subseq list 0 mid)
            (subseq list mid))))

(defun count-inversions (list)
  (if (small-list-p list) (list list 0)
    (multiple-value-bind (lower upper) (split-in-half list)
      (merge-inversions
        (count-inversions lower)
        (count-inversions upper)))))

(defmacro move-last (source target)
  `(setf ,target (nconc ,target (list (pop ,source)))))

(defun merge-inversions (lower-pair upper-pair )
  (loop 
    with lower = (first lower-pair)
    with upper = (first upper-pair)
    with merged-list = '()
    with num-inversions = 0
    while (not (and (null lower) (null upper)))
    do (cond
         ((null lower) (move-last upper merged-list))
         ((null upper) (move-last lower merged-list))
         ((< (first lower) (first upper)) (move-last lower merged-list))
         (t (move-last upper merged-list)
          (incf num-inversions (length lower)) ))
    finally (return (list merged-list (+ (second lower-pair) (second upper-pair) num-inversions)))))

(let ((file-name "D:\\workspace\\playground\\courses\\algorithms\\IntegerArray.txt"))
  (format t "~%Will calculate inversions in ~a~%" file-name)
  (time (second (count-inversions (read-data file-name))))) 
