using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.Scheduling.Model {

   public enum RegistrationResults {
      Success,
      PrerequisitesNotMet,
      TimeConflict,
      AlreadyEnrolled,
      AlreadyCompleted
   }

   public class Student {
      public int Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public virtual ICollection<CourseGrade> Transcript { get; set; } = new List<CourseGrade>();
      public virtual ICollection<CourseSection> EnrolledCourses { get; set; } = new List<CourseSection>();

      //public virtual ICollection<CourseSection> CompletedCourses { get; set; } = new List<CourseSection>();

      public RegistrationResults CanRegisterForCourseSection(CourseSection section) {
         if (EnrolledCourses.Select(c => c.CatalogCourse).Contains(section.CatalogCourse) && Transcript.Where(c => c.Grade >= GradeTypes.C && c.Course.CatalogCourse == section.CatalogCourse).Count() == 0 && Transcript.Where(c => c.Grade < GradeTypes.C && c.Course.CatalogCourse == section.CatalogCourse).Count() == 0) {
            return RegistrationResults.AlreadyEnrolled;
         }
         if (Transcript.Where(c => c.Course.CatalogCourse == section.CatalogCourse && c.Grade >= GradeTypes.C).Count() != 0) {
            return RegistrationResults.AlreadyCompleted;
         }
         if (!section.CatalogCourse.Prerequisites.All(c => Transcript.Where(tC => tC.Grade >= GradeTypes.C).Select(cC => cC.Course.CatalogCourse).Contains(c))) {
            return RegistrationResults.PrerequisitesNotMet;
         }
         if (EnrolledCourses.Where(c => (c.MeetingDays & section.MeetingDays) > 0 && ContainsTimeConflict(c, section)).Count() != 0) {
            return RegistrationResults.TimeConflict;
         }
         return RegistrationResults.Success;
      }

      private bool ContainsTimeConflict(CourseSection first, CourseSection second) {
         if (first.StartTime == second.StartTime || first.EndTime == second.EndTime) {
            return true;
         }
         if (first.StartTime < second.StartTime) {
            if (first.EndTime > second.StartTime && first.EndTime < second.EndTime) {
               return true;
            }
            if (first.EndTime > second.EndTime) {
               return true;
            }
         }
         else {
            if (second.EndTime > first.StartTime && second.EndTime < first.EndTime) {
               return true;
            }
            if (second.EndTime > first.EndTime) {
               return true;
            }
         }
         return false;
      }
   }
}
