(defun read-graph (file-name)
  "Read adjacency list from a file, return a list of edges."
  (with-open-file (in file-name)
    (loop for line = (read-line in nil 'eof)
          until (eq line 'eof)
          collecting (string-to-list line ))))

(defun string-to-list (line)
  "Convert \"1 2 3 4 5\" to (1 2 3 4 5)."
  (with-input-from-string (in line)
    (loop for token = (read in nil 'eof)
          until (eq token 'eof)
          collecting token)))

(defun find-node (node graph)
  (find-if #'(lambda (i) (eql node (first i))) graph))

;; Input, graph, is in form ((from-1 to-1 to-2 to-3 ...)
;;                           (from-2 to-4 to-5 to-6 ...) ...)
;; where from-n and to-n are integers.
(defun remove-random-edge (graph)
  "Remove one random edge from a graph given as adjacency list."
  (let* ((node-list-1 (elt graph (random (length graph))))
         (node-1 (first node-list-1))
         (destinations (remove-duplicates (rest node-list-1)))
         (node-2 (elt destinations (random (length destinations))))
         (node-list-2 (find-node node-2 graph)))
    (flet ((replace-tail-for-head (node) (if (eql node node-2) node-1 node))
           (is-head-p (node) (eql node-1 node))
           (is-tail-p (node) (eql node-2 node))
           (starts-with-tail-p (nodes) (eql node-2 (first nodes))))
      (setf (rest node-list-1) (concatenate 'list 
                                 (rest node-list-1)
                                 (remove-if #'is-head-p (rest node-list-2)))) 
      (loop for node in (remove-duplicates (rest node-list-2))
            with match
            with repcd 
            do (setf match (find-node node graph))
            do (setf repcd (if (eql node node-1)
                             (remove-if #'is-tail-p (rest match))
                             (map 'list #'replace-tail-for-head (rest match))))
            do (setf (rest match) (sort repcd #'<))) 
      (remove-if #'starts-with-tail-p graph))))

(defun min-cut (graph)
  (loop until (eql 2 (length graph))
        do (setf graph (remove-random-edge graph))
        finally (return (length (cdar graph)))))

(defun find-min-cut (file-name tries)
  (loop repeat tries
        minimizing (min-cut (read-graph file-name))))

(time (print (find-min-cut "d:/workspace/playground/courses/algorithms/kargerMinCut.txt" 1000)))
