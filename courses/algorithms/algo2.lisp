(defun read-data (file-name)
  (with-open-file (in file-name)
    (loop for line = (read in nil 'eof)
          until (eq line 'eof)
          collect line)))

(defun first-for-pivot (array first last) )

(defun last-for-pivot (array first last)
  (rotatef (elt array first) (elt array last)))

(defun median-for-pivot (array first last)
  (let* ((a (elt array first))
         (middle (+ first (floor (/ (- last first) 2))))
         (b (elt array middle))
         (c (elt array last)))
    (cond
      ((or (> a b c)
           (< a b c))  (rotatef (elt array first) (elt array middle)))
      ((or (> a c b)
           (< a c b))  (rotatef (elt array first) (elt array last))))))

(defun quick-sort (array pivot-selector first last)
  (if (< (- last first) 1) (return-from quick-sort 0))
  (funcall pivot-selector array first last)
  (loop for i from (1+ first) to last
        with pivot = (elt array first)
        with j = (1+ first)
        when (< (elt array i) pivot)
        do (progn 
             (rotatef (elt array j) (elt array i))
             (incf j))
        finally (progn
                  (rotatef (elt array first) (elt array (1- j)))
                  (return (+ (- last first)
                             (quick-sort array pivot-selector first (- j 2))
                             (quick-sort array pivot-selector j last))))))

(defun run-sort (list pivot-selector)
  (let ((a (make-array (list (length list)) :initial-contents list))) 
    ;(format t "~& Using ~a~%" pivot-selector)
    ;(format t "~& Before ~a~%" a) 
    (format t "~a ~a~%" pivot-selector (quick-sort a pivot-selector 0 (1- (length a)))) 
    ;(format t "~& After  ~a~%" a)
    ))

(let ((a (read-data "quick-sort.txt")))
  (run-sort a #'first-for-pivot)
  (run-sort a #'last-for-pivot)
  (run-sort a #'median-for-pivot))

