;;;
;;; 2-SUM cheating solution
;;; produce a number of numbers t within interal [2500, 4000] such that 
;;; there exists a pair of distinct numbers x and y such that x + y = t
;;; answer 1477
;;; expected to use hashtables for y lookup
;;;

(defun read-data (file-name max-sum)
  (with-open-file (in file-name)
    (loop for num = (read in nil 'eof)
          until (eq num 'eof)
          when (<= num max-sum)
          collect num)))

(let ((file-name "HashInt.txt")
      (min-sum 2500)
      (max-sum 4000))
  (loop with data = (sort (read-data file-name max-sum) #'<)
        for target from min-sum to max-sum
        summing (loop for x in data
                      for y = (- target x)
                      when (and (not (eql y x))
                             (find y data))
                      do (return 1)
                      finally (return 0))))
