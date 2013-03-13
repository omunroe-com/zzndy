;;; 
;;; Calculate shortest path using Dijkstra's algorithm
;;;

(defun read-graph (file-name)
  (with-open-file (in file-name)
    (loop for line = (read-line in nil 'eof)
          until (eq line 'eof)
          collecting (read-adjacency (string-to-list (substitute #\Space #\, line))))))

(defun string-to-list (line)
  (with-input-from-string (in line)
    (loop for token = (read in nil 'eof) 
          until (eq token 'eof) 
          collecting token))) 

(defun read-adjacency (adjacency-list)
  (cons (first adjacency-list)
        (loop with arcs = (rest adjacency-list)
              for node = (pop arcs)
              for cost = (pop arcs)
              until (or (null node) (null cost))
              collecting (cons node cost))))

(defun dijkstra (source target graph)
  (labels ((min-path (&optional arc-1 arc-2)
             (cond ((and arc-1 arc-2) (if (< (cdr arc-1) (cdr arc-2)) arc-1 arc-2))
                   (arc-1 arc-1)
                   (arc-2 arc-2)
                   (t nil)))
           (get-best-path (visited)
             (reduce #'min-path 
                     (loop for pick in visited 
                           for start = (car pick) 
                           for cost = (cdr pick) 
                           append (loop for arc in (cdr (assoc start graph))
                                        unless (assoc (car arc) visited)
                                        collect (cons (car arc) (+ cost (cdr arc))))))))
    (loop with visited = (list (cons source 0))
          for best-arc = (get-best-path visited)
          when best-arc do
          (push best-arc visited)  
          until (or (null best-arc) (eql (car best-arc) target)) 
          finally (return (cdr best-arc)))))

(let ((file-name "dijkstraData.txt")
      (sources '(1))
      (targets '(7 37 59 82 99 115 133 165 188 197)))
  (loop for source in sources
        do (format t "From ~A to ~{~A~#[~:;,~]~}:~%~{~A~#[~:;,~]~}" source targets
                   (loop for target in targets
                         collecting (dijkstra source target (read-graph file-name))))))
