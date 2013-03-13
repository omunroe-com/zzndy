;;;
;;; Median maintenance cheting solution
;;; produce a sum of all medians when insterting numbers from a file one by one
;;; answer 46831213
;;; Supposed to use two heaps for quick lookup
;;;

(defun read-data (file-name)
  (with-open-file (in file-name)
    (loop with nums = (list)
          for num = (read in nil 'eof)
          until (eq num 'eof)
          do (push num nums)
          (setf nums (sort nums #'<))
          (print (/ (1+ (length nums)) 2))
          sum (if (evenp (length nums))
                (nth (1- (/ (length nums) 2)) nums)
                (nth (1- (/ (1+ (length nums)) 2)) nums)))))

(read-data "Median.txt")
