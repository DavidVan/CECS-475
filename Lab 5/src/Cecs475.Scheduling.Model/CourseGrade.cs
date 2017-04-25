using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.Scheduling.Model {

   public enum GradeTypes {
      A = 4,
      B = 3,
      C = 2,
      D = 1,
      F = 0
   }

   public class CourseGrade {
      public int Id { get; set; }
      public virtual Student Student { get; set; }
      public virtual CourseSection Course { get; set; }
      public virtual GradeTypes Grade { get; set; }
   }
}
