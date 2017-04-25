using Cecs475.Scheduling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test {
   class Program {
      static void Main(string[] args) {

         CatalogContext con = new CatalogContext();

         int choice = -1;
         do {
            Console.WriteLine("Menu:\n0. Quit\n1. Populate database\n2. Show courses\n3. Show course sections\n4. Print student details\n5. Register for course");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice) {
               case 1:
                  // Add some courses to the catalog
                  var cecs174 = new CatalogCourse() {
                     DepartmentName = "CECS",
                     CourseNumber = "174",
                  };
                  con.Courses.Add(cecs174);

                  var cecs274 = new CatalogCourse() {
                     DepartmentName = "CECS",
                     CourseNumber = "274",
                  };
                  cecs274.Prerequisites.Add(cecs174);
                  con.Courses.Add(cecs274);

                  var cecs228 = new CatalogCourse() {
                     DepartmentName = "CECS",
                     CourseNumber = "228",
                  };
                  cecs228.Prerequisites.Add(cecs174);
                  con.Courses.Add(cecs228);

                  var cecs277 = new CatalogCourse() {
                     DepartmentName = "CECS",
                     CourseNumber = "277",
                  };
                  cecs277.Prerequisites.Add(cecs274);
                  cecs277.Prerequisites.Add(cecs228);
                  con.Courses.Add(cecs277);

                  // Add a semester term
                  var spring2017 = new SemesterTerm() {
                     Name = "Spring 2017",
                     StartDate = new DateTime(2017, 1, 23),
                     EndDate = new DateTime(2017, 5, 26)
                  };
                  var fall2017 = new SemesterTerm() {
                     Name = "Fall 2017",
                     StartDate = new DateTime(2017, 8, 21),
                     EndDate = new DateTime(2017, 12, 22)
                  };
                  con.SemesterTerms.Add(fall2017);

                  // Add instructors
                  var neal = new Instructor() {
                     FirstName = "Neal",
                     LastName = "Terrell",
                  };
                  con.Instructors.Add(neal);
                  var anthony = new Instructor() {
                     FirstName = "Anthony",
                     LastName = "Giaccalone"
                  };
                  con.Instructors.Add(anthony);

                  // Add sections
                  var cecs174_99 = new CourseSection() {
                     CatalogCourse = cecs174,
                     SectionNumber = 99,
                     Instructor = neal,
                     MeetingDays = DaysOfWeek.Monday | DaysOfWeek.Wednesday,
                     StartTime = new DateTime(2017, 1, 1, 8, 0, 0), // 8 am
                     EndTime = new DateTime(2017, 1, 1, 8, 50, 0),
                  };
                  spring2017.CourseSections.Add(cecs174_99);

                  var cecs228_99 = new CourseSection() {
                     CatalogCourse = cecs228,
                     SectionNumber = 99,
                     Instructor = anthony,
                     MeetingDays = DaysOfWeek.Friday,
                     StartTime = new DateTime(2017, 1, 1, 10, 0, 0), // 10 am
                     EndTime = new DateTime(2017, 1, 1, 11, 50, 0),
                  };
                  spring2017.CourseSections.Add(cecs228_99);

                  var cecs228_01 = new CourseSection() {
                     CatalogCourse = cecs228,
                     SectionNumber = 1,
                     Instructor = neal,
                     MeetingDays = DaysOfWeek.Tuesday | DaysOfWeek.Thursday,
                     StartTime = new DateTime(2017, 1, 1, 9, 0, 0), // 9 am
                     EndTime = new DateTime(2017, 1, 1, 9, 50, 0),
                  };
                  fall2017.CourseSections.Add(cecs228_01);

                  var cecs274_05 = new CourseSection() {
                     CatalogCourse = cecs274,
                     SectionNumber = 5,
                     Instructor = anthony,
                     MeetingDays = DaysOfWeek.Monday | DaysOfWeek.Wednesday,
                     StartTime = new DateTime(2017, 1, 1, 9, 30, 0), // 9:30 am
                     EndTime = new DateTime(2017, 1, 1, 10, 20, 0),
                  };
                  fall2017.CourseSections.Add(cecs274_05);

                  var cecs274_11 = new CourseSection() {
                     CatalogCourse = cecs274,
                     SectionNumber = 11,
                     Instructor = anthony,
                     MeetingDays = DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday,
                     StartTime = new DateTime(2017, 1, 1, 13, 0, 0), // 1 pm
                     EndTime = new DateTime(2017, 1, 1, 13, 50, 0),
                  };
                  fall2017.CourseSections.Add(cecs274_11);

                  // Add Students
                  var studentDavid = new Student() {
                     FirstName = "David",
                     LastName = "Van",
                  };

                  CourseGrade davidCECS174 = new CourseGrade() {
                     Student = studentDavid,
                     Course = cecs174_99,
                     Grade = GradeTypes.A
                  };

                  CourseGrade davidCECS228 = new CourseGrade() {
                     Student = studentDavid,
                     Course = cecs228_99,
                     Grade = GradeTypes.D
                  };

                  cecs174_99.EnrolledStudents.Add(studentDavid);
                  cecs228_99.EnrolledStudents.Add(studentDavid);

                  studentDavid.Transcript.Add(davidCECS174);
                  studentDavid.Transcript.Add(davidCECS228);

                  var studentRyan = new Student() {
                     FirstName = "Ryan",
                     LastName = "Ea"
                  };

                  con.Students.Add(studentDavid);
                  con.Students.Add(studentRyan);

                  con.SaveChanges();
                  break;
               case 2:
                  // Print all courses in the catalog
                  foreach (var course in con.Courses.OrderBy(c => c.CourseNumber)) {
                     Console.Write($"{course.DepartmentName} {course.CourseNumber}");
                     if (course.Prerequisites.Count > 0) {
                        Console.Write(" (Prerequisites: ");
                        Console.Write(string.Join(", ", course.Prerequisites));
                        Console.Write(")");
                     }
                     Console.WriteLine();
                  }

                  break;

               case 3:
                  // Print all offered sections for Fall 2017
                  var fallSem = con.SemesterTerms.Where(s => s.Name == "Fall 2017").FirstOrDefault();
                  if (fallSem == null) {
                     break;
                  }

                  Console.WriteLine($"{fallSem.Name}: {fallSem.StartDate.ToString("MMM dd")} - {fallSem.EndDate.ToString("MMM dd")}");
                  foreach (var section in fallSem.CourseSections) {
                     Console.WriteLine($"{section.CatalogCourse}-{section.SectionNumber.ToString("D2")} -- " +
                        $"{section.Instructor.FirstName[0]} {section.Instructor.LastName} -- " +
                        $"{section.MeetingDays}, {section.StartTime.ToShortTimeString()} to {section.EndTime.ToShortTimeString()}");
                     if (section.EnrolledStudents.Count() > 0) {
                        Console.Write("Enrolled: ");
                        int i = 0;
                        foreach (var student in section.EnrolledStudents) {
                           Console.Write($"{student.LastName}, {student.FirstName}");
                           if (i < section.EnrolledStudents.Count() - 1) {
                              Console.Write("; ");
                           }
                           else {
                              Console.WriteLine();
                           }
                           i++;
                        }
                     }
                  }
                  break;
               case 4:
                  Console.Write("Enter the name of the student (First Name Last Name) you wish to find: ");
                  string[] nameForFind = Console.ReadLine().Split(' ');
                  var studentsListForFind = con.Students;
                  foreach (var student in studentsListForFind) {
                     if (student.FirstName == nameForFind[0] && student.LastName == nameForFind[1]) {
                        Console.WriteLine($"{student.FirstName} {student.LastName}");
                        if (student.Transcript.Count() == 0) {
                           Console.WriteLine("No Transcript.");
                        }
                        else {
                           int i = 0;
                           Console.Write("Transcript: ");
                           foreach (var grade in student.Transcript) {
                              Console.Write($"{grade.Course.CatalogCourse.DepartmentName} {grade.Course.CatalogCourse.CourseNumber} ({grade.Grade})");
                              if (i < student.Transcript.Count() - 1) {
                                 Console.Write(", ");
                              }
                              else {
                                 Console.WriteLine();
                              }
                              i++;
                           }
                        }
                        var currentSemesterCourses = student.EnrolledCourses.Where(c => c.Semester == con.SemesterTerms.Where(s => s.Name == "Fall 2017").First());
                        if (currentSemesterCourses.Count() == 0) {
                           Console.WriteLine("No Enrolled Courses.");
                        }
                        else {
                           int i = 0;
                           Console.Write("Enrolled: ");
                           foreach (var course in currentSemesterCourses) {
                              Console.Write($"{course.CatalogCourse.DepartmentName} {course.CatalogCourse.CourseNumber}-");
                              if (course.SectionNumber < 10) {
                                 Console.Write("0");
                              }
                              Console.Write($"{course.SectionNumber}");
                              if (i < currentSemesterCourses.Count() - 1) {
                                 Console.Write(", ");
                              }
                              else {
                                 Console.WriteLine();
                              }
                              i++;
                           }
                        }
                     }
                  }
                  break;
               case 5:
                  Console.WriteLine("Enter the name of the student you want to enroll (First Name Last Name): ");
                  string[] nameForRegister = Console.ReadLine().Split(' ');
                  Console.WriteLine("Enter the name of the course and the section number you want to enroll in (e.g. CECS 228-01): ");
                  string[] courseName = Console.ReadLine().Split(' ', '-');
                  var fall2017Semester = con.SemesterTerms.Where(sT => sT.Name == "Fall 2017").FirstOrDefault();

                  var studentsListForRegister = con.Students;
                  foreach (var student in studentsListForRegister) {
                     if (student.FirstName == nameForRegister[0] && student.LastName == nameForRegister[1]) {
                        if (fall2017Semester != null) {
                           var courses = fall2017Semester.CourseSections;
                           foreach (var course in courses) {
                              if (course.CatalogCourse.DepartmentName == courseName[0] && course.CatalogCourse.CourseNumber == courseName[1] && course.SectionNumber == Convert.ToInt32(courseName[2])) {
                                 var result = student.CanRegisterForCourseSection(course);
                                 switch (result) {
                                    case RegistrationResults.Success:
                                       course.EnrolledStudents.Add(student);
                                       student.EnrolledCourses.Add(course);
                                       Console.WriteLine("Course successfully registered!");
                                       break;
                                    case RegistrationResults.PrerequisitesNotMet:
                                       Console.WriteLine("Prerequisites Not Met!");
                                       break;
                                    case RegistrationResults.TimeConflict:
                                       Console.WriteLine("Time Conflict!");
                                       break;
                                    case RegistrationResults.AlreadyEnrolled:
                                       Console.WriteLine("Already Enrolled!");
                                       break;
                                    case RegistrationResults.AlreadyCompleted:
                                       Console.WriteLine("Already Completed!");
                                       break;
                                 }
                              }
                           }
                        }
                     }
                  }
                  con.SaveChanges();
                  break;
            }
            Console.WriteLine();
            Console.WriteLine();

         } while (choice != 0);
      }

   }
}