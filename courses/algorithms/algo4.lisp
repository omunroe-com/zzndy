;;;
;;; Calculate sizes of largest strongly connected components
;;; 

(defun cound-lines (file-name)
  (format t "Counting ...~%")
  (finish-output nil)
  (with-open-file (in file-name)
    (loop until (eq 'eof (read-line in nil 'eof))
          counting t)))

(defun string-to-list (line)
  (with-input-from-string (in line)
    (loop for token = (read in nil 'eof)
          until (eq token 'eof)
          collecting token)))

(defun read-graph (arr file-name)
  (format t "Reading ...~%")
  (finish-output nil)
  (with-open-file (in file-name)
    (loop for line = (read-line in nil 'eof) 
          until (eq line 'eof)
          for edge = (string-to-list line)
          counting t into num-edges
          do (setf (aref arr (1- num-edges) 0) (first edge)
                   (aref arr (1- num-edges) 1) (second edge))
          maximizing (apply #'max edge) into num-nodes
          collecting edge into graph
          finally (return num-nodes))))

(defun make-adjacency-array (num-nodes graph &optional (head-index 0) (tail-index 1))
  "Return an array of adjacency lists, where first element is source node name, rest are target elements"
  (loop with array = (make-array num-nodes :element-type 'list :initial-element (list))
        for i from 0 below (array-dimension graph 0)
        for head = (aref graph i head-index)
        for tail = (aref graph i tail-index)
        do (push tail (aref array (1- head)))
        finally (return array)))

(defun depth-first-search (adjacency-array visit-function 
                                           &optional 
                                           (init-function nil init-function-supplied-p)
                                           (index-function nil index-function-supplied-p))
  (let* ((num-nodes (array-dimension adjacency-array 0))
         (init most-negative-fixnum)
         (nodes-seen (make-array num-nodes :element-type 'fixnum :initial-element init))
         (results (make-array num-nodes :element-type 'fixnum :initial-element init)))
    (labels ((visited-p (node) (not (eql init (aref results (1- node)))))
             (seen-p (node) (not (eql init (aref nodes-seen (1- node)))))
             (mark-seen (node) (setf (aref nodes-seen (1- node)) 1))
             (mark-visited (node)  (setf (aref results (1- node) ) (funcall visit-function)))
             (visit (node)
               (unless (visited-p node)
                 (when init-function-supplied-p (funcall init-function node))
                 (mark-seen node)
                 (loop with stack = (list node)
                       until (null stack)
                       for head = (first stack)
                       for tails = (aref adjacency-array (1- head)) 
                       for fresh-tails = (unless (visited-p head) (loop for tail in tails
                                                                        unless (or (visited-p tail)
                                                                                   (seen-p tail))
                                                                        collect tail))
                       do (if fresh-tails 
                            (progn
                              (mapcar #'mark-seen fresh-tails)
                              (setq stack (append fresh-tails stack)))
                            (progn
                              (pop stack)
                              (unless (visited-p head) (mark-visited head))))))))
      (loop for index from 0 below num-nodes
            for node = (if index-function-supplied-p (funcall index-function index) (1+ index))
            unless (or (null node)
                     (visited-p node))
            do (visit node)
            finally (return results)))))

(defun get-finishing-times (num-nodes graph)
  (let ((finishing-time 0)) 
    (depth-first-search (make-adjacency-array num-nodes graph 1 0) 
                        (lambda () (incf finishing-time))))) 

(defun calculate-sccs (num-nodes graph finishing-times)
  (let ((leader num-nodes)
        (straight-adjacency-array (make-adjacency-array num-nodes graph))
        (sorted-adjacency-array (make-array num-nodes :element-type 'list :initial-element (list))))
    (labels ((map-index (old) (1- (aref finishing-times old))))
      (dotimes (i num-nodes)
        (setf (aref sorted-adjacency-array (- num-nodes (aref finishing-times i)))
          (mapcar (lambda (node)
                    (- num-nodes (aref finishing-times (1- node)) -1)) 
                  (aref straight-adjacency-array i))))) 
    (depth-first-search sorted-adjacency-array 
                        (lambda () leader)
                        (lambda (node)  (setq leader node)))))

(defun count-duplicates (num-nodes arr)
  "Return up to 10 sizes of most numerous groups of duplicate numbers from array.
   E.g.: #(1 1 1 2 2 3) -> (3 2 1)"
  (format t "~&Counting SCC sizes...~%")
  (loop with ht = (make-hash-table)
        with results
        for i being the elements of arr do
        (incf (gethash i ht 0))
        finally
        (maphash #'(lambda (k v) (setf results (cons v results))) ht)
        (setq results (sort results #'>))
        (return (if (> (length results) 10)
                  (subseq results 0 10)
                  results))))

(defun find-top-scc-sizes (graph num-nodes)
  (count-duplicates num-nodes
                    (calculate-sccs num-nodes graph
                                    (get-finishing-times num-nodes graph))))

(defun start-with-file (file-name)
  (defvar *num-edges* (cound-lines file-name)) 
  (defvar *graph* (make-array `(,*num-edges* 2) :element-type 'fixnum)) 
  (defvar *num-nodes* (read-graph *graph* file-name))
  (format t "~&Graph loaded: ~A nodes ~A edges~%" *num-nodes* *num-edges*)
  (format t "~&~A~%" (find-top-scc-sizes *graph* *num-nodes*)) )

;(time (start-with-file "smallgraph.txt"))
(time (start-with-file "SCC.txt"))


